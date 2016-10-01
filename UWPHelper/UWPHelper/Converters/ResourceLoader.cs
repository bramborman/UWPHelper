using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public class ResourceLoader : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}