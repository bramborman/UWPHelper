using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPHelper.UI
{
    public sealed class FontIconHyperLinkButton : HyperlinkButton
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(FontIconHyperLinkButton), null);

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public FontIconHyperLinkButton()
        {
            DefaultStyleKey = typeof(FontIconHyperLinkButton);
        }
    }

    public sealed class PathIconHyperLinkButton : HyperlinkButton
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(PathIconHyperLinkButton), null);

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public PathIconHyperLinkButton()
        {
            DefaultStyleKey = typeof(PathIconHyperLinkButton);
        }
    }

    public sealed class BitmapIconHyperLinkButton : HyperlinkButton
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(nameof(UriSource), typeof(Uri), typeof(BitmapIconHyperLinkButton), null);

        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        public BitmapIconHyperLinkButton()
        {
            DefaultStyleKey = typeof(BitmapIconHyperLinkButton);
        }
    }
}
