using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace UWPHelper.Utilities
{
    public static class ViewHelper
    {
        public static CoreWindow GetCurrentCoreWindow()
        {
            return CoreWindow.GetForCurrentThread() ?? throw new Exception($"Unable to get current {nameof(CoreWindow)}. Please make sure you are calling this method on an UI thread.");
        }

        public static int GetCurrentViewId()
        {
            return ApplicationView.GetApplicationViewIdForWindow(GetCurrentCoreWindow());
        }

        public static IAsyncAction RunOnCurrentViewDispatcherAsync(Action action)
        {
            return GetCurrentCoreWindow().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                action();
            });
        }
        
        public static async Task RunOnEachViewDispatcherAsync(Action action)
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
