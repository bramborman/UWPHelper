using System;
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
    }
}
