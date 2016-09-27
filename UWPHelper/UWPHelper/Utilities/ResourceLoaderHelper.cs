using Windows.ApplicationModel.Resources;

namespace UWPHelper.Utilities
{
    public static class ResourceLoaderHelper
    {
        public static string GetString(string key)
        {
            return ResourceLoader.GetForCurrentView().GetString(key);
        }

        public static string GetString(string fileName, string key)
        {
            return ResourceLoader.GetForCurrentView(fileName).GetString(key);
        }
    }
}