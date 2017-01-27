using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public sealed partial class ThemeSelector : UserControl
    {
        public static readonly DependencyProperty ComboBoxWidthProperty = DependencyProperty.Register(nameof(ComboBoxWidth), typeof(double), typeof(ThemeSelector), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty ComboBoxStyleProperty = DependencyProperty.Register(nameof(ComboBoxStyle), typeof(Style), typeof(ThemeSelector), null);
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(nameof(Theme), typeof(ElementTheme), typeof(ThemeSelector), null);

        public static bool IsDefaultThemeAvailable { get; }

        private Visibility DefaultThemeVisibility
        {
            get
            {
                return IsDefaultThemeAvailable ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public double ComboBoxWidth
        {
            get { return (double)GetValue(ComboBoxWidthProperty); }
            set { SetValue(ComboBoxWidthProperty, value); }
        }
        public Style ComboBoxStyle
        {
            get { return (Style)GetValue(ComboBoxStyleProperty); }
            set { SetValue(ComboBoxStyleProperty, value); }
        }
        public ElementTheme Theme
        {
            get { return (ElementTheme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        static ThemeSelector()
        {
            IsDefaultThemeAvailable = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile" || (AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Holographic" && ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3, 0));
        }

        public ThemeSelector()
        {
            InitializeComponent();
        }
    }
}
