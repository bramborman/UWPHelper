using UWPHelper.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.MultipleViewsTestApp
{
    public sealed class AppData : ViewSpecificBindableClassBase<AppData>
    {
        public ElementTheme Theme
        {
            get { return (ElementTheme)GetValue(); }
            set { SetValue(value); }
        }
        
        public AppData()
        {
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ElementTheme.Default, (oldValue, newValue) =>
            {
                ((Frame)Window.Current.Content).RequestedTheme = (ElementTheme)newValue;
            });
        }

        public static new AppData GetForCurrentView()
        {
            return ViewSpecificBindableClassBase<AppData>.GetForCurrentView();
        }
    }
}
