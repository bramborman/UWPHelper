using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed partial class BarsHelper
    {
        private static readonly bool isApplicationViewTypePresent = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView");
        private static readonly bool isStatusBarTypePresent       = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

        public static BarsHelper Current { get; private set; }

        private readonly List<BarsReferenceHolder> barsReferenceHolders = new List<BarsReferenceHolder>();

        private Func<ElementTheme> requestedThemeGetter;
        private INotifyPropertyChanged themePropertyParent;
        private string themePropertyName;

        private bool _useDarkerStatusBarOnLandscapeOrientation;
        private BarsHelperColorMode? _colorMode;

        public bool IsInitialized
        {
            get
            {
                return barsReferenceHolders.Count > 0;
            }
        }
        public bool IsAutomaticallyUpdated
        {
            get
            {
                return themePropertyParent != null || ColorMode == BarsHelperColorMode.Accent;
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
                        //TODO: Use WeakEventListener
                        if (_useDarkerStatusBarOnLandscapeOrientation)
                        {
                            DisplayInformation.GetForCurrentView().OrientationChanged += DisplayInformation_OrientationChanged;
                        }
                        else
                        {
                            DisplayInformation.GetForCurrentView().OrientationChanged -= DisplayInformation_OrientationChanged;
                        }
                    }

                    if (IsInitialized)
                    {
                        SetStatusBarColors();
                    }
                }
            }
        }
        public BarsHelperColorMode ColorMode
        {
            get { return _colorMode.Value; }
            set
            {
                if (_colorMode != value || _colorMode == null)
                {
                    if (!IsUsingCustomColorsSetter && _colorMode == BarsHelperColorMode.Accent)
                    {
                        AccentColorHelper.CurrentInternal.IsActive = false;
                        AccentColorHelper.CurrentInternal.PropertyChanged -= AccentColorHelper_PropertyChanged;
                    }

                    _colorMode = value;

                    if (!IsUsingCustomColorsSetter || ColorsSetter == null)
                    {
                        if (_colorMode == BarsHelperColorMode.Themed)
                        {
                            ColorsSetter = BarsHelperColorsSetterDefault.Current;
                        }
                        else
                        {
                            AccentColorHelper.CurrentInternal.IsActive = true;
                            ColorsSetter = BarsHelperColorsSetterAccent.Current;

                            AccentColorHelper.CurrentInternal.PropertyChanged += AccentColorHelper_PropertyChanged;
                        }
                    }

                    if (IsAutomaticallyUpdated)
                    {
                        SetBarsColors();
                    }
                }
            }
        }
        public bool IsUsingCustomColorsSetter
        {
            get
            {
                return !(ColorsSetter is BarsHelperColorsSetterDefault || ColorsSetter is BarsHelperColorsSetterAccent);
            }
        }
        public IBarsHelperColorsSetter ColorsSetter { get; private set; }

        static BarsHelper()
        {
            Current = new BarsHelper();
        }

        // Prevent from creating new instances
        private BarsHelper()
        {

        }
        
        public void InitializeForCurrentView(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            InitializeForCurrentView(false, colorMode, null, requestedThemeGetter, null, null);
        }

        public void InitializeForCurrentView(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);

            InitializeForCurrentView(false, colorMode, colorsSetter, requestedThemeGetter, null, null);
        }

        public void InitializeForCurrentView(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            InitializeForCurrentView(false, colorMode, null, requestedThemeGetter, themePropertyParent, themePropertyName);
        }

        public void InitializeForCurrentView(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            InitializeForCurrentView(false, colorMode, colorsSetter, requestedThemeGetter, themePropertyParent, themePropertyName);
        }

        public void InitializeForCurrentAdditionalView()
        {
            InitializeForCurrentView(false);
        }

        public void ReinitializeForCurrentView(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            InitializeForCurrentView(true, colorMode, null, requestedThemeGetter, null, null);
        }

        public void ReinitializeForCurrentView(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);

            InitializeForCurrentView(true, colorMode, colorsSetter, requestedThemeGetter, null, null);
        }

        public void ReinitializeForCurrentView(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            InitializeForCurrentView(true, colorMode, null, requestedThemeGetter, themePropertyParent, themePropertyName);
        }

        public void ReinitializeForCurrentView(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            InitializeForCurrentView(true, colorMode, colorsSetter, requestedThemeGetter, themePropertyParent, themePropertyName);
        }

        public void ReinitializeForCurrentAdditionalView()
        {
            InitializeForCurrentView(true);
        }

        private void InitializeForCurrentView(bool reinitialization, BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            if (reinitialization && !IsInitialized)
            {
                throw new InvalidOperationException($"Cannot reinitialize - {nameof(BarsHelper)} must be initialized before.");
            }

            InitializeForCurrentView(reinitialization);
            
            ColorsSetter                = ColorsSetter;
            this.requestedThemeGetter   = requestedThemeGetter;
            
            if (this.themePropertyParent != null)
            {
                themePropertyParent.PropertyChanged -= ThemePropertyParent_PropertyChanged;

                if (isStatusBarTypePresent)
                {
                    Window.Current.VisibilityChanged -= Window_VisibilityChanged;
                }
            }

            this.themePropertyParent = themePropertyParent;

            if (this.themePropertyParent != null)
            {
                this.themePropertyParent.PropertyChanged += ThemePropertyParent_PropertyChanged;

                if (isStatusBarTypePresent)
                {
                    Window.Current.VisibilityChanged += Window_VisibilityChanged;
                }
            }

            this.themePropertyName = themePropertyName;
            ColorMode = colorMode;

            if (IsAutomaticallyUpdated)
            {
                SetBarsColors();
            }
        }

        private void InitializeForCurrentView(bool reinitialization)
        {
            BarsReferenceHolder barsReferenceHolder     = null;
            ApplicationViewTitleBar currentViewTitleBar = null;
            StatusBar currentViewStatusBar              = null;

            if (isApplicationViewTypePresent && ApplicationView.GetForCurrentView().TitleBar is ApplicationViewTitleBar currentTitleBar)
            {
                currentViewTitleBar = currentTitleBar;

                barsReferenceHolder = barsReferenceHolders.FirstOrDefault(b =>
                {
                    b.TitleBar.TryGetTarget(out ApplicationViewTitleBar titleBar);
                    return titleBar == currentTitleBar;
                });
            }

            if (isStatusBarTypePresent && StatusBar.GetForCurrentView() is StatusBar currentStatusBar)
            {
                currentViewStatusBar = currentStatusBar;

                if (barsReferenceHolder == null)
                {
                    barsReferenceHolder = barsReferenceHolders.FirstOrDefault(b =>
                    {
                        b.StatusBar.TryGetTarget(out StatusBar statusBar);
                        return statusBar == currentStatusBar;
                    });
                }
            }

            if (barsReferenceHolder == null)
            {
                barsReferenceHolders.Add(new BarsReferenceHolder(currentViewTitleBar, currentViewStatusBar));
            }
            else if (!reinitialization && barsReferenceHolder != null)
            {
                throw new InvalidOperationException($"{nameof(BarsHelper)} is already initialized for this view.");
            }
        }

        private void ThemePropertyParent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == themePropertyName)
            {
                SetBarsColors();
            }
        }

        private void Window_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (requestedThemeGetter() == ElementTheme.Default && e.Visible)
            {
                SetStatusBarColors();
            }
        }

        private void AccentColorHelper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccentColorHelper.AccentColor) || e.PropertyName == nameof(AccentColorHelper.AccentContrastingTheme))
            {
                SetBarsColors();
            }
        }

        private void ValidateColorsSetter(IBarsHelperColorsSetter colorsSetter)
        {
            if (colorsSetter == null)
            {
                throw new ArgumentNullException(nameof(colorsSetter));
            }
        }
        
        private void ValidateRequestedThemeGetter(Func<ElementTheme> requestedThemeGetter)
        {
            if (requestedThemeGetter == null)
            {
                throw new ArgumentNullException(nameof(requestedThemeGetter));
            }
        }

        private void ValidateThemePropertyParent(INotifyPropertyChanged themePropertyParent)
        {
            if (themePropertyParent == null)
            {
                throw new ArgumentNullException(nameof(themePropertyParent));
            }
        }

        private void ValidateThemePropertyName(string themePropertyName)
        {
            if (string.IsNullOrWhiteSpace(themePropertyName))
            {
                throw new ArgumentException("Value cannot be empty or null", nameof(themePropertyName));
            }
        }

        private void ValidateInitialization()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(BarsHelper)} is not initialized. Call the {nameof(InitializeForCurrentView)} method first.");
            }
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            SetStatusBarColors(sender.CurrentOrientation);
        }

        public void SetBarsColors()
        {
            SetTitleBarColors();
            SetStatusBarColors();
        }

        public void SetTitleBarColors()
        {
            ValidateInitialization();

            if (isApplicationViewTypePresent)
            {
                foreach (BarsReferenceHolder barsReferenceHolder in barsReferenceHolders)
                {
                    if (barsReferenceHolder.TitleBar.TryGetTarget(out ApplicationViewTitleBar titleBar))
                    {
                        ColorsSetter.SetTitleBarColors(titleBar, requestedThemeGetter());
                    }
                }
            }
        }

        public void SetStatusBarColors()
        {
            SetStatusBarColors(DisplayInformation.GetForCurrentView().CurrentOrientation);
        }
        
        private void SetStatusBarColors(DisplayOrientations currentOrientation)
        {
            ValidateInitialization();

            if (isStatusBarTypePresent)
            {
                foreach (BarsReferenceHolder barsReferenceHolder in barsReferenceHolders)
                {
                    if (barsReferenceHolder.StatusBar.TryGetTarget(out StatusBar statusBar))
                    {
                        ColorsSetter.SetStatusBarColors(statusBar, requestedThemeGetter(), UseDarkerStatusBarOnLandscapeOrientation, currentOrientation);
                    }
                }
            }
        }

        private sealed class BarsReferenceHolder
        {
            internal WeakReference<ApplicationViewTitleBar> TitleBar { get; }
            internal WeakReference<StatusBar> StatusBar { get; }

            internal BarsReferenceHolder(ApplicationViewTitleBar titleBar, StatusBar statusBar)
            {
                TitleBar            = new WeakReference<ApplicationViewTitleBar>(titleBar);
                StatusBar           = new WeakReference<StatusBar>(statusBar);
            }
        }
    }
}
