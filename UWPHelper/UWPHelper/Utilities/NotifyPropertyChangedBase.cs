using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UWPHelper.Utilities
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        private Dictionary<string, PropertyInfo> backingStore;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RegisterProperty(string name, Type type, object defaultValue)
        {
            RegisterProperty(name, type, defaultValue, null);
        }

        protected void RegisterProperty(string name, Type type, object defaultValue, Action onValueChangedAction)
        {
            if (backingStore == null)
            {
                backingStore = new Dictionary<string, PropertyInfo>();
            }
            else if (backingStore.ContainsKey(name))
            {
                throw new ArgumentException($"This class already contains property named {name}.");
            }

            backingStore.Add(name, new PropertyInfo(defaultValue, type, onValueChangedAction));
        }

        protected void ChangeOnChangedAction(string propertyName, Action onValueChangedAction)
        {
            CheckPropertyName(propertyName);
            backingStore[propertyName].OnValueChangedAction = onValueChangedAction;
        }

        protected T GetValue<T>(string propertyName)
        {
            CheckPropertyName(propertyName);
            return (T)backingStore[propertyName].Value;
        }

        protected void SetValue<T>(string propertyName, ref T newValue)
        {
            CheckPropertyName(propertyName);

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

        private void CheckPropertyName(string propertyName)
        {
            if (backingStore?.ContainsKey(propertyName) != true)
            {
                throw new ArgumentException($"There is no such property called {propertyName}.");
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