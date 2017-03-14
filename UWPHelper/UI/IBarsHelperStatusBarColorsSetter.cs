using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public interface IBarsHelperStatusBarColorsSetter
    {
        void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDifferentStatusBarColorsOnLandscapeOrientation, DisplayOrientations currentOrientation);
    }
}
