using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperStatusBarColorsSetterAccent : IBarsHelperStatusBarColorsSetter
    {
        private bool UseLightOverlay
        {
            get
            {
                return AccentColorHelper.GetForCurrentView().AccentContrastingTheme == ElementTheme.Dark;
            }
        }
        
        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDarkerStatusBarOnLandscapeOrientation, DisplayOrientations currentOrientation)
        {
            statusBar.BackgroundOpacity = 1.0;

            statusBar.BackgroundColor   = AccentColorHelper.GetForCurrentView().AccentColor;
            statusBar.ForegroundColor   = UseLightOverlay ? Colors.White : Colors.Black;
        }
    }
}
