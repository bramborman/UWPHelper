using UWPHelper.Utilities;
using Windows.UI;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public static class BarsHelperColorsSetterHelper
    {
        public static Color GetTitleBarInactiveForegroundColor(Color foregroundColor, ElementTheme titleBarTheme)
        {
            return GetTitleBarColor(0x99, false, foregroundColor, titleBarTheme, nameof(titleBarTheme));
        }

        public static Color GetTitleBarButtonHoverBackgroundColor(Color buttonBackgroundColor, ElementTheme titleBarTheme)
        {
            return GetTitleBarColor(0x19, true, buttonBackgroundColor, titleBarTheme, nameof(titleBarTheme));
        }

        public static Color GetTitleBarButtonPressedBackgroundColor(Color buttonBackgroundColor, ElementTheme titleBarTheme)
        {
            return GetTitleBarColor(0x33, true, buttonBackgroundColor, titleBarTheme, nameof(titleBarTheme));
        }

        private static Color GetTitleBarColor(byte overlayAlpha, bool isBackgroundColor, Color baseColor, ElementTheme titleBarTheme, string titleBarThemeParameterName)
        {
            ExceptionHelper.ValidateEnumValueDefined(titleBarTheme, titleBarThemeParameterName);

            if (titleBarTheme == ElementTheme.Default)
            {
                titleBarTheme = baseColor.GetContrastingTheme();
            }

            if (!isBackgroundColor)
            {
                titleBarTheme = titleBarTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
            }

            byte colorByte = (byte)(titleBarTheme == ElementTheme.Dark ? 0xFF : 0x00);
            return baseColor.Mix(Color.FromArgb(overlayAlpha, colorByte, colorByte, colorByte));
        }

        public static Color GetStatusBarForegroundColor(Color backgroundColor, ElementTheme statusBarTheme)
        {
            ExceptionHelper.ValidateEnumValueDefined(statusBarTheme, nameof(statusBarTheme));

            if (statusBarTheme == ElementTheme.Default)
            {
                statusBarTheme = backgroundColor.GetContrastingTheme();
            }

            return statusBarTheme == ElementTheme.Light ? Color.FromArgb(0xFF, 0x66, 0x66, 0x66) : Color.FromArgb(0xFF, 0xBF, 0xBF, 0xBF);
        }
        
        public static ElementTheme GetElementThemeFromApplicationTheme()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
        }

        internal static void TryCalculateThemeForElementThemeDefault(bool calculateThemeForElementThemeDefault, ref ElementTheme requestedTheme)
        {
            if (calculateThemeForElementThemeDefault && requestedTheme == ElementTheme.Default)
            {
                requestedTheme = GetElementThemeFromApplicationTheme();
            }
        }

        internal static void ValidateDefaultThemeColorInfo(bool calculateThemeForElementThemeDefault, BarsHelperColorsSetterColorInfo defaultThemeColorInfo, string parameterName)
        {
            if (!calculateThemeForElementThemeDefault)
            {
                ExceptionHelper.ValidateNotNull(defaultThemeColorInfo, parameterName);
            }
        }
    }
}
