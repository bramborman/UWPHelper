using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperStatusBarColorsSetterThemedGray : IBarsHelperStatusBarColorsSetter
    {
        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDifferentStatusBarColorsOnLandscapeOrientation, DisplayOrientations currentOrientation)
        {
            statusBar.BackgroundOpacity = 1.0;
            
            ElementTheme applicationRequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
            bool lightTheme = (requestedTheme == ElementTheme.Default ? applicationRequestedTheme : requestedTheme) == ElementTheme.Light;
            
            if (useDifferentStatusBarColorsOnLandscapeOrientation && (currentOrientation == DisplayOrientations.Landscape || currentOrientation == DisplayOrientations.LandscapeFlipped))
            {
                if (lightTheme)
                {
                    statusBar.BackgroundColor = Color.FromArgb(0xFF, 0xF2, 0xF2, 0xF2);
                }
                else
                {
                    statusBar.BackgroundColor = Color.FromArgb(0xFF, 0x17, 0x17, 0x17);
                }
            }
            else
            {
                if (lightTheme)
                {
                    statusBar.BackgroundColor = Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6);
                }
                else
                {
                    statusBar.BackgroundColor = Color.FromArgb(0xFF, 0x1F, 0x1F, 0x1F);
                }
            }

            if (lightTheme)
            {
                statusBar.ForegroundColor = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);
            }
            else
            {
                statusBar.ForegroundColor = Color.FromArgb(0xFF, 0xBF, 0xBF, 0xBF);
            }
        }
    }
}
