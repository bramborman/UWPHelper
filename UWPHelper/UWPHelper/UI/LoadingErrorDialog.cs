using UWPHelper.Utilities;
using Windows.Foundation;
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
            return new ContentDialog()
            {
                Title   = string.Format(ResourceLoaderHelper.GetString("LoadingErrorDialog/Title"), title),
                Content = string.Format(ResourceLoaderHelper.GetString("LoadingErrorDialog/Content"), continueWith),

                PrimaryButtonText   = ResourceLoaderHelper.GetString("LoadingErrorDialog/PrimaryButtonText"),
                SecondaryButtonText = ResourceLoaderHelper.GetString("LoadingErrorDialog/SecondaryButtonText")
            }.ShowAsync();
        }
    }
}