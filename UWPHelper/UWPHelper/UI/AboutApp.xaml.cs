using Microsoft.Services.Store.Engagement;
using System;
using UWPHelper.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public sealed partial class AboutApp : UserControl
    {
        public static readonly DependencyProperty AppStoreIdProperty        = DependencyProperty.Register(nameof(AppStoreId), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty AppUriProperty            = DependencyProperty.Register(nameof(AppUri), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty AppLogoPathProperty       = DependencyProperty.Register(nameof(AppLogoPath), typeof(string), typeof(AboutApp), new PropertyMetadata(@"ms-appx:Assets/AppLogo.png"));
        public static readonly DependencyProperty AppDeveloperMailProperty  = DependencyProperty.Register(nameof(AppDeveloperMail), typeof(string), typeof(AboutApp), null);
        
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
            get { return $"{ResourceLoaderHelper.GetString("Version")} {Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision}"; }
        }
        private string AppPublisher
        {
            get { return Package.Current.PublisherDisplayName; }
        }
        private string AppStoreLink
        {
            get { return $"{AppName} {ResourceLoaderHelper.GetString("InWindowsStore")}"; }
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
        public string AppLogoPath
        {
            get { return (string)GetValue(AppLogoPathProperty); }
            set { SetValue(AppLogoPathProperty, value); }
        }
        public string AppDeveloperMail
        {
            get { return (string)GetValue(AppDeveloperMailProperty); }
            set { SetValue(AppDeveloperMailProperty, value); }
        }

        public AboutApp()
        {
            InitializeComponent();
        }

        private async void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            string content = ((TextBlock)((HyperlinkButton)sender).Content).Text;

            if (content.Contains(AppStoreLink))
            {
                await $@"ms-windows-store://pdp/?ProductId={AppStoreId}".LaunchAsUriAsync();
            }
            else if (content.Contains(ResourceLoaderHelper.GetString("RateApp/Text")))
            {
                await $@"ms-windows-store://review/?ProductId={AppStoreId}".LaunchAsUriAsync();
            }
            else if (content.Contains(ResourceLoaderHelper.GetString("ShareApp/Text")))
            {
                DataTransferManager.GetForCurrentView().DataRequested += SharingDataRequested;
                DataTransferManager.ShowShareUI();
            }
            else if (content.Contains(ResourceLoaderHelper.GetString("MoreAppsByPublisher/Text")))
            {
                await $@"ms-windows-store://publisher/?name={AppPublisher}".LaunchAsUriAsync();
            }
            else
            {
                string mailContent = $@"mailto:{AppDeveloperMail}?subject={AppName} app: &body=

Device family: {DeviceInfo.SystemFamily}
Windows version: {DeviceInfo.SystemVersion}
Device info: {DeviceInfo.DeviceManufacturer} {DeviceInfo.DeviceModel}
App info: {AppName} {Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision} ({DeviceInfo.PackageArchitecture})
".Replace(" ", "%20").Replace("\r\n", "%0A");
                await mailContent.LaunchAsUriAsync();
            }
        }

        private void SharingDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.Properties.Title = AppName;
            args.Request.Data.Properties.Description = AppStoreLink;
            args.Request.Data.SetText($@"{AppStoreLink} (https://www.microsoft.com/store/apps/{AppStoreId})");
            args.Request.Data.SetWebLink(new Uri($"https://www.microsoft.com/store/apps/{AppStoreId}"));
            args.Request.Data.SetApplicationLink(new Uri(AppUri));

            DataTransferManager.GetForCurrentView().DataRequested -= SharingDataRequested;
        }
        
        private async void OpenFeedbackHub(object sender, RoutedEventArgs e)
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }
    }
}