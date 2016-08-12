using Microsoft.Services.Store.Engagement;
using System;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper
{
    public sealed partial class AboutApp : UserControl
    {
        public static readonly DependencyProperty AppStoreIdProperty = DependencyProperty.Register(nameof(AppStoreId), typeof(string), typeof(AboutApp), new PropertyMetadata(null));
        
        private PackageVersion Version
        {
            get { return Package.Current.Id.Version; }
        }
        private bool FeedbackSupported
        {
            get { return Feedback.IsSupported; }
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

        public AboutApp()
        {
            InitializeComponent();
        }

        private async void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            string content = ((HyperlinkButton)sender).Content.ToString();

            if (content == AppStoreLink)
            {
                await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?ProductId=" + AppStoreId));
            }
            else if (content == "Rate this app")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=" + AppStoreId));
            }
            else if (content == "More apps by this publisher")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-windows-store://publisher/?name=Marian Dolinský"));
            }
        }
        
        private async void Bt_Feedback_Click(object sender, RoutedEventArgs e)
        {
            await Feedback.LaunchFeedbackAsync();
        }
    }
}