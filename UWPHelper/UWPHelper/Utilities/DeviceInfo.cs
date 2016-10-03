using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace UWPHelper.Utilities
{
    // Base code taken from https://www.suchan.cz/2015/08/uwp-quick-tip-getting-device-os-and-app-info/
    public static class DeviceInfo
    {
        public static string SystemFamily { get; }
        public static string SystemVersion { get; }
        public static string PackageArchitecture { get; }
        public static string DeviceManufacturer { get; }
        public static string DeviceModel { get; }

        static DeviceInfo()
        {
            SystemFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            
            ulong v = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            ulong major     = (v & 0xFFFF000000000000L) >> 48;
            ulong minor     = (v & 0x0000FFFF00000000L) >> 32;
            ulong build     = (v & 0x00000000FFFF0000L) >> 16;
            ulong revision  = (v & 0x000000000000FFFFL);
            SystemVersion = $"{major}.{minor}.{build}.{revision}";
            
            PackageArchitecture = Package.Current.Id.Architecture.ToString();
            
            EasClientDeviceInformation easClientDeviceInformation = new EasClientDeviceInformation();
            DeviceManufacturer  = easClientDeviceInformation.SystemManufacturer;
            DeviceModel         = easClientDeviceInformation.SystemProductName;
        }
    }
}