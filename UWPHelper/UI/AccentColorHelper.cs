using System.Collections.Generic;
using UWPHelper.Utilities;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWPHelper.UI
{
    public sealed class AccentColorHelper : NotifyPropertyChangedBase
    {
        private static readonly Dictionary<int, AccentColorHelper> accentColorHelpers = new Dictionary<int, AccentColorHelper>();
        
        public Color AccentColor
        {
            get { return (Color)GetValue(); }
            private set { SetValue(value); }
        }
        public SolidColorBrush AccentColorBrush
        {
            get { return (SolidColorBrush)GetValue(); }
            private set { SetValue(value); }
        }
        public ElementTheme AccentContrastingTheme
        {
            get { return (ElementTheme)GetValue(); }
            private set { SetValue(value); }
        }

        public event TypedEventHandler<AccentColorHelper, Color> AccentColorChanged;

        // Prevent from creating new instances
        private AccentColorHelper()
        {
            RegisterProperty(nameof(AccentColor), typeof(Color), new Color(), (oldValue, newValue) =>
            {
                Color newColor = (Color)newValue;

                AccentColorBrush        = new SolidColorBrush(newColor);
                AccentContrastingTheme  = newColor.GetContrastingTheme();

                AccentColorChanged?.Invoke(this, newColor);
            });
            RegisterProperty(nameof(AccentColorBrush), typeof(SolidColorBrush), null);
            RegisterProperty(nameof(AccentContrastingTheme), typeof(ElementTheme), ElementTheme.Default);

            UpdateAccentColorAsync();
            
            ViewHelper.GetCurrentCoreWindow().Activated += (sender, args) =>
            {
                if (args.WindowActivationState != CoreWindowActivationState.Deactivated)
                {
                    UpdateAccentColorAsync();
                }
            };
        }
        
        private async void UpdateAccentColorAsync()
        {
            Color newAccentColor = (Color)Application.Current.Resources["SystemAccentColor"];

            await ViewHelper.RunOnEachViewDispatcherAsync(() =>
            {
                int currentViewId = ViewHelper.GetCurrentViewId();

                if (accentColorHelpers.ContainsKey(currentViewId))
                {
                    accentColorHelpers[currentViewId].AccentColor = newAccentColor;
                }
            });
        }
        
        public static AccentColorHelper GetForCurrentView()
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (!accentColorHelpers.ContainsKey(currentViewId))
            {
                accentColorHelpers.Add(currentViewId, new AccentColorHelper());
            }

            return accentColorHelpers[currentViewId];
        }
    }
}
