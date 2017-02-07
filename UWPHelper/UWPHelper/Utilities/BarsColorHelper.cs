using System;
using System.ComponentModel;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI;
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

        private Func<ElementTheme> themeGetter;
        private INotifyPropertyChanged themePropertyParent;
        private string themePropertyName;

        private bool _useDarkerStatusBarOnLandscapeOrientation;

        public bool IsInitialized { get; private set; }
        public bool IsAutomaticallyUpdated
        {
            get
            {
                return themePropertyParent != null;
            }
        }
        public bool UseDarkerStatusBarOnLandscapeOrientation
        {
            get { return _useDarkerStatusBarOnLandscapeOrientation; }
            private set
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
                }
            }
        }

        static BarsHelper()
        {
            Current = new BarsHelper();
        }

        private BarsHelper()
        {
            IsInitialized                               = false;
            UseDarkerStatusBarOnLandscapeOrientation    = false;
        }

        public void Initialize(ElementTheme requestedTheme)
        {
            Initialize(false, () => requestedTheme, null, null);
        }

        public void Initialize(Func<ElementTheme> themeGetter)
        {
            CheckThemeGetter(themeGetter);
            Initialize(false, themeGetter, null, null);
        }

        public void Initialize(Func<ElementTheme> themeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            CheckThemeGetter(themeGetter);
            CheckThemePropertyParent(themePropertyParent);
            CheckThemePropertyName(themePropertyName);

            Initialize(false, themeGetter, themePropertyParent, themePropertyName);
        }

        public void Reinitialize(ElementTheme requestedTheme)
        {
            Initialize(true, () => requestedTheme, null, null);
        }

        public void Reinitialize(Func<ElementTheme> themeGetter)
        {
            CheckThemeGetter(themeGetter);
            Initialize(true, themeGetter, null, null);
        }

        public void Reinitialize(Func<ElementTheme> themeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            CheckThemeGetter(themeGetter);
            CheckThemePropertyParent(themePropertyParent);
            CheckThemePropertyName(themePropertyName);

            Initialize(true, themeGetter, themePropertyParent, themePropertyName);
        }
        
        private void Initialize(bool reinitialization, Func<ElementTheme> themeGetter, INotifyPropertyChanged themePropertyParent, string themePropertyName)
        {
            if (!reinitialization && IsInitialized)
            {
                throw new InvalidOperationException("Object is already initialized.");
            }
            
            IsInitialized = true;

            this.themeGetter = themeGetter;
            
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
            if (themeGetter() == ElementTheme.Default && e.Visible)
            {
                SetStatusBarColors();
            }
        }

        private void CheckThemeGetter(Func<ElementTheme> themeGetter)
        {
            if (themeGetter == null)
            {
                throw new ArgumentNullException(nameof(themeGetter));
            }
        }

        private void CheckThemePropertyParent(INotifyPropertyChanged themePropertyParent)
        {
            if (themePropertyParent == null)
            {
                throw new ArgumentNullException(nameof(themePropertyParent));
            }
        }

        private void CheckThemePropertyName(string themePropertyName)
        {
            if (string.IsNullOrWhiteSpace(themePropertyName))
            {
                throw new ArgumentException("Value cannot be empty or null", nameof(themePropertyName));
            }
        }

        private void CheckIsInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"Object is not initialized. Call the {nameof(Initialize)} method first.");
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
            CheckIsInitialized();

            if (isApplicationViewTypePresent && ApplicationView.GetForCurrentView().TitleBar is ApplicationViewTitleBar titleBar)
            {
                switch (themeGetter())
                {
                    case ElementTheme.Default:
                        titleBar.BackgroundColor                = null;
                        titleBar.ForegroundColor                = null;
                        titleBar.InactiveForegroundColor        = null;

                        titleBar.ButtonHoverBackgroundColor     = null;
                        titleBar.ButtonPressedBackgroundColor   = null;

                        break;
                        
                    case ElementTheme.Light:
                        titleBar.BackgroundColor                = Colors.White;
                        titleBar.ForegroundColor                = Colors.Black;
                        titleBar.InactiveForegroundColor        = Color.FromArgb(0xFF, 0x99, 0x99, 0x99);

                        titleBar.ButtonHoverBackgroundColor     = Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6);
                        titleBar.ButtonPressedBackgroundColor   = Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC);

                        break;

                    case ElementTheme.Dark:
                        titleBar.BackgroundColor                = Colors.Black;
                        titleBar.ForegroundColor                = Colors.White;
                        titleBar.InactiveForegroundColor        = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);

                        titleBar.ButtonHoverBackgroundColor     = Color.FromArgb(0xFF, 0x19, 0x19, 0x19);
                        titleBar.ButtonPressedBackgroundColor   = Color.FromArgb(0xFF, 0x33, 0x33, 0x33);

                        break;
                }
            
                titleBar.InactiveBackgroundColor        = titleBar.BackgroundColor;

                titleBar.ButtonBackgroundColor          = titleBar.BackgroundColor;
                titleBar.ButtonForegroundColor          = titleBar.ForegroundColor;

                titleBar.ButtonHoverForegroundColor     = titleBar.ButtonForegroundColor;
                titleBar.ButtonInactiveBackgroundColor  = titleBar.InactiveBackgroundColor;
                titleBar.ButtonInactiveForegroundColor  = titleBar.InactiveForegroundColor;
                titleBar.ButtonPressedForegroundColor   = titleBar.ButtonForegroundColor;
            }
        }

        public void SetStatusBarColors()
        {
            SetStatusBarColors(DisplayInformation.GetForCurrentView().CurrentOrientation);
        }
        
        private void SetStatusBarColors(DisplayOrientations currentOrientation)
        {
            CheckIsInitialized();

            if (isStatusBarTypePresent && StatusBar.GetForCurrentView() is StatusBar statusBar)
            {
                statusBar.BackgroundOpacity = 1;

                ElementTheme requestedTheme = themeGetter();
                ElementTheme applicationRequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                bool lightTheme = (requestedTheme == ElementTheme.Default ? applicationRequestedTheme : requestedTheme) == ElementTheme.Light;
                
                if (UseDarkerStatusBarOnLandscapeOrientation && (currentOrientation == DisplayOrientations.Landscape || currentOrientation == DisplayOrientations.LandscapeFlipped))
                {
                    if (lightTheme)
                    {
                        statusBar.BackgroundColor = Color.FromArgb(0xFF, 0xF2, 0xF2, 0xF2);
                    }
                    else
                    {
                        statusBar.BackgroundColor = Color.FromArgb(0xFF, 0x17, 0x17, 0x17);
                    }
                }
                else
                {
                    if (lightTheme)
                    {
                        statusBar.BackgroundColor = Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6);
                    }
                    else
                    {
                        statusBar.BackgroundColor = Color.FromArgb(0xFF, 0x1F, 0x1F, 0x1F);
                    }
                }

                if (lightTheme)
                {
                    statusBar.ForegroundColor = Color.FromArgb(0xFF, 0x5C, 0x5C, 0x5C);
                }
                else
                {
                    statusBar.ForegroundColor = Color.FromArgb(0xFF, 0xC7, 0xC7, 0xC7);
                }
            }
        }
    }
}
