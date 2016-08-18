using System;
using System.Threading.Tasks;
using UWPHelper.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Test
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        private async Task Initialize(IActivatedEventArgs args)
        {
            bool loadAppData = AppData.Current == null;
            Task loadAppDataTask = null;

            if (loadAppData)
            {
                loadAppDataTask = AppData.LoadAsync();
            }

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = false;
                DebugSettings.IsTextPerformanceVisualizationEnabled = false;
            }
#endif

            LaunchActivatedEventArgs launchArgs = args as LaunchActivatedEventArgs;
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            if (loadAppData)
            {
                await loadAppDataTask;
                AppData.Current.Foo++;
            }

            if (launchArgs?.PrelaunchActivated != true)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), launchArgs?.Arguments);
                }
                
                Window.Current.Activate();

                ApplicationViewExtension.SetTitleBarColors(ElementTheme.Dark);
                ApplicationViewExtension.SetStatusBarColors(ElementTheme.Dark, RequestedTheme);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await Initialize(args);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await Initialize(e);
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await AppData.Current?.SaveAsync();
            deferral.Complete();
        }
    }
}