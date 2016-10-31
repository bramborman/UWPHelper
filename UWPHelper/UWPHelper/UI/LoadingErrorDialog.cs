using UWPHelper.Utilities;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public sealed class LoadingErrorDialog : ContentDialog
    {
        public LoadingErrorDialog(string title) : this(title, "")
        {

        }

        public LoadingErrorDialog(string title, string continueWith)
        {
            Title   = string.Format(ResourceLoaderHelper.GetString("LoadingErrorDialog/Title"), title);
            Content = string.Format(ResourceLoaderHelper.GetString("LoadingErrorDialog/Content"), string.IsNullOrWhiteSpace(continueWith) ? "" : $" {continueWith}");

            PrimaryButtonText   = ResourceLoaderHelper.GetString("LoadingErrorDialog/PrimaryButtonText");
            SecondaryButtonText = ResourceLoaderHelper.GetString("LoadingErrorDialog/SecondaryButtonText");
        }
    }
}
