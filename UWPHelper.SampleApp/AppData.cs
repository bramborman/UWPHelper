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
    public sealed class AppData : ViewSpecificBindableClassBase<AppData>
    {
        private const string FILE_NAME = "AppData.json";

        private static AppData mainAppData;

        public static bool Loaded
        {
            get
            {
                return InstancesCount != 0;
            }
        }

        // Add every property to the GetForCurrentViewMethod cloning
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
            IsPropertyChangedCallbackInvokingEnabled    = false;
            IsPropertyChangedEventInvokingEnabled       = false;

            RegisterProperty(nameof(Foo), typeof(int), 0);
            RegisterProperty(nameof(CheckBoxChecked), typeof(bool?), true);
            RegisterProperty(nameof(SampleEnum), typeof(SampleEnum), SampleEnum.Zero);
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ThemeSelector.IsDefaultThemeAvailable ? ElementTheme.Default : ElementTheme.Dark, (sender, e) =>
            {
                SetTheme();
            });
            RegisterProperty(nameof(Uri), typeof(string), "");
        }

        public void SetTheme()
        {
            ((Frame)Window.Current.Content).RequestedTheme = Theme;
        }

        public Task SaveAsync()
        {
            return StorageHelper.SaveObjectAsync(this, FILE_NAME, ApplicationData.Current.LocalFolder);
        }

        public static async Task LoadAsync()
        {
#if DEBUG
            if (Loaded)
            {
                throw new Exception("You're not doing it right ;)");
            }
#endif

            StorageHelperLoadResult<AppData> loadResult = await StorageHelper.LoadObjectAsync<AppData>(FILE_NAME, ApplicationData.Current.LocalFolder);
            mainAppData                     = BaseGetForCurrentView(() => loadResult.LoadedObject);
            mainAppData.ShowLoadingError    = loadResult.Status == StorageHelperStatus.Failure;

            MainPropertyChanged += async (sender, e) =>
            {
                await mainAppData.SaveAsync();
            };
            
            mainAppData.IsPropertyChangedCallbackInvokingEnabled    = true;
            mainAppData.IsPropertyChangedEventInvokingEnabled       = true;
        }

        public static AppData GetForCurrentView()
        {
            return BaseGetForCurrentView(() => new AppData
            {
                Foo                                         = mainAppData.Foo,
                CheckBoxChecked                             = mainAppData.CheckBoxChecked,
                SampleEnum                                  = mainAppData.SampleEnum,
                Theme                                       = mainAppData.Theme,
                Uri                                         = mainAppData.Uri,
                IsPropertyChangedCallbackInvokingEnabled    = true,
                IsPropertyChangedEventInvokingEnabled       = true
            });
        }
    }
}
