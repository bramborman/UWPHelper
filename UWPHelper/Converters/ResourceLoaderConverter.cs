using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class ResourceLoaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string[] parameters = ((string)parameter).Split('|');

            switch (parameters.Length)
            {
                case 1:  return ResourceLoader.GetForCurrentView().GetString(parameters[0]);
                case 2:  return ResourceLoader.GetForCurrentView(parameters[0]).GetString(parameters[1]);
                default: throw new ArgumentException($"Incorrect formatting of {nameof(parameter)}.", nameof(parameter));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
