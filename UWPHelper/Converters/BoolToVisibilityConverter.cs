using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = System.Convert.ToBoolean(value);
            ConvertersHelper.TryInvertBool(parameter, ref boolValue);

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool output = (Visibility)value == Visibility.Visible;
            ConvertersHelper.TryInvertBool(parameter, ref output);

            return output;
        }
    }
}
