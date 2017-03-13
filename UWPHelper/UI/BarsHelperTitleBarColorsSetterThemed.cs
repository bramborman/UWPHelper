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
                    titleBar.BackgroundColor = null;
                    titleBar.ForegroundColor = null;
                    
                    break;
                        
                case ElementTheme.Light:
                    titleBar.BackgroundColor = Colors.White;
                    titleBar.ForegroundColor = Colors.Black;
                    
                    break;

                case ElementTheme.Dark:
                    titleBar.BackgroundColor = Colors.Black;
                    titleBar.ForegroundColor = Colors.White;

                    break;
            }

            if (requestedTheme == ElementTheme.Default)
            {
                titleBar.InactiveForegroundColor        = null;

                titleBar.ButtonHoverBackgroundColor     = null;
                titleBar.ButtonPressedBackgroundColor   = null;
            }
            else
            {
                titleBar.InactiveForegroundColor        = BarsHelperColorsSetterHelper.GetTitleBarInactiveForegroundColor(titleBar.ForegroundColor.Value, requestedTheme);

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
