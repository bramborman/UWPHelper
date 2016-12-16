using System;
using UWPHelper.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.UI
{
    public class AdvancedSlider : Slider
    {
        public static readonly DependencyProperty IsValueChangedDelayEnabledProperty    = DependencyProperty.Register(nameof(IsValueChangedDelayEnabled), typeof(bool), typeof(AdvancedSlider), new PropertyMetadata(false, OnIsValueChangedDelayEnabledPropertyChanged));
        public static readonly DependencyProperty ValueChangedDelayMilisecondsProperty  = DependencyProperty.Register(nameof(ValueChangedDelayMiliseconds), typeof(double), typeof(AdvancedSlider), new PropertyMetadata(0.0, OnValueChangedDelayMilisecondsPropertyChanged));
        
        private double oldValue;
        private Delayer delayer;

        public bool IsValueChangedDelayEnabled
        {
            get { return (bool)GetValue(IsValueChangedDelayEnabledProperty); }
            set { SetValue(IsValueChangedDelayEnabledProperty, value); }
        }
        public double ValueChangedDelayMiliseconds
        {
            get { return (double)GetValue(ValueChangedDelayMilisecondsProperty); }
            set { SetValue(ValueChangedDelayMilisecondsProperty, value); }
        }

        public AdvancedSlider()
        {
            long valuePropertyChangedCallbackToken = 0;

            valuePropertyChangedCallbackToken = RegisterPropertyChangedCallback(ValueProperty, (d, dp) =>
            {
                // Get the initial Value
                oldValue = Value;
                UnregisterPropertyChangedCallback(ValueProperty, valuePropertyChangedCallbackToken);
            });
        }

        public void ForceRaiseValueChanged()
        {
            delayer.Stop();
            Delayer_Tick(null);
        }

        private void Delayer_Tick(Delayer obj)
        {
            base.OnValueChanged(oldValue, Value);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (!IsValueChangedDelayEnabled)
            {
                base.OnValueChanged(oldValue, newValue);
            }
            else
            {
                delayer.Reset();
            }
        }

        private static void OnIsValueChangedDelayEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdvancedSlider advancedSlider = (AdvancedSlider)d;

            if ((bool)e.NewValue)
            {
                advancedSlider.delayer = new Delayer(advancedSlider.ValueChangedDelayMiliseconds);
                advancedSlider.delayer.Tick += advancedSlider.Delayer_Tick;
            }
            else
            {
                advancedSlider.delayer.Tick -= advancedSlider.Delayer_Tick;
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

            if (advancedSlider.IsValueChangedDelayEnabled)
            {
                advancedSlider.delayer.Delay = TimeSpan.FromMilliseconds(advancedSlider.ValueChangedDelayMiliseconds);
            }
        }
    }
}
