using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = System.Convert.ToBoolean(value);
            ConvertersHelper.TryInvertBool(parameter, ref boolValue);

            return System.Convert.ToInt32(boolValue);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool output = System.Convert.ToBoolean(value);
            ConvertersHelper.TryInvertBool(parameter, ref output);

            return output;
        }
    }
}
