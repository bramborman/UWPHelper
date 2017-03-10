﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private readonly List<int> viewInfo = new List<int>();

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
        private OneEventHandlerHelper _windowActivatedEventHandlerHelper;
        private OneEventHandlerHelper _accentColorHelperColorChangedEventHandlerHelper;

        private OneEventHandlerHelper WindowActivatedEventHandlerHelper
        {
            get
            {
                if (_windowActivatedEventHandlerHelper == null)
                {
                    _windowActivatedEventHandlerHelper = new OneEventHandlerHelper();
                }

                return _windowActivatedEventHandlerHelper;
            }
        }
        private OneEventHandlerHelper AccentColorHelperColorChangedEventHandlerHelper
        {
            get
            {
                if (_accentColorHelperColorChangedEventHandlerHelper == null)
                {
                    _accentColorHelperColorChangedEventHandlerHelper = new OneEventHandlerHelper();
                }

                return _accentColorHelperColorChangedEventHandlerHelper;
            }
        }

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
        }
        public BarsHelperColorMode TitleBarColorMode
        {
            get { return _titleBarColorMode; }
        }
        public BarsHelperColorMode StatusBarColorMode
        {
            get { return _statusBarColorMode; }
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

        public async Task SetUseDarkerStatusBarOnLandscapeOrientationAsync(bool value)
        {
            if (_useDarkerStatusBarOnLandscapeOrientation != value)
            {
                _useDarkerStatusBarOnLandscapeOrientation = value;

                if (isStatusBarTypePresent)
                {
                    bool cachedValue = _useDarkerStatusBarOnLandscapeOrientation;
                    await RunOnEachInitializedViewDispatcherAsync(() => InitializeUseDarkerStatusBarOnLandscapeOrientationForCurrentView(cachedValue));

                    TrySetStatusBarColorsAsync();
                }
            }
        }

        public Task SetTitleBarColorModeAsync(BarsHelperColorMode value)
        {
            return SetColorModeAsync(true, () => _titleBarColorMode, cm => _titleBarColorMode = cm, value, () => isTitleBarColorModeSet, cms => isTitleBarColorModeSet = cms, nameof(TitleBarColorMode), () =>
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

        public Task SetStatusBarColorModeAsync(BarsHelperColorMode value)
        {
            return SetColorModeAsync(false, () => _statusBarColorMode, cm => _statusBarColorMode = cm, value, () => isStatusBarColorModeSet, cms => isStatusBarColorModeSet = cms, nameof(StatusBarColorMode), () =>
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
        
        private async Task SetColorModeAsync(bool isTitleBarColorMode, Func<BarsHelperColorMode> colorModeGetter, Action<BarsHelperColorMode> colorModeSetter, BarsHelperColorMode value, Func<bool> colorModeSetGetter, Action<bool> colorModeSetSetter, string propertyName, Action switchColorsSetter)
        {
            if (!Enum.IsDefined(typeof(BarsHelperColorMode), value))
            {
                throw new ArgumentOutOfRangeException(propertyName);
            }

            BarsHelperColorMode cachedValue = colorModeGetter();

            if (cachedValue != value || !colorModeSetGetter())
            {
                colorModeSetSetter(true);
                await RunOnEachInitializedViewDispatcherAsync(() => InitializeColorModeForCurrentView(isTitleBarColorMode, false, cachedValue));

                colorModeSetter(value);
                switchColorsSetter();

                cachedValue = value;
                await RunOnEachInitializedViewDispatcherAsync(() => InitializeColorModeForCurrentView(isTitleBarColorMode, true, cachedValue));

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

        public async Task InitializeForCurrentViewAsync()
        {
            if (RequestedThemeGetter == null)
            {
                RequestedThemeGetter = () => ElementTheme.Default;
            }

            if (!isTitleBarColorModeSet)
            {
                await SetTitleBarColorModeAsync(default(BarsHelperColorMode));
            }

            if (!isStatusBarColorModeSet)
            {
                await SetStatusBarColorModeAsync(default(BarsHelperColorMode));
            }

            int currentViewId = ViewHelper.GetCurrentViewId();

            if (viewInfo.Contains(currentViewId))
            {
                throw new InvalidOperationException($"{nameof(BarsHelper)} is already initialized for this view.");
            }

            viewInfo.Add(currentViewId);
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
                    if (initialize)
                    {
                        WindowActivatedEventHandlerHelper.AddHandler(() =>
                        {
                            Window.Current.Activated += Window_Activated;
                        });
                    }
                    else
                    {
                        WindowActivatedEventHandlerHelper.RemoveHandler(() =>
                        {
                            Window.Current.Activated -= Window_Activated;
                        });
                    }

                    break;
                
                case BarsHelperColorMode.Accent:
                    if (initialize)
                    {
                        AccentColorHelperColorChangedEventHandlerHelper.AddHandler(() =>
                        {
                            AccentColorHelper.GetForCurrentView().AccentColorChanged += AccentColorHelper_AccentColorChanged;
                        });
                    }
                    else
                    {
                        AccentColorHelperColorChangedEventHandlerHelper.RemoveHandler(() =>
                        {
                            AccentColorHelper.GetForCurrentView().AccentColorChanged -= AccentColorHelper_AccentColorChanged;
                        });
                    }

                    break;
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                TrySetBarsColorsAsync();
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

            return RunOnEachInitializedViewDispatcherAsync(() =>
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

                return RunOnEachInitializedViewDispatcherAsync(() =>
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

        private Task RunOnEachInitializedViewDispatcherAsync(Action action)
        {
            return ViewHelper.RunOnEachViewDispatcherAsync(() =>
            {
                int currentViewId = ViewHelper.GetCurrentViewId();

                if (viewInfo.Any(id => id == currentViewId))
                {
                    action();
                }
            });
        }
    }
}
