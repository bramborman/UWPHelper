using System.Threading.Tasks;
using UWPHelper.UI;
using UWPHelper.Utilities;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Test
{
    public class AppData : NotifyPropertyChangedBase
    {
        const string FILE = "AppData.json";

        public static AppData Current { get; private set; }
        public static bool ShowLoadingError { get; set; }

        public int Foo
        {
            get { return GetValue<int>(nameof(Foo)); }
            set { SetValue(nameof(Foo), ref value); }
        }
        public bool? WatBoxChecked
        {
            get { return GetValue<bool?>(nameof(WatBoxChecked)); }
            set { SetValue(nameof(WatBoxChecked), ref value); }
        }
        public TestEnum TestEnum
        {
            get { return GetValue<TestEnum>(nameof(TestEnum)); }
            set { SetValue(nameof(TestEnum), ref value); }
        }
        public ElementTheme Theme
        {
            get { return GetValue<ElementTheme>(nameof(Theme)); }
            set { SetValue(nameof(Theme), ref value); }
        }
        public string Uri
        {
            get { return GetValue<string>(nameof(Uri)); }
            set { SetValue(nameof(Uri), ref value); }
        }

        public AppData()
        {
            RegisterProperty(nameof(Foo), typeof(int), 0);
            RegisterProperty(nameof(WatBoxChecked), typeof(bool?), true);
            RegisterProperty(nameof(TestEnum), typeof(TestEnum), TestEnum._0);
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ThemeSelector.IsDefaultThemeAvailable ? ElementTheme.Default : ElementTheme.Dark);
            RegisterProperty(nameof(Uri), typeof(string), "");
        }

        public void SetTheme()
        {
            ((Frame)Window.Current.Content).RequestedTheme = Theme;
            ApplicationViewHelper.SetTitleBarColors(Theme);
            ApplicationViewHelper.SetStatusBarColors(Theme, App.Current.RequestedTheme);
        }

        public Task SaveAsync()
        {
            return StorageFileHelper.SaveObjectAsync(this, FILE, ApplicationData.Current.LocalFolder);
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