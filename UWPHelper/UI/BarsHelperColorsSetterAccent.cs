using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperColorsSetterAccent : IBarsHelperTitleBarColorsSetter, IBarsHelperStatusBarColorsSetter
    {
        private bool UseLightOverlay
        {
            get
            {
                // Seems weird, right? :D
                // When AccentContrastingTheme is ElementTheme.Dark, then the accent color is dark so contrasting foreground is white
                // I've finally understood why I've done it this way xD
                return AccentColorHelper.GetForCurrentView().AccentContrastingTheme == ElementTheme.Dark;
            }
        }

        public void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme)
        {
            titleBar.BackgroundColor       = AccentColorHelper.GetForCurrentView().AccentColor;
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;

            ElementTheme overlayTheme               = AccentColorHelper.GetForCurrentView().AccentContrastingTheme;
            titleBar.ButtonHoverBackgroundColor     = BarsHelperColorsSetterHelper.GetTitleBarButtonHoverBackgroundColor(titleBar.BackgroundColor.Value, overlayTheme);
            titleBar.ButtonPressedBackgroundColor   = BarsHelperColorsSetterHelper.GetTitleBarButtonPressedBackgroundColor(titleBar.BackgroundColor.Value, overlayTheme);

            titleBar.ForegroundColor                = UseLightOverlay ? Colors.White : Colors.Black;
            // Do not change foreground colors when buttons are in different states
            titleBar.ButtonForegroundColor          = titleBar.ForegroundColor;
            titleBar.ButtonHoverForegroundColor     = titleBar.ForegroundColor;
            titleBar.ButtonPressedForegroundColor   = titleBar.ForegroundColor;

            Color inactiveForegroundColorBase;

            switch (requestedTheme)
            {
                case ElementTheme.Default:
                    titleBar.InactiveBackgroundColor    = null;
                    break;
                
                case ElementTheme.Light:
                    titleBar.InactiveBackgroundColor    = Colors.White;
                    inactiveForegroundColorBase         = Colors.Black;
                    break;
                
                case ElementTheme.Dark:
                    titleBar.InactiveBackgroundColor    = Colors.Black;
                    inactiveForegroundColorBase         = Colors.White;
                    break;
            }

            if (requestedTheme == ElementTheme.Default)
            {
                titleBar.InactiveForegroundColor = null;
            }
            else
            {
                titleBar.InactiveForegroundColor = BarsHelperColorsSetterHelper.GetTitleBarInactiveForegroundColor(inactiveForegroundColorBase, requestedTheme);
            }

            titleBar.ButtonInactiveBackgroundColor = titleBar.InactiveBackgroundColor;
            titleBar.ButtonInactiveForegroundColor = titleBar.InactiveForegroundColor;
        }

        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDifferentStatusBarColorsOnLandscapeOrientation, DisplayOrientations currentOrientation)
        {
            statusBar.BackgroundOpacity = 1.0;

            statusBar.BackgroundColor = AccentColorHelper.GetForCurrentView().AccentColor;
            statusBar.ForegroundColor = UseLightOverlay ? Colors.White : Colors.Black;
        }
    }
}
