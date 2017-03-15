using System;
using UWPHelper.Utilities;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MoreViewsTest
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
                foreach (CoreApplicationView view in CoreApplication.Views)
                {
                    await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ((Frame)Window.Current.Content).RequestedTheme = (ElementTheme)newValue;
                    });
                }
            });
        }
    }
}
