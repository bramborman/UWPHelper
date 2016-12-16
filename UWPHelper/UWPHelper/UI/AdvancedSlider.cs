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
        public static readonly DependencyProperty ValueChangedDelayMilisecondsProperty = DependencyProperty.Register(nameof(ValueChangedDelayMiliseconds), typeof(double), typeof(AdvancedSlider), new PropertyMetadata(0.0, OnValueChangedDelayMilisecondsPropertyChanged));

        public event AdvancedSliderValueChangedDelayedEventHandler ValueChangedDelayed;
        
        private double oldValueDelayed;
        private Delayer delayer;

        public bool IsValueChangedDelayedEnabled
        {
            get { return (bool)GetValue(IsValueChangedDelayedEnabledProperty); }
            set { SetValue(IsValueChangedDelayedEnabledProperty, value); }
        }
        public double ValueChangedDelayMiliseconds
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
            OnValueChangedDelayed(null);
        }

        private void OnValueChangedDelayed(Delayer obj)
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
                advancedSlider.delayer         = new Delayer(advancedSlider.ValueChangedDelayMiliseconds);
                advancedSlider.delayer.Tick   += advancedSlider.OnValueChangedDelayed;
            }
            else
            {
                advancedSlider.delayer.Tick -= advancedSlider.OnValueChangedDelayed;
                advancedSlider.delayer = null;
            }
        }
        
        private static void OnValueChangedDelayMilisecondsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdvancedSlider advancedSlider = (AdvancedSlider)d;

            if ((double)e.NewValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(advancedSlider.ValueChangedDelayMiliseconds));
            }

            if (advancedSlider.IsValueChangedDelayedEnabled)
            {
                advancedSlider.delayer.Delay = TimeSpan.FromMilliseconds(advancedSlider.ValueChangedDelayMiliseconds);
            }
        }
    }
}
