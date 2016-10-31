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
            ((OrientationTrigger)d).UpdateTrigger(DisplayInformation.GetForCurrentView().CurrentOrientation);
        }

        private void UpdateTrigger(DisplayOrientations currentOrientation)
        {
            if (currentOrientation == DisplayOrientations.Portrait || currentOrientation == DisplayOrientations.PortraitFlipped)
            {
                SetActive(Orientation == Orientation.Portrait);
            }
            else if (currentOrientation == DisplayOrientations.Landscape || currentOrientation == DisplayOrientations.LandscapeFlipped)
            {
                SetActive(Orientation == Orientation.Landscape);
            }
            else// if (currentOrientation == DisplayOrientations.None)
            {
                SetActive(Orientation == Orientation.None);
            }
        }
    }
}
