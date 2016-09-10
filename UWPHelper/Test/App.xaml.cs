using System;
using System.Threading.Tasks;
using UWPHelper.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Test
{
    sealed partial class App : Application
    {
        public static new App Current { get; private set; }

        public App()
        {
            Current = this;

            InitializeComponent();
            Suspending += OnSuspending;
        }

        private async void Initialize(IActivatedEventArgs args)
        {
            bool loadAppData = AppData.Current == null;
            Task loadAppDataTask = null;

            if (loadAppData)
            {
                loadAppDataTask = AppData.LoadAsync();
            }

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootFrame;

                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                {
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(330, 460));
                }
            }

            LaunchActivatedEventArgs launchArgs = args as LaunchActivatedEventArgs;

            if (loadAppData)
            {
                await loadAppDataTask;
                AppData.Current.SetTheme();

                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    Window.Current.VisibilityChanged += (sender2, e2) =>
                    {
                        if (AppData.Current.Theme == ElementTheme.Default && e2.Visible)
                        {
                            ApplicationViewHelper.SetStatusBarColors(AppData.Current.Theme, RequestedTheme);
                        }
                    };
                }
            }

            if (launchArgs?.PrelaunchActivated != true)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), launchArgs?.Arguments);
                }
                
                Window.Current.Activate();
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            Initialize(args);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Initialize(e);
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