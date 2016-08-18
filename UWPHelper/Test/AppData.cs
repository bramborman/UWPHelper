using System.Threading.Tasks;
using UWPHelper.Utilities;
using Windows.Storage;

namespace Test
{
    public class AppData : NotifyPropertyChanged
    {
        const string FILE = "AppData.json";

        public static AppData Current { get; private set; }
        public static bool ShowLoadingError { get; set; }

        int _foo;
        bool? _watBoxChecked;
        TestEnum _testEnum;

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

        public AppData()
        {
            Foo = 0;
            WatBoxChecked = true;
            TestEnum = TestEnum._0;
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