using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPHelper.UI
{
    public abstract class HamburgerMenuItem : ListViewItem
    {
        protected const double DEFAULT_ICON_SIZE = 16.0;

        public static readonly DependencyProperty PageTypeProperty  = DependencyProperty.Register(nameof(PageType), typeof(Type), typeof(HamburgerMenuItem), null);
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(HamburgerMenuItem), new PropertyMetadata(HamburgerMenu.DEFAULT_ICON_WIDTH));
        
        public Type PageType
        {
            get { return (Type)GetValue(PageTypeProperty); }
            set { SetValue(PageTypeProperty, value); }
        }
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
    }

    public sealed class FontIconHamburgerMenuItem : HamburgerMenuItem
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(FontIconHamburgerMenuItem), null);
        public static readonly DependencyProperty IconFontSizeProperty = DependencyProperty.Register(nameof(IconFontSize), typeof(double), typeof(FontIconHamburgerMenuItem), new PropertyMetadata(DEFAULT_ICON_SIZE));

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }
        public double IconFontSize
        {
            get { return (double)GetValue(IconFontSizeProperty); }
            set { SetValue(IconFontSizeProperty, value); }
        }

        public FontIconHamburgerMenuItem()
        {
            DefaultStyleKey = typeof(FontIconHamburgerMenuItem);
        }
    }

    public sealed class PathIconHamburgerMenuItem : HamburgerMenuItem
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(PathIconHamburgerMenuItem), null);
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(PathIconHamburgerMenuItem), new PropertyMetadata(DEFAULT_ICON_SIZE));
        
        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public PathIconHamburgerMenuItem()
        {
            DefaultStyleKey = typeof(PathIconHamburgerMenuItem);
        }
    }

    public sealed class BitmapIconHamburgerMenuItem : HamburgerMenuItem
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(BitmapIconHamburgerMenuItem), null);
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(BitmapIconHamburgerMenuItem), new PropertyMetadata(DEFAULT_ICON_SIZE));
        
        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public BitmapIconHamburgerMenuItem()
        {
            DefaultStyleKey = typeof(BitmapIconHamburgerMenuItem);
        }
    }
}
