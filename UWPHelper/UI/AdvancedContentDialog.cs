using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public class AdvancedContentDialog : ContentDialog
    {
        public static bool IsOpened { get; private set; }

        public AdvancedContentDialog()
        {
            Opened += (sender, args) =>
            {
                IsOpened = true;
            };

            Closed += (sender, args) =>
            {
                IsOpened = false;
            };
        }
    }
}
