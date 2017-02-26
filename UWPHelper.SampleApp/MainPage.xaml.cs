using System;
using UWPHelper.UI;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.SampleApp
{
    public sealed partial class MainPage : Page
    {
        private AppData AppData
        {
            get { return AppData.Current; }
        }

        public MainPage()
        {
            InitializeComponent();

            Loaded += async (sender, e) =>
            {
                if (AppData.ShowLoadingError)
                {
                    AppData.ShowLoadingError = false;
                    ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

                    if (await new LoadingErrorDialog(resourceLoader.GetString("LoadingErrorDialog/Settings"), resourceLoader.GetString("LoadingErrorDialog/ContinueWith")).ShowAsync() == ContentDialogResult.Primary)
                    {
                        Application.Current.Exit();
                    }
                    else
                    {
                        await AppData.Current.SaveAsync();
                    }
                }

                ATX_Uri.SelectionStart = ATX_Uri.Text.Length;
            };
        }
        
        private async void LaunchUriAsync(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ATX_Uri.Text))
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri(ATX_Uri.Text));
                }
                catch (Exception exception)
                {
                    await new AdvancedContentDialog()
                    {
                        Content             = exception,
                        SecondaryButtonText = ResourceLoader.GetForCurrentView().GetString("LaunchUriExceptionDialog/SecondaryButtonText")
                    }.ShowAsync();
                }
            }
        }
    }
}
