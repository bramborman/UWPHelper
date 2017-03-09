using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UWPHelper.Utilities;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed partial class BarsHelper
    {
        private static readonly bool isStatusBarTypePresent = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

        public static BarsHelper Current { get; private set; }

        private readonly Dictionary<int, int> viewInfo = new Dictionary<int, int>();

        private bool isTitleBarColorModeSet;
        private bool isStatusBarColorModeSet;
        private IBarsHelperTitleBarColorsSetter titleBarColorsSetter;
        private IBarsHelperStatusBarColorsSetter statusBarColorsSetter;
        private bool _useDarkerStatusBarOnLandscapeOrientation;
        private BarsHelperColorMode _titleBarColorMode;
        private BarsHelperColorMode _statusBarColorMode;
        private Func<ElementTheme> _requestedThemeGetter;
        private INotifyPropertyChanged _requestedThemePropertyParent;
        private string _requestedThemePropertyName;

        public bool IsInitialized
        {
            get
            {
                return viewInfo.Count > 0;
            }
        }
        public bool UseDarkerStatusBarOnLandscapeOrientation
        {
            get { return _useDarkerStatusBarOnLandscapeOrientation; }
            set
            {
                if (_useDarkerStatusBarOnLandscapeOrientation != value)
                {
                    _useDarkerStatusBarOnLandscapeOrientation = value;

                    if (isStatusBarTypePresent)
                    {
                        bool cachedValue = _useDarkerStatusBarOnLandscapeOrientation;
                        ViewHelper.RunOnEachViewDispatcher(() => InitializeUseDarkerStatusBarOnLandscapeOrientationForCurrentView(cachedValue));

                        TrySetStatusBarColorsAsync();
                    }
                }
            }
        }
        public BarsHelperColorMode TitleBarColorMode
        {
            get { return _titleBarColorMode; }
            set
            {
                SetColorMode(true, ref _titleBarColorMode, ref value, ref isTitleBarColorModeSet, () =>
                {
                    switch (_titleBarColorMode)
                    {
                        case BarsHelperColorMode.Themed:
                            titleBarColorsSetter = new BarsHelperTitleBarColorsSetterThemed();
                            break;

                        case BarsHelperColorMode.ThemedGray:
                            titleBarColorsSetter = new BarsHelperTitleBarColorsSetterThemedGray();
                            break;

                        case BarsHelperColorMode.Accent:
                            titleBarColorsSetter = new BarsHelperTitleBarColorsSetterAccent();
                            break;
                    }
                });
            }
        }
        public BarsHelperColorMode StatusBarColorMode
        {
            get { return _statusBarColorMode; }
            set
            {
                SetColorMode(false, ref _statusBarColorMode, ref value, ref isStatusBarColorModeSet, () =>
                {
                    switch (_statusBarColorMode)
                    {
                        case BarsHelperColorMode.Themed:
                            statusBarColorsSetter = new BarsHelperStatusBarColorsSetterThemed();
                            break;
                        
                        case BarsHelperColorMode.ThemedGray:
                            statusBarColorsSetter = new BarsHelperStatusBarColorsSetterThemedGray();
                            break;
                        
                        case BarsHelperColorMode.Accent:
                            statusBarColorsSetter = new BarsHelperStatusBarColorsSetterAccent();
                            break;
                    }
                });
            }
        }
        public Func<ElementTheme> RequestedThemeGetter
        {
            get { return _requestedThemeGetter; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!ReferenceEquals(_requestedThemeGetter, value))
                {
                    _requestedThemeGetter = value;
                    TrySetBarsColorsAsync();
                }
            }
        }
        public INotifyPropertyChanged RequestedThemePropertyParent
        {
            get { return _requestedThemePropertyParent; }
            private set
            {
                // Validated in InitializeAutoUpdating method
                if (!ReferenceEquals(_requestedThemePropertyParent, value))
                {
                    if (_requestedThemePropertyParent != null)
                    {
                        _requestedThemePropertyParent.PropertyChanged -= RequestedThemePropertyParent_PropertyChanged;
                    }

                    _requestedThemePropertyParent = value;

                    // Will be null after calling the UnitializeAutoUpdating method
                    if (_requestedThemePropertyParent != null)
                    {
                        _requestedThemePropertyParent.PropertyChanged += RequestedThemePropertyParent_PropertyChanged;
                        //TODO: auto updating for ColorMode.ThemedGray when system theme is changed
                    }

                    // Not calling the TrySetBarsColorsAsync method since this hasn't got any impact on bars colors
                }
            }
        }

        public string RequestedThemePropertyName
        {
            get { return _requestedThemePropertyName; }
            private set
            {
                // Validated in InitializeAutoUpdating method
                // No need for equality checking since we're not invoking anything from here
                _requestedThemePropertyName = value;
            }
        }

        static BarsHelper()
        {
            Current = new BarsHelper();
        }

        // Prevent from creating new instances
        private BarsHelper()
        {

        }
        
        public async Task InitializeForCurrentViewAsync()
        {
            if (RequestedThemeGetter == null)
            {
                RequestedThemeGetter = () => ElementTheme.Default;
            }

            if (!isTitleBarColorModeSet)
            {
                TitleBarColorMode = default(BarsHelperColorMode);
            }

            if (!isStatusBarColorModeSet)
            {
                StatusBarColorMode = default(BarsHelperColorMode);
            }

            int currentViewId = ViewHelper.GetCurrentViewId();

            if (viewInfo.ContainsKey(currentViewId))
            {
                throw new InvalidOperationException($"{nameof(BarsHelper)} is already initialized for this view.");
            }

            viewInfo.Add(currentViewId, 0);
            TrySetBarsColorsAsync();

            if (isStatusBarTypePresent && UseDarkerStatusBarOnLandscapeOrientation)
            {
                await ViewHelper.RunOnCurrentViewDispatcherAsync(() => InitializeUseDarkerStatusBarOnLandscapeOrientationForCurrentView(true));
            }

            await ViewHelper.RunOnCurrentViewDispatcherAsync(() => InitializeColorModeForCurrentView(true, true, TitleBarColorMode));
            await ViewHelper.RunOnCurrentViewDispatcherAsync(() => InitializeColorModeForCurrentView(false, true, StatusBarColorMode));
        }

        public void InitializeAutoUpdating(Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged requestedThemePropertyParent, string requestedThemePropertyName)
        {
            if (string.IsNullOrWhiteSpace(requestedThemePropertyName))
            {
                throw new ArgumentException("Value cannot be empty or null.", nameof(requestedThemePropertyName));
            }

            RequestedThemeGetter            = requestedThemeGetter;
            // Assign this before attaching an event handler to parent
            RequestedThemePropertyName      = requestedThemePropertyName;
            RequestedThemePropertyParent    = requestedThemePropertyParent ?? throw new ArgumentNullException(nameof(requestedThemePropertyParent));
        }

        public void UnitializeAutoUpdating()
        {
            RequestedThemePropertyName      = null;
            RequestedThemePropertyParent    = null;
        }

        private void RequestedThemePropertyParent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RequestedThemePropertyName)
            {
                TrySetBarsColorsAsync();
            }
        }

        private void InitializeUseDarkerStatusBarOnLandscapeOrientationForCurrentView(bool initialize)
        {
            if (initialize)
            {
                DisplayInformation.GetForCurrentView().OrientationChanged += DisplayInformation_OrientationChanged;
            }
            else
            {
                DisplayInformation.GetForCurrentView().OrientationChanged -= DisplayInformation_OrientationChanged;
            }
        }

        private void InitializeColorModeForCurrentView(bool isTitleBarColorMode, bool initialize, BarsHelperColorMode colorMode)
        {
            switch (colorMode)
            {
                case BarsHelperColorMode.ThemedGray:
                    if (!isTitleBarColorMode && isStatusBarTypePresent)
                    {
                        if (initialize)
                        {
                            Window.Current.Activated += Window_Activated;
                        }
                        else
                        {
                            Window.Current.Activated -= Window_Activated;
                        }
                    }

                    break;
                
                case BarsHelperColorMode.Accent:
                    const string EXCEPTION_TEXT = "Internal error in BarsHelper initialization - {0}";
                    int currentViewId = ViewHelper.GetCurrentViewId();

                    if (initialize)
                    {
                        if (viewInfo[currentViewId] == 0)
                        {
                            AccentColorHelper.GetForCurrentView().AccentColorChanged += AccentColorHelper_AccentColorChanged;
                        }

                        if (++viewInfo[currentViewId] > 2)
                        {
                            throw new Exception(string.Format(EXCEPTION_TEXT, viewInfo[currentViewId]));
                        }
                    }
                    else
                    {
                        if (--viewInfo[currentViewId] < 0)
                        {
                            throw new Exception(string.Format(EXCEPTION_TEXT, viewInfo[currentViewId]));
                        }

                        if (viewInfo[currentViewId] == 0)
                        {
                            AccentColorHelper.GetForCurrentView().AccentColorChanged -= AccentColorHelper_AccentColorChanged;
                        }
                    }

                    break;
            }
        }

        private void SetColorMode(bool isTitleBarColorMode, ref BarsHelperColorMode colorMode, ref BarsHelperColorMode value, ref bool colorModeSet, Action switchColorsSetter)
        {
            if (colorMode != value || !colorModeSet)
            {
                colorModeSet = true;

                BarsHelperColorMode cachedValue = colorMode;
                ViewHelper.RunOnEachViewDispatcher(() => InitializeColorModeForCurrentView(isTitleBarColorMode, false, cachedValue));

                colorMode = value;
                switchColorsSetter();

                cachedValue = colorMode;
                ViewHelper.RunOnEachViewDispatcher(() => InitializeColorModeForCurrentView(isTitleBarColorMode, true, cachedValue));

                if (isTitleBarColorMode)
                {
                    TrySetTitleBarColorsAsync();
                }
                else
                {
                    TrySetStatusBarColorsAsync();
                }
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                TrySetStatusBarColorsAsync();
            }
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            TrySetStatusBarColorsAsync();
        }

        private void AccentColorHelper_AccentColorChanged(AccentColorHelper sender, Color args)
        {
            TrySetBarsColorsAsync();
        }

        private async void TrySetBarsColorsAsync()
        {
            if (IsInitialized)
            {
                await SetBarsColorsAsync();
            }
        }

        private async void TrySetTitleBarColorsAsync()
        {
            if (IsInitialized)
            {
                await SetTitleBarColorsAsync();
            }
        }

        private async void TrySetStatusBarColorsAsync()
        {
            if (IsInitialized)
            {
                await SetStatusBarColorsAsync();
            }
        }

        public async Task SetBarsColorsAsync()
        {
            await SetTitleBarColorsAsync();
            await SetStatusBarColorsAsync();
        }

        public Task SetTitleBarColorsAsync()
        {
            ValidateInitialization();

            // Cache the value to prevent unnecessary calls and prevent from changing the value while setting the colors
            ElementTheme requestedTheme = RequestedThemeGetter();

            return ViewHelper.RunOnEachViewDispatcherAsync(() =>
            {
                if (ApplicationView.GetForCurrentView().TitleBar is ApplicationViewTitleBar titleBar)
                {
                    titleBarColorsSetter.SetTitleBarColors(titleBar, requestedTheme);
                }
            });
        }

        public Task SetStatusBarColorsAsync()
        {
            ValidateInitialization();

            if (isStatusBarTypePresent)
            {
                // Cache the values to prevent unnecessary calls and prevent from changing the values while setting the colors
                bool useDarkerStatusBarOnLandscapeOrientation = UseDarkerStatusBarOnLandscapeOrientation;
                ElementTheme requestedTheme = RequestedThemeGetter();

                return ViewHelper.RunOnEachViewDispatcherAsync(() =>
                {
                    if (StatusBar.GetForCurrentView() is StatusBar statusBar)
                    {
                        statusBarColorsSetter.SetStatusBarColors(statusBar, requestedTheme, useDarkerStatusBarOnLandscapeOrientation, DisplayInformation.GetForCurrentView().CurrentOrientation);
                    }
                });
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        private void ValidateInitialization()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(BarsHelper)} is not initialized for any view. Call the {nameof(InitializeForCurrentViewAsync)} method first.");
            }
        }
    }
}
