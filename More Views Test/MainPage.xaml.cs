using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MoreViewsTest
{
    public sealed partial class MainPage : Page
    {
        private AppData AppData
        {
            get { return AppData.Current; }
        }
        private string CurrentWindowNumber { get; set; }
        private string CurrentViewId
        {
            get
            {
                if (CoreWindow.GetForCurrentThread() is CoreWindow coreWindow)
                {
                    return ApplicationView.GetApplicationViewIdForWindow(coreWindow).ToString();
                }

                return "N/A";
            }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            int theme = (int)AppData.Theme;

            if (++theme == 3)
            {
                theme = 0;
            }

            AppData.Theme = (ElementTheme)theme;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e?.Parameter is NavigationParameters navigationParameters)
            {
                CurrentWindowNumber = navigationParameters.CurrentWindowNumber.ToString();
            }
            else
            {
                CurrentWindowNumber = "N/A";
            }
        }

        public sealed class NavigationParameters
        {
            public object Arguments { get; }
            public int CurrentWindowNumber { get; }

            public NavigationParameters(object arguments, int currentWindowNumber)
            {
                Arguments           = arguments;
                CurrentWindowNumber = currentWindowNumber;
            }
        }
    }
}
