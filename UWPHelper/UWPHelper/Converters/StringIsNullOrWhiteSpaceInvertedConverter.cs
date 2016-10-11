using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class StringIsNullOrWhiteSpaceInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !string.IsNullOrWhiteSpace((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}