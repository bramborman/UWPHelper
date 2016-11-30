using System;
using UWPHelper.UI;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
                        App.Current.Exit();
                    }
                    else
                    {
                        await AppData.Current.SaveAsync();
                    }
                }

                TX_Uri.SelectionStart = TX_Uri.Text.Length;
            };
        }

        private void TX_Uri_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                LaunchUri(this, null);
            }
        }

        private async void LaunchUri(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TX_Uri.Text))
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri(TX_Uri.Text));
                }
                catch (Exception exception)
                {
                    await new ContentDialog()
                    {
                        Content = exception,
                        SecondaryButtonText = ResourceLoader.GetForCurrentView().GetString("LaunchUriExceptionDialog/SecondaryButtonText")
                    }.ShowAsync();
                }
            }
        }
    }
}
