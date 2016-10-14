using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public class AdvancedContentDialog : ContentDialog
    {
        public new event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> PrimaryButtonClick;
        public new event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> SecondaryButtonClick;

        public AdvancedContentDialog()
        {
            base.PrimaryButtonClick     += OnPrimaryButtonClick;
            base.SecondaryButtonClick   += OnSecondaryButtonClick;

            Opened += (sender, args) =>
            {
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            };

            Closed += (sender, args) =>
            {
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            };
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Enter:
                    args.Handled = true;
                    OnPrimaryButtonClick(this, null);
                    break;

                case VirtualKey.Escape:
                    args.Handled = true;
                    OnSecondaryButtonClick(this, null);
                    break;
            }
        }

        protected void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PrimaryButtonClick?.Invoke(sender, null);
        }

        protected void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SecondaryButtonClick?.Invoke(sender, null);
        }
    }
}