using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPHelper.UI
{
    public class AdvancedTextBox : TextBox
    {
        public event RoutedEventHandler SubmitKeyDown;
        public event RoutedEventHandler SubmitKeyUp;

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                SubmitKeyDown?.Invoke(this, new RoutedEventArgs());
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                SubmitKeyUp?.Invoke(this, new RoutedEventArgs());
            }
            else
            {
                base.OnKeyUp(e);
            }
        }
    }
}
