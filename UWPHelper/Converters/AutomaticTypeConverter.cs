using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class AutomaticTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ChangeType(value, targetType);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ChangeType(value, targetType);
        }
    }
}
