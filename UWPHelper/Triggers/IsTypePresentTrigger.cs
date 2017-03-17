using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace UWPHelper.Triggers
{
    public sealed class IsTypePresentTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(string), typeof(IsTypePresentTrigger), new PropertyMetadata(null, OnTypePropertyChanged));

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        private static void OnTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsTypePresentTrigger isTypePresentTrigger = (IsTypePresentTrigger)d;
            isTypePresentTrigger.SetActive(ApiInformation.IsTypePresent(isTypePresentTrigger.Type));
        }
    }
}
