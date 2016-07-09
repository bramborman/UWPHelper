using System.Threading.Tasks;
using UWPHelper.Utilities;
using Windows.Storage;

namespace Test
{
    public class AppData : NotifyPropertyChanged
    {
        const string FILE = "Settings.json";

        public static AppData Current { get; private set; }
        public static bool ShowLoadingError { get; set; }

        int _foo;
        bool _bar;

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
        public bool Bar
        {
            get { return _bar; }
            set
            {
                if (_bar != value)
                {
                    _bar = value;
                    OnPropertyChanged(nameof(Bar));
                }
            }
        }

        public AppData()
        {
            Foo = 0;
            Bar = false;
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

            Current.PropertyChanged += async (sender, e) => await Current.SaveAsync();
        }
    }
}