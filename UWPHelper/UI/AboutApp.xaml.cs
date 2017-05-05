using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWPHelper.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public sealed partial class AboutApp : UserControl
    {
        #region Licenses
        private const string UWPHELPER_LICENSE =
@"MIT License

Copyright (c) 2017 Marian Dolinský

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
        private const string JSON_NET_LICENSE =
@"The MIT License (MIT)

Copyright (c) 2007 James Newton-King

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
        #endregion
        
        public static readonly DependencyProperty AppStoreIdProperty            = DependencyProperty.Register(nameof(AppStoreId), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty AppUriProperty                = DependencyProperty.Register(nameof(AppUri), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty AppLogoPathProperty           = DependencyProperty.Register(nameof(AppLogoPath), typeof(string), typeof(AboutApp), new PropertyMetadata(@"ms-appx:Assets/AboutAppIcon.png"));
        public static readonly DependencyProperty AppDeveloperMailProperty      = DependencyProperty.Register(nameof(AppDeveloperMail), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty IsGitHubLinkEnabledProperty   = DependencyProperty.Register(nameof(IsGitHubLinkEnabled), typeof(bool), typeof(AboutApp), null);
        public static readonly DependencyProperty GitHubProjectNameProperty     = DependencyProperty.Register(nameof(GitHubProjectName), typeof(string), typeof(AboutApp), null);
        public static readonly DependencyProperty GitHubLinkUrlProperty         = DependencyProperty.Register(nameof(GitHubLinkUrl), typeof(string), typeof(AboutApp), null);
        
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("UWPHelper/Resources");

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
            get { return $"{resourceLoader.GetString("Version")} {Version.Major}.{Version.Minor}.{Version.Build}"; }
        }
        private string CurrentYear
        {
            get
            {
                return DateTime.Today.Year.ToString();
            }
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
        public List<ThirdPartySoftwareInfo> ThirdPartySoftwareInfo { get; }
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
        public ObservableCollection<HyperlinkButton> Links { get; }

        public event TypedEventHandler<AboutApp, EventArgs> ThirdPartySoftwareInfoDialogOpening;
        public event TypedEventHandler<AboutApp, EventArgs> ThirdPartySoftwareInfoDialogClosed;

        public AboutApp()
        {
            ThirdPartySoftwareInfo = new List<ThirdPartySoftwareInfo>
            {
                new ThirdPartySoftwareInfo { SoftwareName = "UWPHelper", SoftwareLicense = UWPHELPER_LICENSE },
                new ThirdPartySoftwareInfo { SoftwareName = "Json.NET", SoftwareLicense = JSON_NET_LICENSE }
            };
            Links = new ObservableCollection<HyperlinkButton>();

            InitializeComponent();
        }

        private async void OpenHyperLinkAsync(object sender, RoutedEventArgs e)
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
            else if (content == resourceLoader.GetString("ContactDeveloper/Content"))
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
            else
            {
                ThirdPartySoftwareInfoDialogOpening?.Invoke(this, new EventArgs());
                await new ThirdPartySoftwareInfoDialog(ThirdPartySoftwareInfo).ShowAsync();
                ThirdPartySoftwareInfoDialogClosed?.Invoke(this, new EventArgs());
            }
        }

        private async void OpenGitHubLinkAsync(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(GitHubLinkUrl));
        }

        private void SharingDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.Properties.Title          = AppName;
            args.Request.Data.Properties.Description    = AppStoreLink;

            args.Request.Data.SetText(AppStoreLink + @" (https://www.microsoft.com/store/apps/" + AppStoreId + ")");
            args.Request.Data.SetWebLink(new Uri("https://www.microsoft.com/store/apps/" + AppStoreId));
            args.Request.Data.SetApplicationLink(new Uri(AppUri));

            DataTransferManager.GetForCurrentView().DataRequested -= SharingDataRequested;
        }
        
        private async void OpenFeedbackHubAsync(object sender, RoutedEventArgs e)
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }
    }
}
