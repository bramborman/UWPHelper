using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPHelper.UI
{
    public class AdvancedTextBox : TextBox
    {
        public static readonly DependencyProperty SelectAllOnGotFocusProperty        = DependencyProperty.Register(nameof(SelectAllOnGotFocus), typeof(bool), typeof(AdvancedTextBox), null);
        public static readonly DependencyProperty IsTextFallbackValueEnabledProperty = DependencyProperty.Register(nameof(IsTextFallbackValueEnabled), typeof(bool), typeof(AdvancedTextBox), null);
        public static readonly DependencyProperty TextFallbackValueProperty          = DependencyProperty.Register(nameof(TextFallbackValue), typeof(string), typeof(AdvancedTextBox), null);

        public bool SelectAllOnGotFocus
        {
            get { return (bool)GetValue(SelectAllOnGotFocusProperty); }
            set { SetValue(SelectAllOnGotFocusProperty, value); }
        }
        public bool IsTextFallbackValueEnabled
        {
            get { return (bool)GetValue(IsTextFallbackValueEnabledProperty); }
            set { SetValue(IsTextFallbackValueEnabledProperty, value); }
        }
        public string TextFallbackValue
        {
            get { return (string)GetValue(TextFallbackValueProperty); }
            set { SetValue(TextFallbackValueProperty, value); }
        }

        public event RoutedEventHandler SubmitKeyDown;
        public event RoutedEventHandler SubmitKeyUp;
        public event RoutedEventHandler EscapeKeyDown;
        public event RoutedEventHandler EscapeKeyUp;

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (SelectAllOnGotFocus)
            {
                SelectAll();
            }

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (IsTextFallbackValueEnabled && string.IsNullOrWhiteSpace(Text))
            {
                Text = TextFallbackValue;
            }

            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    RoutedEventHandler submitKeyDownHandler = SubmitKeyDown;

                    if (submitKeyDownHandler != null)
                    {
                        e.Handled = true;
                        submitKeyDownHandler.Invoke(this, new RoutedEventArgs());
                    }

                    break;

                case VirtualKey.Escape:
                    RoutedEventHandler escapeKeyDownHandler = EscapeKeyDown;

                    if (escapeKeyDownHandler != null)
                    {
                        e.Handled = true;
                        escapeKeyDownHandler.Invoke(this, new RoutedEventArgs());
                    }

                    break;
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    RoutedEventHandler submitKeyUpHandler = SubmitKeyUp;

                    if (submitKeyUpHandler != null)
                    {
                        e.Handled = true;
                        submitKeyUpHandler.Invoke(this, new RoutedEventArgs());
                    }

                    break;

                case VirtualKey.Escape:
                    RoutedEventHandler escapeKeyUpHandler = EscapeKeyUp;

                    if (escapeKeyUpHandler != null)
                    {
                        e.Handled = true;
                        escapeKeyUpHandler.Invoke(this, new RoutedEventArgs());
                    }

                    break;
            }

            base.OnKeyUp(e);
        }
    }
}
