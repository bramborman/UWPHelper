using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public sealed class BarsHelperColorsSetterAccent : IBarsHelperColorsSetter
    {
        public static BarsHelperColorsSetterAccent Current { get; }
        
        private readonly Color[] lightOverlayColors = { Color.FromArgb(0x19, 0xFF, 0xFF, 0xFF), Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF) };
        private readonly Color[] darkOverlayColors  = { Color.FromArgb(0x19, 0x00, 0x00, 0x00), Color.FromArgb(0x33, 0x00, 0x00, 0x00) };

        private bool UseLightOverlay
        {
            get
            {
                return AccentColorHelper.CurrentInternal.AccentContrastingTheme == ElementTheme.Dark;
            }
        }

        static BarsHelperColorsSetterAccent()
        {
            Current = new BarsHelperColorsSetterAccent();
        }

        // Prevent from creating new instances
        private BarsHelperColorsSetterAccent()
        {

        }

        public void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme)
        {
            titleBar.BackgroundColor       = AccentColorHelper.CurrentInternal.AccentColor;
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;

            if (UseLightOverlay)
            {
                titleBar.ForegroundColor = Colors.White;

                titleBar.ButtonHoverBackgroundColor   = titleBar.ButtonBackgroundColor.Value.Mix(lightOverlayColors[0]);
                titleBar.ButtonPressedBackgroundColor = titleBar.ButtonBackgroundColor.Value.Mix(lightOverlayColors[1]);
            }
            else
            {
                titleBar.ForegroundColor = Colors.Black;

                titleBar.ButtonHoverBackgroundColor   = titleBar.ButtonBackgroundColor.Value.Mix(darkOverlayColors[0]);
                titleBar.ButtonPressedBackgroundColor = titleBar.ButtonBackgroundColor.Value.Mix(darkOverlayColors[1]);
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

            statusBar.BackgroundColor   = AccentColorHelper.CurrentInternal.AccentColor;
            statusBar.ForegroundColor   = UseLightOverlay ? Colors.White : Colors.Black;
        }
    }
}
