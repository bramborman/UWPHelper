using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace UWPHelper.Triggers
{
    public class IsTypePresentTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(string), typeof(IsTypePresentTrigger), new PropertyMetadata(null, OnDeviceFamilyPropertyChanged));

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        private static void OnDeviceFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsTypePresentTrigger isTypePresentTrigger = (IsTypePresentTrigger)d;
            isTypePresentTrigger.SetActive(ApiInformation.IsTypePresent(isTypePresentTrigger.Type));
        }
    }
}
