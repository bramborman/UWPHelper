using System;
using System.Collections.Generic;
using UWPHelper.UI;
using UWPHelper.Utilities;
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
            "insideten:",
            "md-insideten:",
            "http://insideten.xyz",
            "http://www.insideten.xyz",
            "https://insideten.xyz",
            "https://www.insideten.xyz",
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

            Loaded += async (sender, e) =>
            {
                if (AppData.ShowLoadingError)
                {
                    AppData.ShowLoadingError = false;

                    if (await LoadingErrorDialog.ShowAsync("settings", " with default settings") == ContentDialogResult.Primary)
                    {
                        App.Current.Exit();
                    }
                    else
                    {
                        await AppData.Current.SaveAsync();
                    }
                }
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
            catch (Exception ex)
            {
                await new ContentDialog()
                {
                    Content = ex,
                    SecondaryButtonText = "Cancel"
                }.ShowAsync();
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