using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UWPHelper.Utilities
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        const string PROPERTY_NOT_REGISTERED_EXCEPTION_FORMAT = "There is no registered property called {0}.";

        static Dictionary<PropertyIdentifier, PropertyData> backingStore;

        public event PropertyChangedEventHandler PropertyChanged;

        static NotifyPropertyChangedBase()
        {
            backingStore = new Dictionary<PropertyIdentifier, PropertyData>();
        }

        protected void RegisterProperty(string name, Type type, object defaultValue)
        {
            RegisterProperty(name, type, defaultValue, null);
        }

        protected void RegisterProperty(string name, Type type, object defaultValue, Action onValueChangedAction)
        {
            PropertyIdentifier propertyIdentifier = new PropertyIdentifier(this, name);

            try
            {
                backingStore.Add(propertyIdentifier, new PropertyData(type.Name == "Nullable`1" ? defaultValue : Convert.ChangeType(defaultValue, type), type, onValueChangedAction));
            }
            catch (Exception ex)
            {
                if (backingStore.ContainsKey(propertyIdentifier))
                {
                    throw new ArgumentException($"This class already contains property named {name}.");
                }

                throw ex;
            }
        }

        protected T GetValue<T>(string propertyName)
        {
            PropertyIdentifier propertyIdentifier = new PropertyIdentifier(this, propertyName);

            try
            {
                return (T)backingStore[propertyIdentifier].Value;
            }
            catch (Exception ex)
            {
                CheckPropertyName(propertyIdentifier);
                throw ex;
            }
        }

        protected void SetValue<T>(string propertyName, ref T newValue)
        {
            PropertyIdentifier propertyIdentifier = new PropertyIdentifier(this, propertyName);

            try
            {
                PropertyData propertyData = backingStore[propertyIdentifier];

                if (propertyData.Type != typeof(T))
                {
                    throw new ArgumentException($"The type of {nameof(newValue)} is not the same as the type of {propertyName}.");
                }

                if (!EqualityComparer<T>.Default.Equals((T)backingStore[propertyIdentifier].Value, newValue))
                {
                    propertyData.Value = newValue;
                    propertyData.OnValueChangedAction?.Invoke();

                    OnPropertyChanged(propertyName);
                }
            }
            catch (Exception ex)
            {
                CheckPropertyName(propertyIdentifier);
                throw ex;
            }
        }

        protected void ChangeOnValueChangedAction(string propertyName, Action onValueChangedAction)
        {
            PropertyIdentifier propertyIdentifier = new PropertyIdentifier(this, propertyName);

            try
            {
                backingStore[propertyIdentifier].OnValueChangedAction = onValueChangedAction;
            }
            catch (Exception ex)
            {
                CheckPropertyName(propertyIdentifier);
                throw ex;
            }
        }

        private void CheckPropertyName(PropertyIdentifier propertyIdentifier)
        {
            if (!backingStore.ContainsKey(propertyIdentifier))
            {
                throw new ArgumentException($"There is no registered property called {propertyIdentifier.PropertyName}.");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class PropertyIdentifier
        {
            public object Instance { get; }
            public string PropertyName { get; }

            public PropertyIdentifier(object instance, string propertyName)
            {
                Instance = instance;
                PropertyName = propertyName;
            }

            public override bool Equals(object obj)
            {
                PropertyIdentifier propertyIdentifier = (PropertyIdentifier)obj;
                return ReferenceEquals(Instance, propertyIdentifier.Instance) && PropertyName == propertyIdentifier.PropertyName;
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(Instance);
            }
        }

        private class PropertyData
        {
            public object Value { get; set; }
            public Type Type { get; }
            public Action OnValueChangedAction { get; set; }

            public PropertyData(object defaultValue, Type type, Action onValueChangedAction)
            {
                Value = defaultValue;
                Type  = type;
                OnValueChangedAction = onValueChangedAction;
            }
        }
    }
}