using System;
using UWPHelper.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public delegate void AdvancedSliderValueChangedDelayedEventHandler(object sender, AdvancedSliderValueChangedDelayedEventArgs e);

    public sealed class AdvancedSliderValueChangedDelayedEventArgs
    {
        public double OldValue { get; }
        public double NewValue { get; }

        public AdvancedSliderValueChangedDelayedEventArgs(double oldValue, double newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class AdvancedSlider : Slider
    {
        public static readonly DependencyProperty IsValueChangedDelayedEnabledProperty = DependencyProperty.Register(nameof(IsValueChangedDelayedEnabled), typeof(bool), typeof(AdvancedSlider), new PropertyMetadata(false, OnIsValueChangedDelayEnabledPropertyChanged));
        public static readonly DependencyProperty ValueChangedDelayMilisecondsProperty = DependencyProperty.Register(nameof(ValueChangedDelayMilliseconds), typeof(double), typeof(AdvancedSlider), new PropertyMetadata(0.0, OnValueChangedDelayMillisecondsPropertyChanged));

        public event AdvancedSliderValueChangedDelayedEventHandler ValueChangedDelayed;
        
        private double oldValueDelayed;
        private Debouncer delayer;

        public bool IsValueChangedDelayedEnabled
        {
            get { return (bool)GetValue(IsValueChangedDelayedEnabledProperty); }
            set { SetValue(IsValueChangedDelayedEnabledProperty, value); }
        }
        public double ValueChangedDelayMilliseconds
        {
            get { return (double)GetValue(ValueChangedDelayMilisecondsProperty); }
            set { SetValue(ValueChangedDelayMilisecondsProperty, value); }
        }

        public AdvancedSlider()
        {
            oldValueDelayed = Value;
        }

        public void ForceRaiseValueChanged()
        {
            delayer.Stop();
            OnValueChangedDelayed(null, null);
        }

        private void OnValueChangedDelayed(object sender, EventArgs e)
        {
            ValueChangedDelayed?.Invoke(this, new AdvancedSliderValueChangedDelayedEventArgs(oldValueDelayed, Value));
            oldValueDelayed = Value;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (IsValueChangedDelayedEnabled)
            {
                delayer?.Reset();
            }

            base.OnValueChanged(oldValue, newValue);
        }

        private static void OnIsValueChangedDelayEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdvancedSlider advancedSlider = (AdvancedSlider)d;

            if ((bool)e.NewValue)
            {
                advancedSlider.oldValueDelayed = advancedSlider.Value;
                advancedSlider.delayer         = new Debouncer(TimeSpan.FromMilliseconds(advancedSlider.ValueChangedDelayMilliseconds));
                advancedSlider.delayer.Tick   += advancedSlider.OnValueChangedDelayed;
            }
            else
            {
                advancedSlider.delayer.Tick -= advancedSlider.OnValueChangedDelayed;
                advancedSlider.delayer = null;
            }
        }
        
        private static void OnValueChangedDelayMillisecondsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExceptionHelper.ValidateIsGreaterOrEqual((double)e.NewValue, 0, nameof(ValueChangedDelayMilliseconds));
            AdvancedSlider advancedSlider = (AdvancedSlider)d;

            if (advancedSlider.IsValueChangedDelayedEnabled)
            {
                advancedSlider.delayer.Delay = TimeSpan.FromMilliseconds(advancedSlider.ValueChangedDelayMilliseconds);
            }
        }
    }
}
