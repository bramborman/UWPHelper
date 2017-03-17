using UWPHelper.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.MultipleViewsTestApp
{
    public sealed class AppData : NotifyPropertyChangedBase
    {
        public static AppData Current { get; private set; }

        public ElementTheme Theme
        {
            get { return (ElementTheme)GetValue(); }
            set { SetValue(value); }
        }

        static AppData()
        {
            Current = new AppData();
        }

        private AppData()
        {
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ElementTheme.Default, async (oldValue, newValue) =>
            {
                await ViewHelper.RunOnEachViewDispatcherAsync(() => ((Frame)Window.Current.Content).RequestedTheme = (ElementTheme)newValue);
            });
        }
    }
}
