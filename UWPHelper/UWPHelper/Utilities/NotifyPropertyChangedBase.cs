using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UWPHelper.Utilities
{
    public delegate void PropertyChangedAction(object oldValue, object newValue);

    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        private const string PROPERTY_NOT_REGISTERED_EXCEPTION_FORMAT = "There is no registered property called {0}.";

        private Dictionary<string, PropertyData> backingStore;

        public event PropertyChangedEventHandler PropertyChanged;

        protected NotifyPropertyChangedBase()
        {
            backingStore = new Dictionary<string, PropertyData>();
        }

        protected void RegisterProperty(string name, Type type, object defaultValue)
        {
            RegisterProperty(name, type, defaultValue, null);
        }

        protected void RegisterProperty(string name, Type type, object defaultValue, PropertyChangedAction propertyChangedAction)
        {
            const string NULLABLE = "Nullable`1";

            try
            {
                backingStore.Add(name, new PropertyData(type.Name == NULLABLE ? defaultValue : Convert.ChangeType(defaultValue, type), type, propertyChangedAction));
            }
            catch (Exception exception)
            {
                if (backingStore.ContainsKey(name))
                {
                    throw new ArgumentException($"This class already contains property named {name}.");
                }

                throw exception;
            }
        }

        protected object GetValue(string propertyName)
        {
            try
            {
                return backingStore[propertyName].Value;
            }
            catch (Exception exception)
            {
                CheckPropertyName(propertyName);
                throw exception;
            }
        }

        protected void ForceSetValue<T>(string propertyName, ref T newValue)
        {
            SetValue(propertyName, ref newValue, true);
        }

        protected void SetValue<T>(string propertyName, ref T newValue)
        {
            SetValue(propertyName, ref newValue, false);
        }

        private void SetValue<T>(string propertyName, ref T newValue, bool forceSetValue)
        {
            try
            {
                PropertyData propertyData = backingStore[propertyName];

                if (propertyData.Type != typeof(T))
                {
                    throw new ArgumentException($"The type of {nameof(newValue)} is not the same as the type of {propertyName} property.");
                }

                if (!EqualityComparer<T>.Default.Equals((T)propertyData.Value, newValue) || forceSetValue)
                {
                    object oldValue = propertyData.Value;
                    propertyData.Value = newValue;

                    propertyData.PropertyChangedAction?.Invoke(oldValue, newValue);
                    OnPropertyChanged(propertyName);
                }
            }
            catch (Exception exception)
            {
                CheckPropertyName(propertyName);
                throw exception;
            }
        }
        
        private void CheckPropertyName(string propertyName)
        {
            if (!backingStore.ContainsKey(propertyName))
            {
                throw new ArgumentException($"There is no registered property called {propertyName}.");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class PropertyData
        {
            public object Value { get; set; }
            public Type Type { get; }
            public PropertyChangedAction PropertyChangedAction { get; set; }

            public PropertyData(object defaultValue, Type type, PropertyChangedAction propertyChangedAction)
            {
                Value = defaultValue;
                Type  = type;
                PropertyChangedAction = propertyChangedAction;
            }
        }
    }
}
