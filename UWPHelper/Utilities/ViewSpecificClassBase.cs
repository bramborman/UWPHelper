using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace UWPHelper.Utilities
{
    // When you change something here, change it even it the ViewSpecificBindableClassBase
    // If these class get renamed, modify its names in the UWPHelper.rd.xml file too
    public abstract class ViewSpecificClassBase<T> where T : ViewSpecificClassBase<T>, new()
    {
        private static readonly Dictionary<int, T> instances = new Dictionary<int, T>();

        private static object locker;

        protected static event PropertyChangedEventHandler MainPropertyChanged;

        protected ViewSpecificClassBase()
        {
            if (this is INotifyPropertyChanged iNotifyPropertyChanged)
            {
                locker = new object();

                iNotifyPropertyChanged.PropertyChanged += async (sender, e) =>
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
                                if (ViewHelper.GetCurrentViewId() != callerViewId && BaseGetForCurrentViewIfExists(out T currentViewInstance))
                                {
                                    PropertyInfo changedProperty = typeof(T).GetRuntimeProperty(e.PropertyName);

                                    if (changedProperty.CanRead && changedProperty.CanWrite)
                                    {
                                        changedProperty.SetValue(currentViewInstance, changedProperty.GetValue(this));
                                    }
                                }
                            });
                        }

                        MainPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
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
        }

        protected static void BaseSetForCurrentView(T obj)
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (!instances.ContainsKey(currentViewId))
            {
                instances.Add(currentViewId, null);
            }

            instances[currentViewId] = obj;
        }

        protected static bool BaseGetForCurrentViewIfExists(out T obj)
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (instances.ContainsKey(currentViewId))
            {
                obj = instances[currentViewId];
                return true;
            }

            obj = null;
            return false;
        }

        protected static T BaseGetForCurrentView()
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (!instances.ContainsKey(currentViewId))
            {
                instances.Add(currentViewId, new T());
            }

            return instances[currentViewId];
        }
    }
}
