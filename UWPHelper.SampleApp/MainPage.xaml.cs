using System;
using UWPHelper.UI;
using UWPHelper.Utilities;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPHelper.SampleApp
{
    public sealed partial class MainPage : Page
    {
        private AppData AppData
        {
            get { return AppData.GetForCurrentView(); }
        }
        private int CurrentWindowNumber { get; set; }
        private string CurrentViewId
        {
            get
            {
                try
                {
                    return ViewHelper.GetCurrentViewId().ToString();
                }
                catch
                {
                    return "N/A";
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (AppData.ShowLoadingError)
            {
                AppData.ShowLoadingError = false;

                ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
                LoadingErrorDialog errorDialog = new LoadingErrorDialog(resourceLoader.GetString("LoadingErrorDialog/Settings"), resourceLoader.GetString("LoadingErrorDialog/ContinueWith"));

                if (await errorDialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    Application.Current.Exit();
                }
                else
                {
                    await AppData.SaveAsync();
                }
            }

            ATX_Uri.SelectionStart = ATX_Uri.Text.Length;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CurrentWindowNumber = (int)e.Parameter;
        }
    }
}
