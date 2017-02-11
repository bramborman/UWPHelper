using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public sealed class BarsHelperColorsSetterDefault : IBarsHelperColorsSetter
    {
        public static BarsHelperColorsSetterDefault Current { get; }

        static BarsHelperColorsSetterDefault()
        {
            Current = new BarsHelperColorsSetterDefault();
        }

        // Prevent from creating new instances
        private BarsHelperColorsSetterDefault()
        {

        }

        public void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme)
        {
            switch (requestedTheme)
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

        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDarkerStatusBarOnLandscapeOrientation, DisplayOrientations currentOrientation)
        {
            statusBar.BackgroundOpacity = 1;
            
            ElementTheme applicationRequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
            bool lightTheme = (requestedTheme == ElementTheme.Default ? applicationRequestedTheme : requestedTheme) == ElementTheme.Light;
                
            if (useDarkerStatusBarOnLandscapeOrientation && (currentOrientation == DisplayOrientations.Landscape || currentOrientation == DisplayOrientations.LandscapeFlipped))
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
