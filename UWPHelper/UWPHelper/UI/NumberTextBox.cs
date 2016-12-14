using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPHelper.UI
{
    public class NumberTextBox : AdvancedTextBox
    {
        public NumberTextBox()
        {
            InputScope = new InputScope();
            InputScope.Names.Add(new InputScopeName { NameValue = InputScopeNameValue.Number });
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            TextChanging += TextValidation;
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            TextChanging -= TextValidation;
            base.OnLostFocus(e);
        }

        private void TextValidation(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            int cursorPosition = SelectionStart;

            for (int i = 0; i < Text.Length; i++)
            {
                if (!char.IsDigit(Text[i]))
                {
                    Text = Text.Remove(i, 1);
                }
            }

            SelectionStart = cursorPosition;
        }
    }
}
