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
                    
                    break;
                        
                case ElementTheme.Light:
                    titleBar.BackgroundColor                = Colors.White;
                    titleBar.ForegroundColor                = Colors.Black;
                    titleBar.InactiveForegroundColor        = Color.FromArgb(0xFF, 0x99, 0x99, 0x99);
                    
                    break;

                case ElementTheme.Dark:
                    titleBar.BackgroundColor                = Colors.Black;
                    titleBar.ForegroundColor                = Colors.White;
                    titleBar.InactiveForegroundColor        = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);

                    break;
            }

            if (requestedTheme == ElementTheme.Default)
            {
                titleBar.ButtonHoverBackgroundColor     = null;
                titleBar.ButtonPressedBackgroundColor   = null;
            }
            else
            {
                titleBar.ButtonHoverBackgroundColor     = BarsHelperColorsSetterHelper.GetTitleBarButtonHoverBackgroundColor(titleBar.BackgroundColor.Value, requestedTheme);
                titleBar.ButtonPressedBackgroundColor   = BarsHelperColorsSetterHelper.GetTitleBarButtonPressedBackgroundColor(titleBar.BackgroundColor.Value, requestedTheme);
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
