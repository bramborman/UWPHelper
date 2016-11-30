using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.ObjectModel;
using UWPHelper.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public sealed partial class AboutApp : UserControl
    {
        public static readonly DependencyProperty AppStoreIdProperty            = DependencyProperty.Register(nameof(AppStoreId), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty AppUriProperty                = DependencyProperty.Register(nameof(AppUri), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty AppLogoPathProperty           = DependencyProperty.Register(nameof(AppLogoPath), typeof(string), typeof(AboutApp), new PropertyMetadata(@"ms-appx:Assets/AppLogo.png"));
        public static readonly DependencyProperty AppDeveloperMailProperty      = DependencyProperty.Register(nameof(AppDeveloperMail), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty LinksProperty                 = DependencyProperty.Register(nameof(Links), typeof(ObservableCollection<HyperlinkButton>), typeof(AboutApp), new PropertyMetadata(new ObservableCollection<HyperlinkButton>()));
        public static readonly DependencyProperty IsGitHubLinkEnabledProperty   = DependencyProperty.Register(nameof(IsGitHubLinkEnabled), typeof(bool), typeof(AboutApp), null);
        public static readonly DependencyProperty GitHubProjectNameProperty     = DependencyProperty.Register(nameof(GitHubProjectName), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty GitHubLinkUrlProperty         = DependencyProperty.Register(nameof(GitHubLinkUrl), typeof(string), typeof(AboutApp), null);

        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

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
            get { return $"{resourceLoader.GetString("Version")} {Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision}"; }
        }
        private string AppPublisher
        {
            get { return Package.Current.PublisherDisplayName; }
        }
        private string AppStoreLink
        {
            get { return $"{AppName} {resourceLoader.GetString("InWindowsStore")}"; }
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
        public ObservableCollection<HyperlinkButton> Links
        {
            get { return (ObservableCollection<HyperlinkButton>)GetValue(LinksProperty); }
        }
        public bool IsGitHubLinkEnabled
        {
            get { return (bool)GetValue(IsGitHubLinkEnabledProperty); }
            set { SetValue(IsGitHubLinkEnabledProperty, value); }
        }
        public string GitHubProjectName
        {
            get { return (string)GetValue(GitHubProjectNameProperty); }
            set { SetValue(GitHubProjectNameProperty, value); }
        }
        public string GitHubLinkUrl
        {
            get { return (string)GetValue(GitHubLinkUrlProperty); }
            set { SetValue(GitHubLinkUrlProperty, value); }
        }

        public AboutApp()
        {
            InitializeComponent();
        }

        private async void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            string content = (string)(((HyperlinkButton)sender).Content);

            if (content == AppStoreLink)
            {
                await Launcher.LaunchUriAsync(new Uri($@"ms-windows-store://pdp/?ProductId={AppStoreId}"));
            }
            else if (content == resourceLoader.GetString("RateApp/Content"))
            {
                await Launcher.LaunchUriAsync(new Uri($@"ms-windows-store://review/?ProductId={AppStoreId}"));
            }
            else if (content == resourceLoader.GetString("ShareApp/Content"))
            {
                DataTransferManager.GetForCurrentView().DataRequested += SharingDataRequested;
                DataTransferManager.ShowShareUI();
            }
            else if (content == resourceLoader.GetString("MoreAppsByThisPublisher/Content"))
            {
                await Launcher.LaunchUriAsync(new Uri($@"ms-windows-store://publisher/?name={AppPublisher}"));
            }
            else
            {
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.To.Add(new EmailRecipient(AppDeveloperMail, AppPublisher));
                emailMessage.Subject = $"{AppName} app: ";
                emailMessage.Body = $@"

Device family: {DeviceInfo.SystemFamily}
Windows version: {DeviceInfo.SystemVersion}
Device info: {DeviceInfo.DeviceManufacturer} {DeviceInfo.DeviceModel}
App info: {AppName} {Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision} ({DeviceInfo.PackageArchitecture})";

                await EmailManager.ShowComposeNewEmailAsync(emailMessage);
            }
        }

        private async void OpenGitHubLink(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(GitHubLinkUrl));
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
