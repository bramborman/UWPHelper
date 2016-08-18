using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public static class ApplicationViewExtension
    {
        public static void SetTitleBarColors(ElementTheme requestedTheme)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (titleBar != null)
                {
                    switch (requestedTheme)
                    {
                        case ElementTheme.Default:
                            titleBar.BackgroundColor = null;
                            titleBar.ForegroundColor = null;
                            titleBar.InactiveForegroundColor = null;

                            titleBar.ButtonHoverBackgroundColor = null;
                            titleBar.ButtonInactiveForegroundColor = null;
                            titleBar.ButtonPressedBackgroundColor = null;

                            break;
                        
                        case ElementTheme.Light:
                            titleBar.BackgroundColor = Colors.White;
                            titleBar.ForegroundColor = Colors.Black;
                            titleBar.InactiveForegroundColor = Color.FromArgb(0xFF, 0x99, 0x99, 0x99);

                            titleBar.ButtonHoverBackgroundColor     = Color.FromArgb(0xFF, 0xE6, 0xE6, 0xE6);
                            titleBar.ButtonInactiveForegroundColor  = Color.FromArgb(0xFF, 0x99, 0x99, 0x99);
                            titleBar.ButtonPressedBackgroundColor   = Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC);

                            break;

                        case ElementTheme.Dark:
                            titleBar.BackgroundColor = Colors.Black;
                            titleBar.ForegroundColor = Colors.White;
                            titleBar.InactiveForegroundColor = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);

                            titleBar.ButtonHoverBackgroundColor     = Color.FromArgb(0xFF, 0x19, 0x19, 0x19);
                            titleBar.ButtonInactiveForegroundColor  = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);
                            titleBar.ButtonPressedBackgroundColor   = Color.FromArgb(0xFF, 0x33, 0x33, 0x33);

                            break;
                    }
                    
                    titleBar.InactiveBackgroundColor = titleBar.BackgroundColor;

                    titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;
                    titleBar.ButtonForegroundColor = titleBar.ForegroundColor;

                    titleBar.ButtonHoverForegroundColor     = titleBar.ButtonForegroundColor;
                    titleBar.ButtonInactiveBackgroundColor  = titleBar.InactiveBackgroundColor;
                    titleBar.ButtonPressedForegroundColor   = titleBar.ButtonForegroundColor;
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