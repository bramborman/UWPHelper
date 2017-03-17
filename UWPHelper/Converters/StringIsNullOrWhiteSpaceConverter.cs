using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class StringIsNullOrWhiteSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool output = string.IsNullOrWhiteSpace((string)value);
            ConvertersHelper.TryInvertBool(parameter, ref output);

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
