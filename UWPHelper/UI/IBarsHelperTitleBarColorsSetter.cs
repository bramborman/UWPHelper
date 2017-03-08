using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPHelper.UI
{
    public interface IBarsHelperTitleBarColorsSetter
    {
        void SetTitleBarColors(ApplicationViewTitleBar titleBar, ElementTheme requestedTheme);
    }
}
