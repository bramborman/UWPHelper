using NotifyPropertyChangedBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace UWPHelper.Utilities
{
    // When you change something here, change it even it the ViewSpecificClassBase
    // If these class get renamed, modify its names in the UWPHelper.rd.xml file too
    public abstract class ViewSpecificBindableClassBase<T> : NotifyPropertyChanged where T : ViewSpecificBindableClassBase<T>, new()
    {
        private static readonly Dictionary<int, T> instances = new Dictionary<int, T>();
        private static readonly object syncRoot = new object();

        protected static int InstancesCount
        {
            get
            {
                return instances.Count;
            }
        }

        protected static event PropertyChangedEventHandler MainPropertyChanged;

        protected ViewSpecificBindableClassBase()
        {
            PropertyChanged += async (sender, e) =>
            {
                bool lockTaken = false;

                try
                {
                    Monitor.TryEnter(syncRoot, 0, ref lockTaken);

                    if (lockTaken)
                    {
                        int callerViewId = ViewHelper.GetCurrentViewId();

                        await ViewHelper.RunOnEachViewDispatcherAsync(() =>
                        {
                            if (ViewHelper.GetCurrentViewId() != callerViewId && BaseTryGetForCurrentView(out T currentViewInstance))
                            {
                                PropertyInfo changedProperty = typeof(T).GetRuntimeProperty(e.PropertyName);

                                if (changedProperty.CanRead && changedProperty.CanWrite)
                                {
                                    changedProperty.SetValue(currentViewInstance, changedProperty.GetValue(this));
                                }
                            }
                        });

                        MainPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(syncRoot);
                    }
                }
            };
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

        protected static bool BaseTryGetForCurrentView(out T obj)
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
            return BaseGetForCurrentView(() => new T());
        }

        protected static T BaseGetForCurrentView(Func<T> newInstanceDataGetter)
        {
            int currentViewId = ViewHelper.GetCurrentViewId();

            if (!instances.ContainsKey(currentViewId))
            {
                instances.Add(currentViewId, newInstanceDataGetter());
            }

            return instances[currentViewId];
        }
    }
}
