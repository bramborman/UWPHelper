using Windows.System.Profile;
using Windows.UI.Xaml;

namespace UWPHelper.Triggers
{
    public sealed class DeviceFamilyTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register(nameof(DeviceFamily), typeof(string), typeof(DeviceFamilyTrigger), new PropertyMetadata(null, OnDeviceFamilyPropertyChanged));

        public string DeviceFamily
        {
            get { return (string)GetValue(DeviceFamilyProperty); }
            set { SetValue(DeviceFamilyProperty, value); }
        }

        private static void OnDeviceFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DeviceFamilyTrigger deviceFamilyTrigger = ((DeviceFamilyTrigger)d);
            deviceFamilyTrigger.SetActive(deviceFamilyTrigger.DeviceFamily == AnalyticsInfo.VersionInfo.DeviceFamily);
        }
    }
}
