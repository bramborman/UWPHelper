using UWPHelper.Utilities;
using Windows.ApplicationModel.Resources;

namespace UWPHelper.UI
{
    public sealed class LoadingErrorDialog : AdvancedContentDialog
    {
        public LoadingErrorDialog(string title) : this(title, null)
        {

        }

        public LoadingErrorDialog(string title, string continueWith)
        {
            ExceptionHelper.ValidateNotNullOrWhiteSpace(title, nameof(title));
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("UWPHelper/Resources");
            
            Title   = string.Format(resourceLoader.GetString("LoadingErrorDialog/Title"), title);
            Content = string.Format(resourceLoader.GetString("LoadingErrorDialog/Content"), string.IsNullOrWhiteSpace(continueWith) ? "" : $" {continueWith}");

            PrimaryButtonText   = resourceLoader.GetString("LoadingErrorDialog/PrimaryButtonText");
            SecondaryButtonText = resourceLoader.GetString("LoadingErrorDialog/SecondaryButtonText");
        }
    }
}
