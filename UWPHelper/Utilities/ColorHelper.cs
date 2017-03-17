using Windows.UI;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public static class ColorHelper
    {
        // Original taken from http://stackoverflow.com/a/9955317/6843321
        public static Color Mix(this Color background, Color foreground)
        {
            if (foreground.A == 0xFF)
            {
                return foreground;
            }
            else if (foreground.A == 0x00)
            {
                return background;
            }

            float alpha      = foreground.A / 255.0f;
            float difference = 1.0f - alpha;

            byte r = (byte)((foreground.R * alpha) + (background.R * difference));
            byte g = (byte)((foreground.G * alpha) + (background.G * difference));
            byte b = (byte)((foreground.B * alpha) + (background.B * difference));

            return Color.FromArgb(0xFF, r, g, b);
        }

        public static float GetLuma(this Color color)
        {
            return (0.2126f * (color.R / 255.0f)) + (0.7152f * (color.G / 255.0f)) + (0.0722f * (color.B / 255.0f));
        }
        
        public static ElementTheme GetContrastingTheme(this Color color)
        {
            float luma = color.GetLuma();
            // Don't know why these values, but with these it works as Windows itself does...
            return luma < 0.4869937f || luma == 0.541142f ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
