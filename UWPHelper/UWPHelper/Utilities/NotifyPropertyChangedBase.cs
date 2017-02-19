using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UWPHelper.Utilities
{
    public delegate void PropertyChangedAction(object oldValue, object newValue);

    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, PropertyData> backingStore = new Dictionary<string, PropertyData>();

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void RegisterProperty(string name, Type type, object defaultValue)
        {
            RegisterProperty(name, type, defaultValue, null);
        }

        protected void RegisterProperty(string name, Type type, object defaultValue, PropertyChangedAction propertyChangedAction)
        {
            ValidateName(name);

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            try
            {
                backingStore.Add(name, new PropertyData(type.Name == "Nullable`1" ? defaultValue : Convert.ChangeType(defaultValue, type), type, propertyChangedAction));
            }
            catch (Exception exception)
            {
                if (backingStore.ContainsKey(name))
                {
                    throw new ArgumentException($"This class already contains registered property named {name}.");
                }

                throw exception;
            }
        }

        protected object GetValue([CallerMemberName]string propertyName = null)
        {
            try
            {
                return backingStore[propertyName].Value;
            }
            catch (Exception exception)
            {
                ValidateName(propertyName);
                ValidatePropertyName(propertyName);

                throw exception;
            }
        }

        protected void ForceSetValue<T>(T newValue, [CallerMemberName]string propertyName = null)
        {
            SetValue(newValue, propertyName, true);
        }

        protected void SetValue<T>(T newValue, [CallerMemberName]string propertyName = null)
        {
            SetValue(newValue, propertyName, false);
        }

        private void SetValue<T>(T newValue, string propertyName, bool forceSetValue)
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
                ValidateName(propertyName);
                ValidatePropertyName(propertyName);

                throw exception;
            }
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be empty or null.", nameof(name));
            }
        }
        
        private void ValidatePropertyName(string propertyName)
        {
            if (!backingStore.ContainsKey(propertyName))
            {
                throw new ArgumentException($"There is no registered property called {propertyName}.");
            }
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            ValidateName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class PropertyData
        {
            internal object Value { get; set; }
            internal Type Type { get; }
            internal PropertyChangedAction PropertyChangedAction { get; }

            internal PropertyData(object defaultValue, Type type, PropertyChangedAction propertyChangedAction)
            {
                Value = defaultValue;
                Type  = type;
                PropertyChangedAction = propertyChangedAction;
            }
        }
    }
}
