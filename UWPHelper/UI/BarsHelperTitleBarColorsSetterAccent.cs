using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperTitleBarColorsSetterAccent : IBarsHelperTitleBarColorsSetter
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
    }
}
