using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperTitleBarColorsSetterThemed : IBarsHelperTitleBarColorsSetter
    {
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
    }
}
