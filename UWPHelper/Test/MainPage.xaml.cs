using Windows.UI.Xaml.Controls;

namespace Test
{
    public sealed partial class MainPage : Page
    {
        private AppData AppData
        {
            get { return AppData.Current; }
        }
        public static MainPage m;
        public MainPage()
        {
            m = this;
            InitializeComponent();
        }
    }
}