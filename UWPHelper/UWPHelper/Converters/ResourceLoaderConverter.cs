using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class ResourceLoaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ResourceLoader.GetForCurrentView().GetString((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
