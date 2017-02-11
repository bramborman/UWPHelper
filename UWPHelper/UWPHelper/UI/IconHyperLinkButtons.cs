using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPHelper.UI
{
    public abstract class IconHyperlinkButton : HyperlinkButton
    {
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(IconHyperlinkButton), null);

        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
    }

    public sealed class FontIconHyperlinkButton : IconHyperlinkButton
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(FontIconHyperlinkButton), null);

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public FontIconHyperlinkButton()
        {
            DefaultStyleKey = typeof(FontIconHyperlinkButton);
        }
    }

    public sealed class PathIconHyperlinkButton : IconHyperlinkButton
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(PathIconHyperlinkButton), null);

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public PathIconHyperlinkButton()
        {
            DefaultStyleKey = typeof(PathIconHyperlinkButton);
        }
    }

    public sealed class BitmapIconHyperlinkButton : IconHyperlinkButton
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(BitmapIconHyperlinkButton), null);

        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        public BitmapIconHyperlinkButton()
        {
            DefaultStyleKey = typeof(BitmapIconHyperlinkButton);
        }
    }
}
