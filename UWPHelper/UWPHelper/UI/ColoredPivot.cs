using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPHelper.UI
{
    public sealed class ColoredPivot : Pivot
    {
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(ColoredPivot), null);

        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public ColoredPivot()
        {
            DefaultStyleKey = typeof(ColoredPivot);
        }
    }
}
