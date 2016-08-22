using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace UWPHelper.Triggers
{
    public class IsApiContractPresentTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register(nameof(ContractName), typeof(string), typeof(IsApiContractPresentTrigger), new PropertyMetadata(null, OnPropertyChanged));
        public static readonly DependencyProperty MajorVersionProperty = DependencyProperty.Register(nameof(MajorVersion), typeof(int), typeof(IsApiContractPresentTrigger), new PropertyMetadata(null, OnPropertyChanged));
        public static readonly DependencyProperty MinorVersionProperty = DependencyProperty.Register(nameof(MinorVersion), typeof(int), typeof(IsApiContractPresentTrigger), new PropertyMetadata(null, OnPropertyChanged));
        
        public string ContractName
        {
            get { return (string)GetValue(ContractNameProperty); }
            set { SetValue(ContractNameProperty, value); }
        }

        public int MajorVersion
        {
            get { return (int)GetValue(MajorVersionProperty); }
            set { SetValue(MajorVersionProperty, value); }
        }

        public int MinorVersion
        {
            get { return (int)GetValue(MinorVersionProperty); }
            set { SetValue(MinorVersionProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsApiContractPresentTrigger isApiContractPresentTrigger = (IsApiContractPresentTrigger)d;

            if (isApiContractPresentTrigger.ReadLocalValue(MinorVersionProperty) == DependencyProperty.UnsetValue)
            {
                isApiContractPresentTrigger.SetActive(ApiInformation.IsApiContractPresent(isApiContractPresentTrigger.ContractName, (ushort)isApiContractPresentTrigger.MajorVersion));
            }
            else
            {
                isApiContractPresentTrigger.SetActive(ApiInformation.IsApiContractPresent(isApiContractPresentTrigger.ContractName, (ushort)isApiContractPresentTrigger.MajorVersion, (ushort)isApiContractPresentTrigger.MinorVersion));
            }
        }
    }
}