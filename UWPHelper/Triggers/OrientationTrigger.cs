using UWPHelper.Utilities;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace UWPHelper.Triggers
{
    public enum Orientation
    {
        None,
        Portrait,
        Landscape
    }

    public sealed class OrientationTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(OrientationTrigger), new PropertyMetadata(null, OnOrientationPropertyChanged));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public OrientationTrigger()
        {
            DisplayInformation.GetForCurrentView().OrientationChanged += (sender, args) =>
            {
                UpdateTrigger(sender.CurrentOrientation);
            };
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExceptionHelper.ValidateEnumValueDefined((Orientation)e.NewValue, nameof(Orientation));
            ((OrientationTrigger)d).UpdateTrigger(DisplayInformation.GetForCurrentView().CurrentOrientation);
        }

        private void UpdateTrigger(DisplayOrientations currentOrientation)
        {
            switch (currentOrientation)
            {
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    SetActive(Orientation == Orientation.Portrait);
                    break;
                
                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    SetActive(Orientation == Orientation.Landscape);
                    break;
                
                case DisplayOrientations.None:
                    SetActive(Orientation == Orientation.None);
                    break;
            }
        }
    }
}
