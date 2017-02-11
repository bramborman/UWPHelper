using System;
using System.ComponentModel;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public sealed partial class BarsHelper
    {
        private static readonly bool isApplicationViewTypePresent = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView");
        private static readonly bool isStatusBarTypePresent       = ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

        public static BarsHelper Current { get; private set; }

        private Func<ElementTheme> requestedThemeGetter;
        private INotifyPropertyChanged themePropertyParent;
        private string themePropertyName;

        private bool _useDarkerStatusBarOnLandscapeOrientation;
        private BarsHelperColorMode? _colorMode;

        public bool IsInitialized { get; private set; }
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
                    _colorMode = value;

                    if (!IsUsingCustomColorsSetter || ColorsSetter == null)
                    {
                        AccentColorHelper.CurrentInternal.IsActive = _colorMode == BarsHelperColorMode.Accent;

                        switch (_colorMode)
                        {
                            case BarsHelperColorMode.Default:
                                ColorsSetter = BarsHelperColorsSetterDefault.Current;
                                break;

                            case BarsHelperColorMode.Accent:
                                ColorsSetter = BarsHelperColorsSetterAccent.Current;
                                break;
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
            IsInitialized                               = false;
            UseDarkerStatusBarOnLandscapeOrientation    = false;
        }
        
        public void Initialize(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            Initialize(false, colorMode, null, requestedThemeGetter, null, null);
        }

        public void Initialize(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);

            Initialize(false, colorMode, colorsSetter, requestedThemeGetter, null, null);
        }

        public void Initialize(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            Initialize(false, colorMode, null, requestedThemeGetter, themePropertyParent, themePropertyName);
        }

        public void Initialize(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            Initialize(false, colorMode, colorsSetter, requestedThemeGetter, themePropertyParent, themePropertyName);
        }
        
        public void Reinitialize(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            Initialize(true, colorMode, null, requestedThemeGetter, null, null);
        }

        public void Reinitialize(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);

            Initialize(true, colorMode, colorsSetter, requestedThemeGetter, null, null);
        }

        public void Reinitialize(BarsHelperColorMode colorMode, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            Initialize(true, colorMode, null, requestedThemeGetter, themePropertyParent, themePropertyName);
        }

        public void Reinitialize(BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            ValidateColorsSetter(colorsSetter);
            ValidateRequestedThemeGetter(requestedThemeGetter);
            ValidateThemePropertyParent(themePropertyParent);
            ValidateThemePropertyName(themePropertyName);

            Initialize(true, colorMode, colorsSetter, requestedThemeGetter, themePropertyParent, themePropertyName);
        }
        
        private void Initialize(bool reinitialization, BarsHelperColorMode colorMode, IBarsHelperColorsSetter colorsSetter, Func<ElementTheme> requestedThemeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            if (!reinitialization && IsInitialized)
            {
                throw new InvalidOperationException("Object is already initialized.");
            }
            
            IsInitialized = true;
            
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
                throw new InvalidOperationException($"{nameof(BarsHelper)} is not initialized. Call the {nameof(Initialize)} method first.");
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

            if (isApplicationViewTypePresent && ApplicationView.GetForCurrentView().TitleBar is ApplicationViewTitleBar titleBar)
            {
                ColorsSetter.SetTitleBarColors(titleBar, requestedThemeGetter());
            }
        }

        public void SetStatusBarColors()
        {
            SetStatusBarColors(DisplayInformation.GetForCurrentView().CurrentOrientation);
        }
        
        private void SetStatusBarColors(DisplayOrientations currentOrientation)
        {
            ValidateInitialization();

            if (isStatusBarTypePresent && StatusBar.GetForCurrentView() is StatusBar statusBar)
            {
                ColorsSetter.SetStatusBarColors(statusBar, requestedThemeGetter(), UseDarkerStatusBarOnLandscapeOrientation, currentOrientation);
            }
        }
    }
}
