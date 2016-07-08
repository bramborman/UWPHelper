using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UWPHelper.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Test
{
    public class AppData : AppDataBase
    {
        const string FILE = "Settings.json";

        public static AppData Current { get; set; }

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

#if DEBUG
            System.Diagnostics.Debug.WriteLine(DebugMessages.OperationInfo("AppData", Operation.Saving, await SaveAsync(FILE)));
#else
            await SaveAsync(FILE);
#endif
        }

        public static async Task<AppData> LoadAsync()
        {
            LoadAsyncResult<AppData> loadAsyncResult = await LoadAsync<AppData>(FILE);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(DebugMessages.OperationInfo("AppData", Operation.Loading, loadAsyncResult.Success));
#endif

            loadAsyncResult.AppData.PropertyChanged += async delegate
            {
                await loadAsyncResult.AppData.SaveAsync();
            };

            return loadAsyncResult.AppData;
        }
    }

    sealed partial class App : Application
    {
        public static AppData appData;

        Task<AppData> loadAppDataTask;

        public App()
        {
            loadAppDataTask = AppData.LoadAsync();

            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();

                ApplicationViewExtension.SetTitleBarColors(ElementTheme.Dark, Current.RequestedTheme);
                ApplicationViewExtension.SetStatusBarColors(ElementTheme.Dark, Current.RequestedTheme);
            }
            
            appData = await loadAppDataTask;
            appData.Foo++;
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
