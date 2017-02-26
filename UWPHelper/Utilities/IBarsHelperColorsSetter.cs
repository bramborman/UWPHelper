using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public interface IBarsHelperColorsSetter
    {
        void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme);

        void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDarkerStatusBarOnLandscapeOrientation, DisplayOrientations currentOrientation);
    }
}
