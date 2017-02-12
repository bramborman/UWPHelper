using System;
using System.Threading.Tasks;
using UWPHelper.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPHelper.SampleApp
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

#pragma warning disable IDE1006 // Naming Styles
        protected override async void OnActivated(IActivatedEventArgs args)
#pragma warning restore IDE1006 // Naming Styles
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
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(450, 460));
                }
            }

            LaunchActivatedEventArgs launchArgs = args as LaunchActivatedEventArgs;

            if (loadAppData)
            {
                await loadAppDataTask;
                BarsHelper.Current.Initialize(BarsHelperColorMode.Themed, () => AppData.Current.Theme, AppData.Current, nameof(AppData.Theme));
                AppData.Current.SetTheme();
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

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            OnActivated(e);
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

#pragma warning disable IDE1006 // Naming Styles
        private async void OnSuspending(object sender, SuspendingEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            await AppData.Current.SaveAsync();
            deferral.Complete();
        }
    }
}
