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
        
        private bool colorModeSet = false;
        private IBarsHelperColorsSetter colorsSetter;
        private bool _useDarkerStatusBarOnLandscapeOrientation;
        private BarsHelperColorMode _colorMode;
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
        public BarsHelperColorMode ColorMode
        {
            get { return _colorMode; }
            set
            {
                if (_colorMode != value || !colorModeSet)
                {
                    colorModeSet = true;

                    BarsHelperColorMode cachedValue = _colorMode;
                    ViewHelper.RunOnEachViewDispatcher(() => InitializeColorModeForCurrentView(false, cachedValue));

                    _colorMode = value;

                    switch (_colorMode)
                    {
                        case BarsHelperColorMode.Themed:
                            colorsSetter = new BarsHelperColorsSetterDefault();
                            break;
                        
                        case BarsHelperColorMode.Accent:
                            colorsSetter = new BarsHelperColorsSetterAccent();
                            break;
                    }

                    cachedValue = _colorMode;
                    ViewHelper.RunOnEachViewDispatcher(() => InitializeColorModeForCurrentView(true, cachedValue));
                    TrySetBarsColorsAsync();
                }
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
            UseDarkerStatusBarOnLandscapeOrientation = false;
            ColorMode       = default(BarsHelperColorMode);
            RequestedTheme  = ElementTheme.Default;
        }
        
        public async Task InitializeForCurrentViewAsync()
        {
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

            await ViewHelper.RunOnCurrentViewDispatcherAsync(() => InitializeColorModeForCurrentView(true, ColorMode));
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

        private void InitializeColorModeForCurrentView(bool initialize, BarsHelperColorMode colorMode)
        {
            switch (colorMode)
            {
                case BarsHelperColorMode.Themed:
                    if (isStatusBarTypePresent)
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
                        AccentColorHelper.GetForCurrentView().AccentColorChanged += AccentColorHelper_AccentColorChanged;
                    }
                    else
                    {
                        AccentColorHelper.GetForCurrentView().AccentColorChanged -= AccentColorHelper_AccentColorChanged;
                    }

                    break;
            }

        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            TrySetStatusBarColorsAsync();
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
                    colorsSetter.SetTitleBarColors(titleBar, requestedTheme);
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
                        colorsSetter.SetStatusBarColors(statusBar, requestedTheme, useDarkerStatusBarOnLandscapeOrientation, DisplayInformation.GetForCurrentView().CurrentOrientation);
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
