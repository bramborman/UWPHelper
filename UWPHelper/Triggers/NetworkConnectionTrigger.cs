using System;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;

namespace UWPHelper.Triggers
{
    public sealed class NetworkConnectionTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty NetworkConnectivityLevelProperty = DependencyProperty.Register(nameof(NetworkConnectivityLevel), typeof(NetworkConnectivityLevel), typeof(NetworkConnectionTrigger), new PropertyMetadata(null, OnNetworkConnectivityLevelPropertyChanged));

        public NetworkConnectivityLevel NetworkConnectivityLevel
        {
            get { return (NetworkConnectivityLevel)GetValue(NetworkConnectivityLevelProperty); }
            set { SetValue(NetworkConnectivityLevelProperty, value); }
        }

        public NetworkConnectionTrigger()
        {
            NetworkInformation.NetworkStatusChanged += sender =>
            {
                UpdateTrigger();
            };
        }

        private static void OnNetworkConnectivityLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Enum.IsDefined(typeof(NetworkConnectivityLevel), e.NewValue))
            {
                throw new ArgumentOutOfRangeException(nameof(NetworkConnectivityLevel));
            }

            ((NetworkConnectionTrigger)d).UpdateTrigger();
        }

        private void UpdateTrigger()
        {
            NetworkConnectivityLevel networkConnectivityLevel = NetworkConnectivityLevel.None;

            if (NetworkInformation.GetInternetConnectionProfile() is ConnectionProfile connectionProfile)
            {
                networkConnectivityLevel = connectionProfile.GetNetworkConnectivityLevel();
            }

            SetActive(NetworkConnectivityLevel == networkConnectivityLevel);
        }
    }
}
