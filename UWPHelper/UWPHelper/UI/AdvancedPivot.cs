using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPHelper.UI
{
    public sealed class AdvancedPivot : Pivot
    {
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(AdvancedPivot), null);

        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public AdvancedPivot()
        {
            DefaultStyleKey = typeof(AdvancedPivot);
        }
    }
}
