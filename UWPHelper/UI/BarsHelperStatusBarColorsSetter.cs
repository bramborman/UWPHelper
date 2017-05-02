using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWPHelper.Utilities;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperStatusBarColorsSetter : IBarsHelperStatusBarColorsSetter
    {
        public double BackgroundOpacity { get; }
        public bool CalculateThemeForElementThemeDefault { get; }
        public ReadOnlyDictionary<bool, ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo>> Colors { get; }

        public BarsHelperStatusBarColorsSetter(double backgroundOpacity,
                                               bool calculateThemeForElementThemeDefault,
                                               BarsHelperColorsSetterColorInfo defaultThemeColorInfo,
                                               BarsHelperColorsSetterColorInfo lightThemeColorInfo,
                                               BarsHelperColorsSetterColorInfo darkThemeColorInfo) : this(backgroundOpacity, calculateThemeForElementThemeDefault, defaultThemeColorInfo, lightThemeColorInfo, darkThemeColorInfo, null, null, null)
        {

        }

        public BarsHelperStatusBarColorsSetter(double backgroundOpacity,
                                               bool calculateThemeForElementThemeDefault,
                                               BarsHelperColorsSetterColorInfo defaultThemeColorInfo,
                                               BarsHelperColorsSetterColorInfo lightThemeColorInfo,
                                               BarsHelperColorsSetterColorInfo darkThemeColorInfo,
                                               BarsHelperColorsSetterColorInfo defaultThemeLandscapeColorInfo,
                                               BarsHelperColorsSetterColorInfo lightThemeLandscapeColorInfo,
                                               BarsHelperColorsSetterColorInfo darkThemeLandscapeColorInfo)
        {
            BarsHelperColorsSetterHelper.ValidateDefaultThemeColorInfo(calculateThemeForElementThemeDefault, defaultThemeColorInfo, nameof(defaultThemeColorInfo));
            ExceptionHelper.ValidateObjectNotNull(lightThemeColorInfo, nameof(lightThemeColorInfo));
            ExceptionHelper.ValidateObjectNotNull(darkThemeColorInfo, nameof(darkThemeColorInfo));
            
            BackgroundOpacity                       = backgroundOpacity;
            CalculateThemeForElementThemeDefault    = calculateThemeForElementThemeDefault;

            Colors = new ReadOnlyDictionary<bool, ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo>>(new Dictionary<bool, ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo>>
            {
                { false, new ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo>(new Dictionary<ElementTheme, BarsHelperColorsSetterColorInfo>
                    {
                        { ElementTheme.Default, defaultThemeColorInfo },
                        { ElementTheme.Light, lightThemeColorInfo },
                        { ElementTheme.Dark, darkThemeColorInfo }
                    })
                },
                { true, new ReadOnlyDictionary<ElementTheme, BarsHelperColorsSetterColorInfo>(new Dictionary<ElementTheme, BarsHelperColorsSetterColorInfo>
                    {
                        { ElementTheme.Default, defaultThemeLandscapeColorInfo ?? defaultThemeColorInfo },
                        { ElementTheme.Light, lightThemeLandscapeColorInfo ?? lightThemeColorInfo },
                        { ElementTheme.Dark, darkThemeLandscapeColorInfo ?? darkThemeColorInfo }
                    })
                },
            });
        }

        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDifferentStatusBarColorsOnLandscapeOrientation, DisplayOrientations currentOrientation)
        {
            BarsHelperColorsSetterHelper.TryCalculateThemeForElementThemeDefault(CalculateThemeForElementThemeDefault, ref requestedTheme);

            bool differentColors = useDifferentStatusBarColorsOnLandscapeOrientation && (currentOrientation == DisplayOrientations.Landscape || currentOrientation == DisplayOrientations.LandscapeFlipped);
            BarsHelperColorsSetterColorInfo colorInfo = Colors[differentColors][requestedTheme];

            statusBar.BackgroundOpacity = BackgroundOpacity;
            statusBar.BackgroundColor   = colorInfo.BackgroundColor;
            statusBar.ForegroundColor   = colorInfo.ForegroundColor;
        }
    }
}
