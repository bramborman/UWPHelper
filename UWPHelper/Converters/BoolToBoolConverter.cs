using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class BoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool output = System.Convert.ToBoolean(value);
            ConvertersHelper.TryInvertBool(parameter, ref output);

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool output = System.Convert.ToBoolean(value);
            ConvertersHelper.TryInvertBool(parameter, ref output);

            return output;
        }
    }
}
