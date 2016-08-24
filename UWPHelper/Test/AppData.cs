using System.Threading.Tasks;
using UWPHelper.UI;
using UWPHelper.Utilities;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Test
{
    public class AppData : NotifyPropertyChanged
    {
        const string FILE = "AppData.json";

        public static AppData Current { get; private set; }
        public static bool ShowLoadingError { get; set; }

        #region Backing stores
        int _foo;
        bool? _watBoxChecked;
        TestEnum _testEnum;
        ElementTheme _theme;
        string _uri;
        #endregion

        public int Foo
        {
            get { return _foo; }
            set
            {
                if (_foo != value)
                {
                    _foo = value;
                    OnPropertyChanged(nameof(Foo));
                }
            }
        }
        public bool? WatBoxChecked
        {
            get { return _watBoxChecked; }
            set
            {
                if (_watBoxChecked != value)
                {
                    _watBoxChecked = value;
                    OnPropertyChanged(nameof(WatBoxChecked));
                }
            }
        }
        public TestEnum TestEnum
        {
            get { return _testEnum; }
            set
            {
                if (_testEnum != value)
                {
                    _testEnum = value;
                    OnPropertyChanged(nameof(TestEnum));
                }
            }
        }
        public ElementTheme Theme
        {
            get { return _theme; }
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    OnPropertyChanged(nameof(Theme));

                    Current?.SetTheme();
                }
            }
        }
        public string Uri
        {
            get { return _uri; }
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    OnPropertyChanged(nameof(Uri));
                }
            }
        }

        public AppData()
        {
            Foo             = 0;
            WatBoxChecked   = true;
            TestEnum        = TestEnum._0;
            Theme           = ThemeSelector.IsDefaultThemeAvailable ? ElementTheme.Default : ElementTheme.Dark;
            Uri             = "";
        }

        public void SetTheme()
        {
            ((Frame)Window.Current.Content).RequestedTheme = Theme;
            ApplicationViewExtension.SetTitleBarColors(Theme);
            ApplicationViewExtension.SetStatusBarColors(Theme, App.Current.RequestedTheme);
        }

        public async Task SaveAsync()
        {
            await StorageFileHelper.SaveObjectAsync(this, FILE, ApplicationData.Current.LocalFolder);
        }

        public static async Task LoadAsync()
        {
            var loadObjectAsyncResult = await StorageFileHelper.LoadObjectAsync<AppData>(FILE, ApplicationData.Current.LocalFolder);
            Current             = loadObjectAsyncResult.Object;
            ShowLoadingError    = !loadObjectAsyncResult.Success;

            Current.PropertyChanged += async (sender, e) =>
            {
                await Current.SaveAsync();
            };

            Current.Foo++;
        }
    }
}