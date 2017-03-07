using System;
using System.Collections.Generic;
using UWPHelper.Utilities;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public sealed class AccentColorHelper : NotifyPropertyChangedBase
    {
        private static readonly Dictionary<int, AccentColorHelper> accentColorHelpers = new Dictionary<int, AccentColorHelper>();
        
        private WeakReference<CoreWindow> CoreWindowReference { get; set; }
        
        public Color AccentColor
        {
            get { return (Color)GetValue(); }
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
                float luma = newColor.GetLuma();
                // Don't know why these values, but with these it works as Windows itself does...
                AccentContrastingTheme = luma < 0.4869937f || luma == 0.541142f ? ElementTheme.Dark : ElementTheme.Light;

                AccentColorChanged?.Invoke(this, newColor);
            });
            RegisterProperty(nameof(AccentContrastingTheme), typeof(ElementTheme), ElementTheme.Default);

            UpdateAccentColor();

            if (CoreWindowReference.TryGetTarget(out CoreWindow currentCoreWindow))
            {
                currentCoreWindow.Activated += (sender, args) =>
                {
                    if (args.WindowActivationState != CoreWindowActivationState.Deactivated)
                    {
                        UpdateAccentColor();
                    }
                };
            }
            else
            {
                throw new Exception($"Unable to initialize {nameof(AccentColorHelper)} for current view.");
            }
        }
        
        private void UpdateAccentColor()
        {
            AccentColor = (Color)Application.Current.Resources["SystemAccentColor"];
        }
        
        public static AccentColorHelper GetForCurrentView()
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (!accentColorHelpers.ContainsKey(currentViewId))
            {
                accentColorHelpers.Add(currentViewId, new AccentColorHelper
                {
                    CoreWindowReference = new WeakReference<CoreWindow>(ViewHelper.GetCurrentCoreWindow())
                });
            }

            return accentColorHelpers[currentViewId];
        }
    }
}
