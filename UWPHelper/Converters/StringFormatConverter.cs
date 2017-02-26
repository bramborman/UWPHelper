using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    // Sample usage:
    // <TextBlock Text="{x:Bind Slider.Value, Converter={StaticResource StringFormatConverter}, ConverterParameter='{}{0:F0}', Mode=OneWay}"/>
    public sealed class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
#if !ENABLE_STRING_FORMAT_CONVERTER_CONVERT_BACK
            throw new NotImplementedException();
#else
            return System.Convert.ChangeType(value, targetType);
#endif
        }
    }
}
