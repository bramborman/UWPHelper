using UWPHelper.Utilities;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class AboutAppLink : DependencyObject
    {
        public static readonly DependencyProperty IconProperty      = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(AboutAppLink), null);
        public static readonly DependencyProperty TextProperty      = DependencyProperty.Register(nameof(Text), typeof(string), typeof(AboutAppLink), null);
        public static readonly DependencyProperty CommandProperty   = DependencyProperty.Register(nameof(Command), typeof(DelegateCommand), typeof(AboutAppLink), null);

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public DelegateCommand Command
        {
            get { return (DelegateCommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
    }
}
