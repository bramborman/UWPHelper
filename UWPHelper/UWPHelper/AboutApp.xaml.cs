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

        readonly PackageVersion version = Package.Current.Id.Version;

        private string AppName
        {
            get { return Package.Current.DisplayName; }
        }
        private string AppVersion
        {
            get { return $"Version {version.Major}.{version.Minor}.{version.Build}.{version.Revision}"; }
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
        /*
        private void Feedback(object sender, RoutedEventArgs e)
        {
            open feedback hub
        }
        */
    }
}
