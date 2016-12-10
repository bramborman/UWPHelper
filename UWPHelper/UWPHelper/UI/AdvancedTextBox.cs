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
        public event RoutedEventHandler EscapeKeyDown;
        public event RoutedEventHandler EscapeKeyUp;

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    e.Handled = true;
                    SubmitKeyDown?.Invoke(this, new RoutedEventArgs());
                    break;

                case VirtualKey.Escape:
                    e.Handled = true;
                    EscapeKeyDown?.Invoke(this, new RoutedEventArgs());
                    break;
                
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    e.Handled = true;
                    SubmitKeyUp?.Invoke(this, new RoutedEventArgs());
                    break;

                case VirtualKey.Escape:
                    e.Handled = true;
                    EscapeKeyUp?.Invoke(this, new RoutedEventArgs());
                    break;

                default:
                    base.OnKeyUp(e);
                    break;
            }
        }
    }
}
