using System;
using System.Threading.Tasks;
using UWPHelper.UI;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPHelper.SampleApp
{
    public sealed partial class App : Application
    {
        private static int windowNumber;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

#pragma warning disable IDE1006 // Naming Styles
        protected override async void OnActivated(IActivatedEventArgs args)
#pragma warning restore IDE1006 // Naming Styles
        {
            windowNumber++;

            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                // Will fail on phone when debugger is not attached
                try
                {
                    CoreApplicationView newView = CoreApplication.CreateNewView();
                    int newViewId = 0;

                    await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        Frame newViewRootFrame = InitializeRootFrameForCurrenView();
                        AppData.GetForCurrentView().SetTheme();
                        InitialRootFrameNavigationForCurrentView(newViewRootFrame);

                        newViewId = ApplicationView.GetForCurrentView().Id;
                        await BarsHelper.Current.InitializeForCurrentViewAsync();
                    });

                    if (args is IApplicationViewActivatedEventArgs appViewArgs)
                    {
                        await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId, ViewSizePreference.Default, appViewArgs.CurrentlyShownApplicationViewId, ViewSizePreference.Default);
                    }
                    else
                    {
                        await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
                    }
                }
                catch
                {

                }

                return;
            }

            bool loadAppData = !AppData.Loaded;
            Task loadAppDataTask = null;

            if (loadAppData)
            {
                loadAppDataTask = AppData.LoadAsync();
            }

            Frame rootFrame = InitializeRootFrameForCurrenView();
            LaunchActivatedEventArgs launchArgs = args as LaunchActivatedEventArgs;

            if (loadAppData)
            {
                await loadAppDataTask;

                AppData loadedAppData = AppData.GetForCurrentView();

                BarsHelper.Current.InitializeAutoUpdating(() => loadedAppData.Theme, loadedAppData, nameof(AppData.Theme));
                await BarsHelper.Current.SetStatusBarColorModeAsync(BarsHelperColorMode.ThemedGray);
                await BarsHelper.Current.InitializeForCurrentViewAsync();

                loadedAppData.SetTheme();
                loadedAppData.Foo++;
            }

            if (launchArgs?.PrelaunchActivated != true)
            {
                InitialRootFrameNavigationForCurrentView(rootFrame);
            }
        }

        private Frame InitializeRootFrameForCurrenView()
        {
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

            return rootFrame;
        }

        private void InitialRootFrameNavigationForCurrentView(Frame rootFrame)
        {
            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), windowNumber);
            }

            Window.Current.Activate();
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

            await AppData.GetForCurrentView().SaveAsync();
            deferral.Complete();
        }
    }
}
