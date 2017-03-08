using System;
using System.Collections.Generic;
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

        private readonly List<int> viewIds = new List<int>();

        private bool isTitleBarColorModeSet;
        private bool isStatusBarColorModeSet;
        private IBarsHelperTitleBarColorsSetter titleBarColorsSetter;
        private IBarsHelperStatusBarColorsSetter statusBarColorsSetter;
        private bool _useDarkerStatusBarOnLandscapeOrientation;
        private BarsHelperColorMode _titleBarColorMode;
        private BarsHelperColorMode _statusBarColorMode;
        private ElementTheme _requestedTheme;

        public bool IsInitialized
        {
            get
            {
                return viewIds.Count > 0;
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
        public ElementTheme RequestedTheme
        {
            get { return _requestedTheme; }
            set
            {
                if (_requestedTheme != value)
                {
                    _requestedTheme = value;
                    TrySetBarsColorsAsync();
                }
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
            if (!isTitleBarColorModeSet)
            {
                TitleBarColorMode = default(BarsHelperColorMode);
            }

            if (!isStatusBarColorModeSet)
            {
                StatusBarColorMode = default(BarsHelperColorMode);
            }

            int currentViewId = ViewHelper.GetCurrentViewId();

            if (viewIds.Contains(currentViewId))
            {
                throw new InvalidOperationException($"{nameof(BarsHelper)} is already initialized for this view.");
            }

            viewIds.Add(currentViewId);
            TrySetBarsColorsAsync();

            if (isStatusBarTypePresent && UseDarkerStatusBarOnLandscapeOrientation)
            {
                await ViewHelper.RunOnCurrentViewDispatcherAsync(() => InitializeUseDarkerStatusBarOnLandscapeOrientationForCurrentView(true));
            }

            await ViewHelper.RunOnCurrentViewDispatcherAsync(() => InitializeColorModeForCurrentView(true, true, TitleBarColorMode));
            await ViewHelper.RunOnCurrentViewDispatcherAsync(() => InitializeColorModeForCurrentView(false, true, StatusBarColorMode));
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
                    if (initialize)
                    {
                        //TODO: avoid multiple handlers for one view
                        AccentColorHelper.GetForCurrentView().AccentColorChanged += AccentColorHelper_AccentColorChanged;
                    }
                    else
                    {
                        AccentColorHelper.GetForCurrentView().AccentColorChanged -= AccentColorHelper_AccentColorChanged;
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
                TrySetBarsColorsAsync();
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

        private async void TrySetStatusBarColorsAsync()
        {
            if (IsInitialized)
            {
                await SetStatusBarColorsAsync();
            }
        }

        private async void TrySetBarsColorsAsync()
        {
            if (IsInitialized)
            {
                await SetBarsColorsAsync();
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
            ElementTheme requestedTheme = RequestedTheme;

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
                ElementTheme requestedTheme = RequestedTheme;

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
