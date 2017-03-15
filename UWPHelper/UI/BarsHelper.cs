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
    public sealed class BarsHelper
    {
        private static readonly bool isStatusBarTypePresent = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

        public static BarsHelper Current { get; private set; }

        // ApplicationTheme is used only when the selected color mode is ThemedGray
        private readonly Dictionary<int, ViewOneEventHandlerHelpers> viewInfo       = new Dictionary<int, ViewOneEventHandlerHelpers>();
        private readonly BarInfo<IBarsHelperTitleBarColorsSetter> titleBarInfo      = new BarInfo<IBarsHelperTitleBarColorsSetter>();
        private readonly BarInfo<IBarsHelperStatusBarColorsSetter> statusBarInfo    = new BarInfo<IBarsHelperStatusBarColorsSetter>();

        private ApplicationTheme? lastApplicationTheme;
        private Func<ElementTheme> _requestedThemeGetter;
        private INotifyPropertyChanged _requestedThemePropertyParent;

        private Color ColorModeThemedLightThemeBackgroundColor
        {
            get { return Colors.White; }
        }
        private Color ColorModeThemedDarkThemeBackgroundColor
        {
            get { return Colors.Black; }
        }
        // SystemChromeMediumColor
        private Color ColorModeThemedGrayLightThemeBackgroundColor
        {
            get
            {
                return Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6);
            }
        }
        private Color ColorModeThemedGrayDarkThemeBackgroundColor
        {
            get
            {
                return Color.FromArgb(0xFF, 0x1F, 0x1F, 0x1F);
            }
        }

        public bool IsInitialized
        {
            get
            {
                return viewInfo.Count > 0;
            }
        }
        public bool UseDifferentStatusBarColorsOnLandscapeOrientation { get; private set; }
        public BarsHelperColorMode TitleBarColorMode
        {
            get { return titleBarInfo.ColorMode; }
        }
        public BarsHelperColorMode StatusBarColorMode
        {
            get { return statusBarInfo.ColorMode; }
        }
        public IBarsHelperTitleBarColorsSetter TitleBarColorsSetter
        {
            get { return titleBarInfo.ColorsSetter; }
        }
        public IBarsHelperStatusBarColorsSetter StatusBarColorsSetter
        {
            get { return statusBarInfo.ColorsSetter; }
        }
        public Func<ElementTheme> RequestedThemeGetter
        {
            get { return _requestedThemeGetter; }
            set
            {
                ExceptionHelper.ValidateNotNull(value, nameof(RequestedThemeGetter));

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

                    // Not calling the TrySetBarsColorsAsync method since this hasn't got any direct impact on bars colors
                }
            }
        }
        public string RequestedThemePropertyName { get; private set; }

        static BarsHelper()
        {
            Current = new BarsHelper();
        }

        // Prevent from creating new instances
        private BarsHelper()
        {

        }

        public async Task SetUseDifferentStatusBarColorsOnLandscapeOrientationAsync(bool value)
        {
            if (UseDifferentStatusBarColorsOnLandscapeOrientation != value)
            {
                UseDifferentStatusBarColorsOnLandscapeOrientation = value;

                if (isStatusBarTypePresent)
                {
                    // Cache current value to prevent from changing the value while setting the colors
                    bool cachedValue = UseDifferentStatusBarColorsOnLandscapeOrientation;
                    await RunOnEachInitializedViewDispatcherAsync(() => InitializeUseDifferentStatusBarColorsOnLandscapeOrientationForCurrentView(cachedValue));

                    TrySetStatusBarColorsAsync();
                }
            }
        }

        public Task SetTitleBarColorModeAsync(BarsHelperColorMode newValue)
        {
            return SetColorModeAsync(titleBarInfo, newValue, nameof(newValue), false, () =>
            {
                if (newValue == BarsHelperColorMode.Accent)
                {
                    if (StatusBarColorsSetter is BarsHelperColorsSetterAccent barsHelperColorsSetterAccent)
                    {
                        titleBarInfo.ColorsSetter = barsHelperColorsSetterAccent;
                    }
                    else
                    {
                        titleBarInfo.ColorsSetter = new BarsHelperColorsSetterAccent();
                    }
                }
                else
                {
                    Color lightThemeBackgroundColor;
                    Color lightThemeForegroundColor;
                    Color darkThemeBackgroundColor;
                    Color darkThemeForegroundColor;

                    if (newValue == BarsHelperColorMode.Themed)
                    {
                        lightThemeBackgroundColor = ColorModeThemedLightThemeBackgroundColor;
                        lightThemeForegroundColor = Colors.Black;
                        darkThemeBackgroundColor  = ColorModeThemedDarkThemeBackgroundColor;
                        darkThemeForegroundColor  = Colors.White;
                    }
                    else
                    {
                        lightThemeBackgroundColor = ColorModeThemedGrayLightThemeBackgroundColor;
                        lightThemeForegroundColor = Colors.Black;
                        darkThemeBackgroundColor  = ColorModeThemedGrayDarkThemeBackgroundColor;
                        darkThemeForegroundColor  = Colors.White;
                    }

                    bool isThemedGrayColorMode = newValue == BarsHelperColorMode.ThemedGray;
                    titleBarInfo.ColorsSetter = new BarsHelperTitleBarColorsSetter(isThemedGrayColorMode,
                                                                                   isThemedGrayColorMode ? null : new BarsHelperColorsSetterColorInfo(null, null),
                                                                                   new BarsHelperColorsSetterColorInfo(lightThemeBackgroundColor, lightThemeForegroundColor, lightThemeBackgroundColor, BarsHelperColorsSetterHelper.GetTitleBarInactiveForegroundColor(lightThemeForegroundColor, ElementTheme.Light)),
                                                                                   new BarsHelperColorsSetterColorInfo(darkThemeBackgroundColor, darkThemeForegroundColor, darkThemeBackgroundColor, BarsHelperColorsSetterHelper.GetTitleBarInactiveForegroundColor(darkThemeForegroundColor, ElementTheme.Dark)));
                }
            });
        }

        public Task SetStatusBarColorModeAsync(BarsHelperColorMode newValue)
        {
            return SetColorModeAsync(statusBarInfo, newValue, nameof(newValue), false, () =>
            {
                if (newValue == BarsHelperColorMode.Accent)
                {
                    if (TitleBarColorsSetter is BarsHelperColorsSetterAccent barsHelperColorsSetterAccent)
                    {
                        statusBarInfo.ColorsSetter = barsHelperColorsSetterAccent;
                    }
                    else
                    {
                        statusBarInfo.ColorsSetter = new BarsHelperColorsSetterAccent();
                    }
                }
                else
                {
                    Color lightThemeBackgroundColor;
                    Color darkThemeBackgroundColor;

                    if (newValue == BarsHelperColorMode.Themed)
                    {
                        lightThemeBackgroundColor = ColorModeThemedLightThemeBackgroundColor;
                        darkThemeBackgroundColor  = ColorModeThemedDarkThemeBackgroundColor;
                    }
                    else
                    {
                        lightThemeBackgroundColor = ColorModeThemedGrayLightThemeBackgroundColor;
                        darkThemeBackgroundColor  = ColorModeThemedGrayDarkThemeBackgroundColor;
                    }

                    Color lightThemeForegroundColor = BarsHelperColorsSetterHelper.GetStatusBarForegroundColor(lightThemeBackgroundColor, ElementTheme.Light);
                    Color darkThemeForegroundColor  = BarsHelperColorsSetterHelper.GetStatusBarForegroundColor(darkThemeBackgroundColor, ElementTheme.Dark);

                    // SystemChromeLowColor
                    Color lightThemeLandscapeBackgroundColor = Color.FromArgb(0xFF, 0xF2, 0xF2, 0xF2);
                    Color darkThemeLandscapeBackgroundColor  = Color.FromArgb(0xFF, 0x17, 0x17, 0x17);

                    // We need to calculate the theme to support different colors on landscape
                    statusBarInfo.ColorsSetter = new BarsHelperStatusBarColorsSetter(1.0, true,
                                                                                     null,
                                                                                     new BarsHelperColorsSetterColorInfo(lightThemeBackgroundColor, lightThemeForegroundColor),
                                                                                     new BarsHelperColorsSetterColorInfo(darkThemeBackgroundColor, darkThemeForegroundColor),
                                                                                     null,
                                                                                     new BarsHelperColorsSetterColorInfo(lightThemeLandscapeBackgroundColor, lightThemeForegroundColor),
                                                                                     new BarsHelperColorsSetterColorInfo(darkThemeLandscapeBackgroundColor, darkThemeForegroundColor));
                }
            });
        }
        
        private async Task SetColorModeAsync<T>(BarInfo<T> barInfo, BarsHelperColorMode newValue, string parameterName, bool enableBarsHelperColorModeCustom, Action switchColorsSetter)
        {
            bool isTitleBarColorMode = GetIsTitleBarInfo(barInfo);
            ExceptionHelper.ValidateEnumValueDefined(newValue, parameterName);

            if (!enableBarsHelperColorModeCustom && newValue == BarsHelperColorMode.Custom)
            {
                throw new ArgumentException($"Setting {(isTitleBarColorMode ? nameof(TitleBarColorMode) : nameof(StatusBarColorMode))} to {nameof(BarsHelperColorMode)}.{nameof(BarsHelperColorMode.Custom)} is not permitted. " +
                                            $"Use the {(isTitleBarColorMode ? nameof(SetCustomTitleBarColorsSetterAsync) : nameof(SetCustomStatusBarColorsSetterAsync))} method to specify custom {(isTitleBarColorMode ? nameof(TitleBarColorsSetter) : nameof(StatusBarColorsSetter))}.");
            }
            
            if (barInfo.ColorMode != newValue || !barInfo.IsColorModeSet)
            {
                barInfo.IsColorModeSet = true;

                bool doStuff = isTitleBarColorMode || isStatusBarTypePresent;

                if (doStuff)
                {
                    BarsHelperColorMode cachedValue = barInfo.ColorMode;
                    await RunOnEachInitializedViewDispatcherAsync(() => InitializeColorModeForCurrentView(isTitleBarColorMode, false, cachedValue));
                }

                barInfo.ColorMode = newValue;
                
                if (doStuff)
                {
                    // Will be null when called from the SetCustomColorsSetterAsync method
                    switchColorsSetter?.Invoke();

                    BarsHelperColorMode cachedValue = barInfo.ColorMode;
                    await RunOnEachInitializedViewDispatcherAsync(() => InitializeColorModeForCurrentView(isTitleBarColorMode, true, cachedValue));
                }

                if (isTitleBarColorMode)
                {
                    TrySetTitleBarColorsAsync();
                }
                else if (isStatusBarTypePresent)
                {
                    TrySetStatusBarColorsAsync();
                }
            }
        }

        public Task SetCustomTitleBarColorsSetterAsync(IBarsHelperTitleBarColorsSetter customColorsSetter)
        {
            return SetCustomColorsSetterAsync(titleBarInfo, customColorsSetter, nameof(customColorsSetter));
        }

        public Task SetCustomStatusBarColorsSetterAsync(IBarsHelperStatusBarColorsSetter customColorsSetter)
        {
            return SetCustomColorsSetterAsync(statusBarInfo, customColorsSetter, nameof(customColorsSetter));
        }

        private Task SetCustomColorsSetterAsync<T>(BarInfo<T> barInfo, T customColorsSetter, string parameterName)
        {
            // Do not use typeof(T) since it will return type of IBarsHelperTitleBarColorsSetter or IBarsHelperStatusBarColorsSetter
            if (customColorsSetter.GetType() == typeof(BarsHelperColorsSetterAccent))
            {
                throw new ArgumentException($"Use this method only to specify custom {(GetIsTitleBarInfo(barInfo) ? nameof(TitleBarColorsSetter) : nameof(StatusBarColorsSetter))}.", parameterName);
            }

            barInfo.ColorsSetter = customColorsSetter;
            return SetColorModeAsync(barInfo, BarsHelperColorMode.Custom, null, true, null);
        }

        private bool GetIsTitleBarInfo<T>(BarInfo<T> barInfo)
        {
            return ReferenceEquals(barInfo, titleBarInfo);
        }

        public async Task InitializeForCurrentViewAsync()
        {
            if (RequestedThemeGetter == null)
            {
                RequestedThemeGetter = () => ElementTheme.Default;
            }

            if (!titleBarInfo.IsColorModeSet)
            {
                await SetTitleBarColorModeAsync(default(BarsHelperColorMode));
            }

            if (!statusBarInfo.IsColorModeSet)
            {
                await SetStatusBarColorModeAsync(default(BarsHelperColorMode));
            }

            int currentViewId = ViewHelper.GetCurrentViewId();

            if (viewInfo.ContainsKey(currentViewId))
            {
                throw new InvalidOperationException($"{nameof(BarsHelper)} is already initialized for this view.");
            }

            viewInfo.Add(currentViewId, new ViewOneEventHandlerHelpers());
            TrySetBarsColorsAsync();
            
            if (isStatusBarTypePresent && UseDifferentStatusBarColorsOnLandscapeOrientation)
            {
                InitializeUseDifferentStatusBarColorsOnLandscapeOrientationForCurrentView(true);
            }

            InitializeColorModeForCurrentView(true, true, TitleBarColorMode);

            if (isStatusBarTypePresent)
            {
                InitializeColorModeForCurrentView(false, true, StatusBarColorMode);
            }
        }

        public void InitializeAutoUpdating(Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged requestedThemePropertyParent, string requestedThemePropertyName)
        {
            ExceptionHelper.ValidateNotNull(requestedThemePropertyParent, nameof(requestedThemePropertyParent));
            ExceptionHelper.ValidateNotNullOrWhiteSpace(requestedThemePropertyName, nameof(requestedThemePropertyName));

            RequestedThemeGetter            = requestedThemeGetter;
            // Assign this before attaching an event handler to parent
            RequestedThemePropertyName      = requestedThemePropertyName;
            RequestedThemePropertyParent    = requestedThemePropertyParent;
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

        private void InitializeUseDifferentStatusBarColorsOnLandscapeOrientationForCurrentView(bool initialize)
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
            ViewOneEventHandlerHelpers currentViewOneEventHandlerHelpers = viewInfo[ViewHelper.GetCurrentViewId()];

            switch (colorMode)
            {
                // We need to change the colors when system theme changes because we are calculating ElementTheme from ApplicationTheme
                case BarsHelperColorMode.Themed:
                case BarsHelperColorMode.ThemedGray:
                    if (initialize)
                    {
                        currentViewOneEventHandlerHelpers.WindowActivatedEventHandlerHelper.AddHandler(() =>
                        {
                            Window.Current.Activated += Window_Activated;
                        });
                    }
                    else
                    {
                        currentViewOneEventHandlerHelpers.WindowActivatedEventHandlerHelper.RemoveHandler(() =>
                        {
                            Window.Current.Activated -= Window_Activated;
                        });
                    }

                    break;
                
                case BarsHelperColorMode.Accent:
                    if (initialize)
                    {
                        currentViewOneEventHandlerHelpers.AccentColorHelperColorChangedEventHandlerHelper.AddHandler(() =>
                        {
                            AccentColorHelper.GetForCurrentView().AccentColorChanged += AccentColorHelper_AccentColorChanged;
                        });
                    }
                    else
                    {
                        currentViewOneEventHandlerHelpers.AccentColorHelperColorChangedEventHandlerHelper.RemoveHandler(() =>
                        {
                            AccentColorHelper.GetForCurrentView().AccentColorChanged -= AccentColorHelper_AccentColorChanged;
                        });
                    }

                    break;
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated && lastApplicationTheme != Application.Current.RequestedTheme)
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
            AssignLastApplicationTheme();

            // Cache the value to prevent unnecessary calls and prevent from changing the value while setting the colors
            ElementTheme requestedTheme = RequestedThemeGetter();

            return RunOnEachInitializedViewDispatcherAsync(() =>
            {
                if (ApplicationView.GetForCurrentView().TitleBar is ApplicationViewTitleBar titleBar)
                {
                    TitleBarColorsSetter.SetTitleBarColors(titleBar, requestedTheme);
                }
            });
        }

        public Task SetStatusBarColorsAsync()
        {
            ValidateInitialization();

            if (isStatusBarTypePresent)
            {
                AssignLastApplicationTheme();

                // Cache the values to prevent unnecessary calls and prevent from changing the values while setting the colors
                bool useDifferentStatusBarColorsOnLandscapeOrientation = UseDifferentStatusBarColorsOnLandscapeOrientation;
                ElementTheme requestedTheme = RequestedThemeGetter();

                return RunOnEachInitializedViewDispatcherAsync(() =>
                {
                    if (StatusBar.GetForCurrentView() is StatusBar statusBar)
                    {
                        StatusBarColorsSetter.SetStatusBarColors(statusBar, requestedTheme, useDifferentStatusBarColorsOnLandscapeOrientation, DisplayInformation.GetForCurrentView().CurrentOrientation);
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

        private void AssignLastApplicationTheme()
        {
            lastApplicationTheme = Application.Current.RequestedTheme;
        }

        private Task RunOnEachInitializedViewDispatcherAsync(Action action)
        {
            return ViewHelper.RunOnEachViewDispatcherAsync(() =>
            {
                if (viewInfo.ContainsKey(ViewHelper.GetCurrentViewId()))
                {
                    action();
                }
            });
        }

        private sealed class BarInfo<T>
        {
            internal BarsHelperColorMode ColorMode { get; set; }
            internal bool IsColorModeSet { get; set; }
            internal T ColorsSetter { get; set; }
        }

        private sealed class ViewOneEventHandlerHelpers
        {
            private OneEventHandlerHelper _windowActivatedEventHandlerHelper;
            private OneEventHandlerHelper _accentColorHelperColorChangedEventHandlerHelper;

            internal OneEventHandlerHelper WindowActivatedEventHandlerHelper
            {
                get
                {
                    if (_windowActivatedEventHandlerHelper == null)
                    {
#if DEBUG
                        _windowActivatedEventHandlerHelper = new OneEventHandlerHelper("Window");
#else
                    _windowActivatedEventHandlerHelper = new OneEventHandlerHelper();
#endif
                    }

                    return _windowActivatedEventHandlerHelper;
                }
            }
            internal OneEventHandlerHelper AccentColorHelperColorChangedEventHandlerHelper
            {
                get
                {
                    if (_accentColorHelperColorChangedEventHandlerHelper == null)
                    {
#if DEBUG
                        _accentColorHelperColorChangedEventHandlerHelper = new OneEventHandlerHelper("Accent");
#else
                    _accentColorHelperColorChangedEventHandlerHelper = new OneEventHandlerHelper();
#endif
                    }

                    return _accentColorHelperColorChangedEventHandlerHelper;
                }
            }

            internal ViewOneEventHandlerHelpers()
            {

            }
        }
    }
}
