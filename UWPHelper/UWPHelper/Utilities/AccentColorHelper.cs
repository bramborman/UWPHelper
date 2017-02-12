using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public sealed class AccentColorHelper : NotifyPropertyChangedBase
    {
        private static AccentColorHelper _currentInternal;
        private static AccentColorHelper _current;

        internal static AccentColorHelper CurrentInternal
        {
            get
            {
                if (_currentInternal == null)
                {
                    _currentInternal = new AccentColorHelper();
                }

                return _currentInternal;
            }
        }

        public static AccentColorHelper Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new AccentColorHelper();
                }

                return _current;
            }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(); }
            set { SetValue(ref value); }
        }
        public Color AccentColor
        {
            get
            {
                ValidateActivation();
                return (Color)GetValue();
            }
            private set
            {
                ValidateActivation();
                SetValue(ref value);
            }
        }
        public ElementTheme AccentContrastingTheme
        {
            get
            {
                ValidateActivation();
                return (ElementTheme)GetValue();
            }
            private set
            {
                ValidateActivation();
                SetValue(ref value);
            }
        }
        
        // Prevent from creating new instances
        private AccentColorHelper()
        {
            RegisterProperty(nameof(IsActive), typeof(bool), false, (oldValue, newValue) =>
            {
                GetAccentColor();

                if ((bool)newValue)
                {
                    Window.Current.Activated += Window_Activated;
                }
                else
                {
                    Window.Current.Activated -= Window_Activated;
                }
            });
            RegisterProperty(nameof(AccentColor), typeof(Color), new Color(), (oldValue, newValue) =>
            {
                float luma = ((Color)newValue).GetLuma();
                AccentContrastingTheme = luma < 0.4869937f || luma == 0.541142f ? ElementTheme.Dark : ElementTheme.Light;
            });
            RegisterProperty(nameof(AccentContrastingTheme), typeof(ElementTheme), ElementTheme.Default);
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                GetAccentColor();
            }
        }

        private void GetAccentColor()
        {
            AccentColor = (Color)Application.Current.Resources["SystemAccentColor"];;
        }

        private void ValidateActivation()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException($"Cannot use this class when it's not activated. You must set the {nameof(AccentColorHelper)}.{nameof(IsActive)} property value to true.");
            }
        }
    }
}
