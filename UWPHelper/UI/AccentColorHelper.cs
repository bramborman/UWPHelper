using System.Collections.Generic;
using System.Threading;
using UWPHelper.Utilities;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWPHelper.UI
{
    // Cannot inherit from ViewSpecificBindableClassBase<AccentColorHelper> since it requires public constructor and since we want to update only one property using this way
    public sealed class AccentColorHelper : NotifyPropertyChangedBase
    {
        private static readonly Dictionary<int, AccentColorHelper> instances = new Dictionary<int, AccentColorHelper>();
        private static readonly object locker = new object();

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
            RegisterProperty(nameof(AccentColor), typeof(Color), new Color(), async (oldValue, newValue) =>
            {
                Color newColor = (Color)newValue;

                AccentColorBrush        = new SolidColorBrush(newColor);
                AccentContrastingTheme  = newColor.GetContrastingTheme();

                AccentColorChanged?.Invoke(this, newColor);

                bool lockTaken = false;

                try
                {
                    Monitor.TryEnter(locker, 0, ref lockTaken);

                    if (lockTaken)
                    {
                        int callerViewId = ViewHelper.GetCurrentViewId();

                        await ViewHelper.RunOnEachViewDispatcherAsync(() =>
                        {
                            int currentViewId = ViewHelper.GetCurrentViewId();

                            if (currentViewId != callerViewId && instances.ContainsKey(currentViewId))
                            {
                                instances[currentViewId].AccentColor = newColor;
                            }
                        });
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(locker);
                    }
                }
            });
            RegisterProperty(nameof(AccentColorBrush), typeof(SolidColorBrush), null);
            RegisterProperty(nameof(AccentContrastingTheme), typeof(ElementTheme), ElementTheme.Default);

            UpdateAccentColor();
            
            ViewHelper.GetCurrentCoreWindow().Activated += (sender, args) =>
            {
                if (args.WindowActivationState != CoreWindowActivationState.Deactivated)
                {
                    UpdateAccentColor();
                }
            };
        }
        
        private void UpdateAccentColor()
        {
            AccentColor = (Color)Application.Current.Resources["SystemAccentColor"];
        }
        
        public static AccentColorHelper GetForCurrentView()
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (!instances.ContainsKey(currentViewId))
            {
                instances.Add(currentViewId, new AccentColorHelper());
            }

            return instances[currentViewId];
        }
    }
}
