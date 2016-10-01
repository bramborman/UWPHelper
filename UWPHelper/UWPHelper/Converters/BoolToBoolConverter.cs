using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    // Use for binding bool? to bool or reverse
    public class BoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value);
        }
    }
}