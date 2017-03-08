using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class BarsHelperColorsSetterThemedDefault : IBarsHelperColorsSetter
    {
        public void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme)
        {
            BarsHelperColorsSetterTitleBarBlackWhite.SetTitleBarBlackWhiteColors(titleBar, requestedTheme);
        }

        public void SetStatusBarColors(StatusBar statusBar, ElementTheme requestedTheme, bool useDarkerStatusBarOnLandscapeOrientation, DisplayOrientations currentOrientation)
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

                if (requestedTheme == ElementTheme.Light)
                {
                    statusBar.ForegroundColor = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);
                    statusBar.BackgroundColor = Colors.White;
                }
                else
                {
                    statusBar.ForegroundColor = Color.FromArgb(0xFF, 0xBF, 0xBF, 0xBF);
                    statusBar.BackgroundColor = Colors.Black;
                }
            }
        }
    }
}
