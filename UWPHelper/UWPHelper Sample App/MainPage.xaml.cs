using System;
using UWPHelper.UI;
using UWPHelper.Utilities;
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

                    if (await new LoadingErrorDialog(ResourceLoaderHelper.GetString("LoadingErrorDialog/Settings"), ResourceLoaderHelper.GetString("LoadingErrorDialog/ContinueWith")).ShowAsync() == ContentDialogResult.Primary)
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
                LaunchUri(this, new RoutedEventArgs());
            }
        }

        private async void LaunchUri(object sender, RoutedEventArgs e)
        {
            try
            {
                await TX_Uri.Text.LaunchAsUriAsync();
            }
            catch (Exception exception)
            {
                await new ContentDialog()
                {
                    Content = exception,
                    SecondaryButtonText = ResourceLoaderHelper.GetString("LaunchUriExceptionDialog/SecondaryButtonText")
                }.ShowAsync();
            }
        }
    }
}
