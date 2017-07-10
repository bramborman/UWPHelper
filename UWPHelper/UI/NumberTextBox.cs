using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPHelper.UI
{
    public class NumberTextBox : AdvancedTextBox
    {
        public NumberTextBox()
        {
            InputScope inputScope = new InputScope();
            inputScope.Names.Add(new InputScopeName
            {
                NameValue = InputScopeNameValue.Number
            });

            InputScope = inputScope;
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
            //TODO: replace with regex
            int cursorPosition = SelectionStart;
            int index = 0;

            while (index < Text.Length)
            {
                if (!char.IsDigit(Text[index]))
                {
                    Text = Text.Remove(index, 1);
                }
                else
                {
                    index++;
                }
            }

            SelectionStart = cursorPosition;
        }
    }
}
