﻿using System.Threading.Tasks;
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

                    if (Current != null)
                    {
                        ((Frame)Window.Current.Content).RequestedTheme = value;
                        ApplicationViewExtension.SetTitleBarColors(value);
                        ApplicationViewExtension.SetStatusBarColors(value, App.Current.RequestedTheme);
                    }
                }
            }
        }

        public AppData()
        {
            Foo             = 0;
            WatBoxChecked   = true;
            TestEnum        = TestEnum._0;
            Theme           = ThemeSelector.IsDefaultThemeAvailable ? ElementTheme.Default : ElementTheme.Dark;
        }

        public async Task SaveAsync()
        {
            await Storage.SaveObjectAsync(this, FILE, ApplicationData.Current.LocalFolder);
        }

        public static async Task LoadAsync()
        {
            var loadObjectAsyncResult = await Storage.LoadObjectAsync<AppData>(FILE, ApplicationData.Current.LocalFolder);
            Current             = loadObjectAsyncResult.Object;
            ShowLoadingError    = !loadObjectAsyncResult.Success;

            Current.PropertyChanged += async (sender, e) =>
            {
                await Current.SaveAsync();
            };

            Current.Foo++;
            ((Frame)Window.Current.Content).RequestedTheme = Current.Theme;
            ApplicationViewExtension.SetTitleBarColors(Current.Theme);
            ApplicationViewExtension.SetStatusBarColors(Current.Theme, App.Current.RequestedTheme);
        }
    }
}