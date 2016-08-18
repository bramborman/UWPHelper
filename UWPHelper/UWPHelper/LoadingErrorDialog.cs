using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper
{
    public class LoadingErrorDialog
    {
        public static async Task<ContentDialogResult> ShowAsync(string title, bool useDefault)
        {
            ContentDialog CD_Dialog = new ContentDialog();
            CD_Dialog.Title = $"{title} not loaded";
            CD_Dialog.Content = $"{title} were not successfully loaded. You can restart this app to try again or continue using this app with{(useDefault ? " default" : "out Your")} {title.ToLower()}.\nWould You like to close the app now?";

            CD_Dialog.PrimaryButtonText = "Yes";
            CD_Dialog.PrimaryButtonClick += (sender, args) =>
            {
                Application.Current.Exit();
            };

            CD_Dialog.SecondaryButtonText = "No";

            return await CD_Dialog.ShowAsync();
        }
    }
}