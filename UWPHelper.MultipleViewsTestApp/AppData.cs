using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UWPHelper.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPHelper.MultipleViewsTestApp
{
    public sealed class AppData : NotifyPropertyChangedBase
    {
        private static readonly object locker = new object();
        private static readonly Dictionary<int, AppData> appData = new Dictionary<int, AppData>();

        public ElementTheme Theme
        {
            get { return (ElementTheme)GetValue(); }
            set { SetValue(value); }
        }
        
        private AppData()
        {
            RegisterProperty(nameof(Theme), typeof(ElementTheme), ElementTheme.Default, (oldValue, newValue) =>
            {
                ((Frame)Window.Current.Content).RequestedTheme = (ElementTheme)newValue;
            });

            PropertyChanged += async (sender, e) =>
            {
                bool lockTaken = false;

                try
                {
                    Monitor.TryEnter(locker, 0, ref lockTaken);

                    if (lockTaken)
                    {
                        int callerViewId = ViewHelper.GetCurrentViewId();

                        await ViewHelper.RunOnEachViewDispatcherAsync(() =>
                        {
                            int currentViewId = ViewHelper.GetCurrentViewId();

                            if (currentViewId != callerViewId && appData.ContainsKey(currentViewId))
                            {
                                AppData currentViewAppData   = appData[currentViewId];
                                PropertyInfo changedProperty = typeof(AppData).GetRuntimeProperty(e.PropertyName);
                                
                                if (changedProperty.CanRead && changedProperty.CanWrite)
                                {
                                    changedProperty.SetValue(currentViewAppData, changedProperty.GetValue(this));
                                }
                            }
                        });
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(locker);
                    }
                }
            };
        }

        public static AppData GetForCurrentView()
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (!appData.ContainsKey(currentViewId))
            {
                appData.Add(currentViewId, new AppData());
            }

            return appData[currentViewId];
        }
    }
}
