using System;
using UWPHelper.Utilities;
using Windows.UI;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public static class BarsHelperColorsSetterHelper
    {
        public static Color GetTitleBarButtonHoverBackgroundColor(Color buttonBackgroundColor, ElementTheme overlayTheme)
        {
            return GetTitleBarButtonBackgroundColor(0x19, buttonBackgroundColor, overlayTheme, nameof(overlayTheme));
        }

        public static Color GetTitleBarButtonPressedBackgroundColor(Color buttonBackgroundColor, ElementTheme overlayTheme)
        {
            return GetTitleBarButtonBackgroundColor(0x33, buttonBackgroundColor, overlayTheme, nameof(overlayTheme));
        }

        private static Color GetTitleBarButtonBackgroundColor(byte alpha, Color buttonBackgroundColor, ElementTheme overlayTheme, string elementThemeParameterName)
        {
            ValidateElementTheme(overlayTheme, elementThemeParameterName);

            if (overlayTheme == ElementTheme.Default)
            {
                overlayTheme = buttonBackgroundColor.GetContrastingTheme();
            }

            byte colorByte = (byte)(overlayTheme == ElementTheme.Dark ? 0xFF : 0x00);
            return buttonBackgroundColor.Mix(Color.FromArgb(alpha, colorByte, colorByte, colorByte));
        }

        private static void ValidateElementTheme(ElementTheme elementTheme, string parameterName)
        {
            if (!Enum.IsDefined(typeof(ElementTheme), elementTheme))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
