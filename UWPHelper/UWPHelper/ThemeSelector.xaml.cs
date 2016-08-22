using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper
{
    public sealed partial class ThemeSelector : UserControl
    {
        public static readonly DependencyProperty ComboBoxWidthProperty = DependencyProperty.Register(nameof(ComboBoxWidth), typeof(double), typeof(ThemeSelector), new PropertyMetadata(180.0));
        public static readonly DependencyProperty ThemeProperty         = DependencyProperty.Register(nameof(Theme), typeof(ElementTheme), typeof(ThemeSelector), null);

        public static bool IsDefaultThemeAvailable
        {
            get { return AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile" || ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3, 0); }
        }

        private int ThemeIndex
        {
            get { return (int)Theme - (IsDefaultThemeAvailable ? 0 : 1); }
            set { Theme = (ElementTheme)value + (IsDefaultThemeAvailable ? 0 : 1); }
        }

        public double ComboBoxWidth
        {
            get { return (double)GetValue(ComboBoxWidthProperty); }
            set { SetValue(ComboBoxWidthProperty, value); }
        }
        public ElementTheme Theme
        {
            get { return (ElementTheme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public ThemeSelector()
        {
            InitializeComponent();
        }
    }
}
