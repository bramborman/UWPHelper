using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace UWPHelper.Utilities
{
    internal static class ViewHelper
    {
        internal static CoreWindow GetCurrentCoreWindow()
        {
            return CoreWindow.GetForCurrentThread() ?? throw new Exception($"Unable to get current {nameof(CoreWindow)}. Please make sure you are calling this method on an UI thread.");
        }

        internal static int GetCurrentViewId()
        {
            return ApplicationView.GetApplicationViewIdForWindow(GetCurrentCoreWindow());
        }

        internal static void RunOnCurrentViewDispatcher(Action action)
        {
            Task.Run(async () => await RunOnCurrentViewDispatcherAsync(action));
        }

        internal static IAsyncAction RunOnCurrentViewDispatcherAsync(Action action)
        {
            return GetCurrentCoreWindow().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                action();
            });
        }

        internal static void RunOnEachViewDispatcher(Action action)
        {
            Task.Run(async () => await RunOnEachViewDispatcherAsync(action));
        }

        internal static async Task RunOnEachViewDispatcherAsync(Action action)
        {
            foreach (CoreApplicationView view in CoreApplication.Views)
            {
                await view.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    action();
                });
            }
        }
    }
}
