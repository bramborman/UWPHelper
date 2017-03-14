using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperTitleBarColorsSetter : IBarsHelperTitleBarColorsSetter
    {
        public bool CalculateThemeForElementThemeDefault { get; }
        public ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo> Colors { get; }
        
        public BarsHelperTitleBarColorsSetter(bool calculateThemeForElementThemeDefault, BarsHelperColorsSetterColorInfo defaultThemeColorInfo, BarsHelperColorsSetterColorInfo lightThemeColorInfo, BarsHelperColorsSetterColorInfo darkThemeColorInfo)
        {
            CalculateThemeForElementThemeDefault = calculateThemeForElementThemeDefault;
            Colors = new ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo>(new Dictionary<ElementTheme, BarsHelperColorsSetterColorInfo>
            {
                { ElementTheme.Default, defaultThemeColorInfo },
                { ElementTheme.Light, lightThemeColorInfo },
                { ElementTheme.Dark, darkThemeColorInfo }
            });
        }
        
        public void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme)
        {
            if (CalculateThemeForElementThemeDefault && requestedTheme == ElementTheme.Default)
            {
                requestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
            }

            BarsHelperColorsSetterColorInfo colorInfo = Colors[requestedTheme];

            titleBar.BackgroundColor = colorInfo.BackgroundColor;
            titleBar.ForegroundColor = colorInfo.ForegroundColor;

            titleBar.InactiveBackgroundColor = colorInfo.InactiveBackgroundColor;
            titleBar.InactiveForegroundColor = colorInfo.InactiveForegroundColor;

            if (titleBar.BackgroundColor == null)
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
