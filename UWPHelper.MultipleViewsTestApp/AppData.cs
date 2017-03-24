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
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ElementTheme.Default, (sender, e) =>
            {
                ((Frame)Window.Current.Content).RequestedTheme = (ElementTheme)e.NewValue;
            });
        }

        public static AppData GetForCurrentView()
        {
            return BaseGetForCurrentView();
        }
    }
}
