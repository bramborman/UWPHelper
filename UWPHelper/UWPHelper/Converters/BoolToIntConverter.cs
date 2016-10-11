using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToInt32(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value);
        }
    }
}