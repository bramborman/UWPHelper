using UWPHelper.Utilities;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperColorsSetterAccent : IBarsHelperColorsSetter
    {
        private bool UseLightOverlay
        {
            get
            {
                return AccentColorHelper.GetForCurrentView().AccentContrastingTheme == ElementTheme.Dark;
            }
        }
        
        public void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme)
        {
            titleBar.BackgroundColor       = AccentColorHelper.GetForCurrentView().AccentColor;
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;

            if (UseLightOverlay)
            {
                titleBar.ForegroundColor = Colors.White;

                titleBar.ButtonHoverBackgroundColor   = titleBar.ButtonBackgroundColor.Value.Mix(Color.FromArgb(0x19, 0xFF, 0xFF, 0xFF));
                titleBar.ButtonPressedBackgroundColor = titleBar.ButtonBackgroundColor.Value.Mix(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF));
            }
            else
            {
                titleBar.ForegroundColor = Colors.Black;

                titleBar.ButtonHoverBackgroundColor   = titleBar.ButtonBackgroundColor.Value.Mix(Color.FromArgb(0x19, 0x00, 0x00, 0x00));
                titleBar.ButtonPressedBackgroundColor = titleBar.ButtonBackgroundColor.Value.Mix(Color.FromArgb(0x33, 0x00, 0x00, 0x00));
            }

            // Don't change foreground colors when buttons are in different states
            titleBar.ButtonForegroundColor          = titleBar.ForegroundColor;
            titleBar.ButtonHoverForegroundColor     = titleBar.ForegroundColor;
            titleBar.ButtonPressedForegroundColor   = titleBar.ForegroundColor;

            switch (requestedTheme)
            {
                case ElementTheme.Default:
                    titleBar.InactiveBackgroundColor = null;
                    titleBar.InactiveForegroundColor = null;
                    break;
                
                case ElementTheme.Light:
                    titleBar.InactiveBackgroundColor = Colors.White;
                    titleBar.InactiveForegroundColor = Color.FromArgb(0xFF, 0x99, 0x99, 0x99);
                    break;
                
                case ElementTheme.Dark:
                    titleBar.InactiveBackgroundColor = Colors.Black;
                    titleBar.InactiveForegroundColor = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);
                    break;
            }

            titleBar.ButtonInactiveBackgroundColor = titleBar.InactiveBackgroundColor;
            titleBar.ButtonInactiveForegroundColor = titleBar.InactiveForegroundColor;
        }

        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDarkerStatusBarOnLandscapeOrientation, DisplayOrientations currentOrientation)
        {
            statusBar.BackgroundOpacity = 1.0;

            statusBar.BackgroundColor   = AccentColorHelper.GetForCurrentView().AccentColor;
            statusBar.ForegroundColor   = UseLightOverlay ? Colors.White : Colors.Black;
        }
    }
}
