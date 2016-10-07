using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UWPHelper.Utilities
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        const string PROPERTY_NOT_REGISTERED_EXCEPTION_FORMAT = "There is no registered property called {0}.";

        Dictionary<string, PropertyInfo> backingStore;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RegisterProperty(string name, Type type, object defaultValue)
        {
            RegisterProperty(name, type, defaultValue, null);
        }

        protected void RegisterProperty(string name, Type type, object defaultValue, Action onValueChangedAction)
        {
            try
            {
                if (backingStore == null)
                {
                    backingStore = new Dictionary<string, PropertyInfo>();
                }

                backingStore.Add(name, new PropertyInfo(type.Name == "Nullable`1" ? defaultValue : Convert.ChangeType(defaultValue, type), type, onValueChangedAction));
            }
            catch (Exception ex)
            {
                if (backingStore?.ContainsKey(name) == true)
                {
                    throw new ArgumentException($"This class already contains property named {name}.");
                }

                throw ex;
            }
        }

        protected T GetValue<T>(string propertyName)
        {
            try
            {
                return (T)backingStore[propertyName].Value;
            }
            catch (Exception ex)
            {
                CheckPropertyName(propertyName);
                throw ex;
            }
        }

        protected void SetValue<T>(string propertyName, ref T newValue)
        {
            try
            {
                if (backingStore[propertyName].Type != typeof(T))
                {
                    throw new ArgumentException($"The type of {nameof(newValue)} is not the same as the type of {propertyName}.");
                }

                if (!EqualityComparer<T>.Default.Equals((T)backingStore[propertyName].Value, newValue))
                {
                    backingStore[propertyName].Value = newValue;
                    backingStore[propertyName].OnValueChangedAction?.Invoke();

                    OnPropertyChanged(propertyName);
                }
            }
            catch (Exception ex)
            {
                CheckPropertyName(propertyName);
                throw ex;
            }
        }

        protected void ChangeOnValueChangedAction(string propertyName, Action onValueChangedAction)
        {
            try
            {
                backingStore[propertyName].OnValueChangedAction = onValueChangedAction;
            }
            catch (Exception ex)
            {
                CheckPropertyName(propertyName);
                throw ex;
            }
        }

        private void CheckPropertyName(string propertyName)
        {
            if (backingStore?.ContainsKey(propertyName) != true)
            {
                throw new ArgumentException($"There is no registered property called {propertyName}.");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class PropertyInfo
        {
            public object Value { get; set; }
            public Type Type { get;}
            public Action OnValueChangedAction { get; set; }

            public PropertyInfo(object defaultValue, Type type, Action onValueChangedAction)
            {
                Value = defaultValue;
                Type  = type;
                OnValueChangedAction = onValueChangedAction;
            }
        }
    }
}