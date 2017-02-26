using System;

namespace UWPHelper.UI
{
    public sealed partial class AboutAppDialog : AdvancedContentDialog
    {
        public AboutApp AboutApp
        {
            get { return AA_AboutApp; }
        }

        public AboutAppDialog()
        {
            InitializeComponent();
        }
        
        private void AA_AboutApp_ThirdPartySoftwareInfoDialogOpening(AboutApp sender, EventArgs args)
        {
            Hide();
        }

        private async void AA_AboutApp_ThirdPartySoftwareInfoDialogClosedAsync(AboutApp sender, EventArgs args)
        {
            await ShowAsync();
        }
    }
}
