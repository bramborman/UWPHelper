using System.Threading.Tasks;
using UWPHelper.UI;
using UWPHelper.Utilities;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Test
{
    public sealed class AppData : NotifyPropertyChangedBase
    {
        const string FILE = "AppData.json";

        public static AppData Current { get; private set; }
        public static bool ShowLoadingError { get; set; }

        public int Foo
        {
            get { return (int)GetValue(nameof(Foo)); }
            set { SetValue(nameof(Foo), ref value); }
        }
        public bool? CheckBoxChecked
        {
            get { return (bool?)GetValue(nameof(CheckBoxChecked)); }
            set { SetValue(nameof(CheckBoxChecked), ref value); }
        }
        public TestEnum TestEnum
        {
            get { return (TestEnum)GetValue(nameof(TestEnum)); }
            set { SetValue(nameof(TestEnum), ref value); }
        }
        public ElementTheme Theme
        {
            get { return (ElementTheme)GetValue(nameof(Theme)); }
            set { SetValue(nameof(Theme), ref value); }
        }
        public string Uri
        {
            get { return (string)GetValue(nameof(Uri)); }
            set { SetValue(nameof(Uri), ref value); }
        }

        public AppData()
        {
            RegisterProperty(nameof(Foo), typeof(int), 0);
            RegisterProperty(nameof(CheckBoxChecked), typeof(bool?), true);
            RegisterProperty(nameof(TestEnum), typeof(TestEnum), TestEnum._0);
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ThemeSelector.IsDefaultThemeAvailable ? ElementTheme.Default : ElementTheme.Dark, (oldValue, newValue) => Current?.SetTheme());
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
