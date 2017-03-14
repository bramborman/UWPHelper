using Windows.UI;

namespace UWPHelper.UI
{
    public sealed class BarsHelperColorsSetterColorInfo
    {
        public Color? BackgroundColor { get; }
        public Color? ForegroundColor { get; }
        public Color? InactiveBackgroundColor { get; }
        public Color? InactiveForegroundColor { get; }
        
        public BarsHelperColorsSetterColorInfo(Color? backgroundColor, Color? foregroundColor, Color? inactiveBackgroundColor, Color? inactiveForegroundColor)
        {
            BackgroundColor         = backgroundColor;
            ForegroundColor         = foregroundColor;
            InactiveBackgroundColor = inactiveBackgroundColor;
            InactiveForegroundColor = inactiveForegroundColor;
        }
    }
}
