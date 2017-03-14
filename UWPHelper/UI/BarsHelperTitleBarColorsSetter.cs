using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperTitleBarColorsSetter : IBarsHelperTitleBarColorsSetter
    {
        public ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo> Colors { get; }
        
        public BarsHelperTitleBarColorsSetter(BarsHelperColorsSetterColorInfo defaultThemeColorInfo, BarsHelperColorsSetterColorInfo lightThemeColorInfo, BarsHelperColorsSetterColorInfo darkThemeColorInfo)
        {
            Colors = new ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo>(new Dictionary<ElementTheme, BarsHelperColorsSetterColorInfo>
            {
                { ElementTheme.Default, defaultThemeColorInfo },
                { ElementTheme.Light, lightThemeColorInfo },
                { ElementTheme.Dark, darkThemeColorInfo }
            });
        }
        
        public void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme)
        {
            BarsHelperColorsSetterColorInfo colorInfo = Colors[requestedTheme];

            titleBar.BackgroundColor = colorInfo.BackgroundColor;
            titleBar.ForegroundColor = colorInfo.ForegroundColor;

            titleBar.InactiveBackgroundColor = colorInfo.InactiveBackgroundColor;
            titleBar.InactiveForegroundColor = colorInfo.InactiveForegroundColor;

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
            
            titleBar.ButtonBackgroundColor          = titleBar.BackgroundColor;
            titleBar.ButtonForegroundColor          = titleBar.ForegroundColor;

            titleBar.ButtonHoverForegroundColor     = titleBar.ButtonForegroundColor;
            titleBar.ButtonInactiveBackgroundColor  = titleBar.InactiveBackgroundColor;
            titleBar.ButtonInactiveForegroundColor  = titleBar.InactiveForegroundColor;
            titleBar.ButtonPressedForegroundColor   = titleBar.ButtonForegroundColor;
        }
    }
}
