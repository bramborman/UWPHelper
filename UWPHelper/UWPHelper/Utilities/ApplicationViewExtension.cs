using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public static class ApplicationViewExtension
    {
        public static void SetTitleBarColors(ElementTheme requestedTheme, ApplicationTheme applicationTheme)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (titleBar != null)
                {
                    ElementTheme finalTheme = requestedTheme != ElementTheme.Default ? requestedTheme : (applicationTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark);

                    if (finalTheme == ElementTheme.Light)
                    {
                        titleBar.ForegroundColor = Colors.Black;
                        titleBar.BackgroundColor = Colors.White;

                        titleBar.ButtonHoverBackgroundColor   = Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6);
                        titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC);
                    }
                    else
                    {
                        titleBar.ForegroundColor = Colors.White;
                        titleBar.BackgroundColor = Colors.Black;

                        titleBar.ButtonHoverBackgroundColor   = Color.FromArgb(0xFF, 0x1F, 0x1F, 0x1F);
                        titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0x33, 0x33, 0x33, 0x33);
                    }

                    titleBar.ButtonForegroundColor          = titleBar.ForegroundColor;
                    titleBar.ButtonHoverForegroundColor     = titleBar.ForegroundColor;
                    titleBar.ButtonPressedForegroundColor   = titleBar.ForegroundColor;

                    titleBar.ButtonBackgroundColor          = titleBar.BackgroundColor;

                    titleBar.InactiveBackgroundColor        = titleBar.BackgroundColor;
                    titleBar.ButtonInactiveBackgroundColor  = titleBar.InactiveBackgroundColor;
                }
            }
        }

        public static void SetStatusBarColors(ElementTheme requestedTheme, ApplicationTheme applicationTheme)
        {
            SetStatusBarColors(requestedTheme, applicationTheme, DisplayOrientations.Portrait);
        }

        public static void SetStatusBarColors(ElementTheme requestedTheme, ApplicationTheme applicationTheme, DisplayOrientations currentOrientation)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();

                if (statusBar != null)
                {
                    ElementTheme finalTheme = requestedTheme != ElementTheme.Default ? requestedTheme : (applicationTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark);

                    statusBar.BackgroundOpacity = 1;

                    if (currentOrientation == DisplayOrientations.Landscape || currentOrientation == DisplayOrientations.LandscapeFlipped)
                    {
                        if (finalTheme == ElementTheme.Light)
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
                        if (finalTheme == ElementTheme.Light)
                        {
                            statusBar.BackgroundColor = Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6);
                        }
                        else
                        {
                            statusBar.BackgroundColor = Color.FromArgb(0xFF, 0x1F, 0x1F, 0x1F);
                        }
                    }

                    if (finalTheme == ElementTheme.Light)
                    {
                        statusBar.ForegroundColor = Color.FromArgb(0xFF, 0x5C, 0x5C, 0x5C);
                    }
                    else
                    {
                        statusBar.ForegroundColor = Color.FromArgb(0xFF, 0xC7, 0xC7, 0xC7);
                    }
                }
            }
        }
    }
}