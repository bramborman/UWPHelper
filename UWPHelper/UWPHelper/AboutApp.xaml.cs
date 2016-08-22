using Microsoft.Services.Store.Engagement;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper
{
    public sealed partial class AboutApp : UserControl
    {
        public static readonly DependencyProperty AppStoreIdProperty = DependencyProperty.Register(nameof(AppStoreId), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty AppUriProperty     = DependencyProperty.Register(nameof(AppUri), typeof(string), typeof(AboutApp), null);
        
        private PackageVersion Version
        {
            get { return Package.Current.Id.Version; }
        }
        private bool FeedbackSupported
        {
            get { return StoreServicesFeedbackLauncher.IsSupported(); }
        }
        private string AppName
        {
            get { return Package.Current.DisplayName; }
        }
        private string AppVersion
        {
            get { return $"Version {Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision}"; }
        }
        private string AppStoreLink
        {
            get { return AppName + " in Windows Store"; }
        }

        public string AppStoreId
        {
            get { return (string)GetValue(AppStoreIdProperty); }
            set { SetValue(AppStoreIdProperty, value); }
        }
        public string AppUri
        {
            get { return (string)GetValue(AppUriProperty); }
            set { SetValue(AppUriProperty, value); }
        }

        public AboutApp()
        {
            InitializeComponent();
        }

        private async void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            string content = ((HyperlinkButton)sender).Content.ToString();

            if (content == AppStoreLink)
            {
                await Launcher.LaunchUriAsync(new Uri(@"ms-windows-store://pdp/?ProductId=" + AppStoreId));
            }
            else if (content == "Rate this app")
            {
                await Launcher.LaunchUriAsync(new Uri(@"ms-windows-store://review/?ProductId=" + AppStoreId));
            }
            else if (content == "Share this app")
            {
                DataTransferManager.GetForCurrentView().DataRequested += SharingDataRequested;
                DataTransferManager.ShowShareUI();
            }
            else// if (content == "More apps by this publisher")
            {
                await Launcher.LaunchUriAsync(new Uri(@"ms-windows-store://publisher/?name=Marian Dolinský"));
            }
        }

        private void SharingDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            string description = $"{AppName} in Windows Store";

            args.Request.Data.Properties.Title = AppName;
            args.Request.Data.Properties.Description = description;
            args.Request.Data.SetText(description + @" - https://www.microsoft.com/store/apps/" + AppStoreId);
            args.Request.Data.SetApplicationLink(new Uri(AppUri));

            DataTransferManager.GetForCurrentView().DataRequested += SharingDataRequested;
        }

        private async void OpenFeedbackHub(object sender, RoutedEventArgs e)
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }
    }
}