using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public class NullableBoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}