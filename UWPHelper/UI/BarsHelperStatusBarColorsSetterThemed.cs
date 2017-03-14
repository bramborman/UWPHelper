using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperStatusBarColorsSetterThemed : IBarsHelperStatusBarColorsSetter
    {
        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDifferentStatusBarColorsOnLandscapeOrientation, DisplayOrientations currentOrientation)
        {
            if (requestedTheme == ElementTheme.Default)
            {
                statusBar.BackgroundOpacity = 0.0;

                statusBar.ForegroundColor   = null;
                statusBar.BackgroundColor   = null;
            }
            else
            {
                statusBar.BackgroundOpacity = 1.0;

                statusBar.BackgroundColor   = requestedTheme == ElementTheme.Light ? Colors.White : Colors.Black;
                statusBar.ForegroundColor   = BarsHelperColorsSetterHelper.GetStatusBarForegroundColor(statusBar.BackgroundColor.Value, requestedTheme);
            }
        }
    }
}
