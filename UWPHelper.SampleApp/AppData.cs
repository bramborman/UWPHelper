using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UWPHelper.UI;
using UWPHelper.Utilities;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.SampleApp
{
    public sealed class AppData : NotifyPropertyChangedBase
    {
        private const string FILE_NAME = "AppData.json";

        public static AppData Current { get; private set; }

        [JsonIgnore]
        public bool ShowLoadingError { get; set; }
        public int Foo
        {
            get { return (int)GetValue(); }
            set { SetValue(value); }
        }
        public bool? CheckBoxChecked
        {
            get { return (bool?)GetValue(); }
            set { SetValue(value); }
        }
        public SampleEnum SampleEnum
        {
            get { return (SampleEnum)GetValue(); }
            set { SetValue(value); }
        }
        public ElementTheme Theme
        {
            get { return (ElementTheme)GetValue(); }
            set { SetValue(value); }
        }
        public string Uri
        {
            get { return (string)GetValue(); }
            set { SetValue(value); }
        }

        public AppData()
        {
            RegisterProperty(nameof(Foo), typeof(int), 0);
            RegisterProperty(nameof(CheckBoxChecked), typeof(bool?), true);
            RegisterProperty(nameof(SampleEnum), typeof(SampleEnum), SampleEnum.Zero);
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ThemeSelector.IsDefaultThemeAvailable ? ElementTheme.Default : ElementTheme.Dark, (oldValue, newValue) =>
            {
                Current?.SetTheme();
            });
            RegisterProperty(nameof(Uri), typeof(string), "");
        }

        public void SetTheme()
        {
            ((Frame)Window.Current.Content).RequestedTheme = Theme;
            BarsHelper.Current.RequestedTheme = Theme;
        }

        public Task SaveAsync()
        {
            return StorageFileHelper.SaveObjectAsync(this, FILE_NAME, ApplicationData.Current.LocalFolder);
        }

        public static async Task LoadAsync()
        {
#if DEBUG
            if (Current != null)
            {
                throw new Exception("You're not doing it right ;)");
            }
#endif

            var loadObjectAsyncResult = await StorageFileHelper.LoadObjectAsync<AppData>(FILE_NAME, ApplicationData.Current.LocalFolder);
            Current                   = loadObjectAsyncResult.Object;
            Current.ShowLoadingError  = !loadObjectAsyncResult.Success;

            Current.PropertyChanged += async (sender, e) =>
            {
                await Current.SaveAsync();
            };

            Current.Foo++;
        }
    }
}
