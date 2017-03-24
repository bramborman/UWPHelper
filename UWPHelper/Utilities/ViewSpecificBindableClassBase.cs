using NotifyPropertyChangedBase;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace UWPHelper.Utilities
{
    // When you change something here, change it even it the ViewSpecificClassBase
    // If these class get renamed, modify its names in the UWPHelper.rd.xml file too
    public abstract class ViewSpecificBindableClassBase<T> : NotifyPropertyChanged where T : ViewSpecificBindableClassBase<T>, new()
    {
        private static readonly Dictionary<int, T> instances = new Dictionary<int, T>();
        private static readonly object locker = new object();

        protected ViewSpecificBindableClassBase()
        {
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

                            if (currentViewId != callerViewId && instances.ContainsKey(currentViewId))
                            {
                                T currentViewInstance = instances[currentViewId];
                                PropertyInfo changedProperty = typeof(T).GetRuntimeProperty(e.PropertyName);

                                if (changedProperty.CanRead && changedProperty.CanWrite)
                                {
                                    changedProperty.SetValue(currentViewInstance, changedProperty.GetValue(this));
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
