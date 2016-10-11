using UWPHelper.Utilities;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public sealed class LoadingErrorDialog
    {
        public static IAsyncOperation<ContentDialogResult> ShowAsync(string title)
        {
            return ShowAsync(title, "");
        }

        public static IAsyncOperation<ContentDialogResult> ShowAsync(string title, string continueWith)
        {
            ContentDialog CD_Dialog = new ContentDialog();
            CD_Dialog.Title = string.Format(ResourceLoaderHelper.GetString("LoadingErrorDialog/Title"), title);
            CD_Dialog.Content = string.Format(ResourceLoaderHelper.GetString("LoadingErrorDialog/Content"), continueWith);

            CD_Dialog.PrimaryButtonText = ResourceLoaderHelper.GetString("LoadingErrorDialog/PrimaryButtonText");
            CD_Dialog.PrimaryButtonClick += (sender, args) =>
            {
                Application.Current.Exit();
            };

            CD_Dialog.SecondaryButtonText = ResourceLoaderHelper.GetString("LoadingErrorDialog/SecondaryButtonText");

            return CD_Dialog.ShowAsync();
        }
    }
}