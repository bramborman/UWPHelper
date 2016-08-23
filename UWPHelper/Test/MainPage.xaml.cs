using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Test
{
    public sealed partial class MainPage : Page
    {
        static readonly List<string> _suggestions = new List<string>()
        {
            #region Suggestions
            "calculator:",
            "md-xcalculator:",
            "metronome:",
            "md-mytronome:",
            "md-test:",
            @"ms-windows-store://pdp/?ProductId=",
            @"ms-windows-store://review/?ProductId=",
            @"ms-windows-store://publisher/?name="
            #endregion
        };

        private AppData AppData
        {
            get { return AppData.Current; }
        }
        private List<string> Suggestions
        {
            get { return _suggestions; }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        private void TX_Uri_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                LaunchUri(this, new RoutedEventArgs());
            }
        }

        private async void LaunchUri(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri(TX_Uri.Text));
            }
            catch (Exception ex)
            {
                ContentDialog CD_Popup = new ContentDialog();

                CD_Popup.Content = ex;
                CD_Popup.SecondaryButtonText = "Cancel";

                await CD_Popup.ShowAsync();
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            TX_Uri.Text = (string)e.ClickedItem;
            TX_Uri.Focus(FocusState.Programmatic);
            TX_Uri.SelectionStart = TX_Uri.Text.Length;
            TX_Uri.SelectionLength = 0;
        }
    }
}