using System;
using Windows.Foundation;
using Windows.System;

namespace UWPHelper.Utilities
{
    public static class LauncherHelper
    {
        public static IAsyncOperation<bool> LaunchAsUriAsync(this string str)
        {
            return Launcher.LaunchUriAsync(new Uri(str));
        }
    }
}