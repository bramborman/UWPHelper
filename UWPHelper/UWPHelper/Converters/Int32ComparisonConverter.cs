using System;
using Windows.UI.Xaml.Data;

namespace UWPHelper.Converters
{
    public sealed class Int32ComparisonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string[] parameters = ((string)parameter).Split('_');

            if (parameters.Length != 2)
            {
                throw new ArgumentException($"Incorrect formatting of {nameof(parameter)}.", nameof(parameter));
            }

            switch (parameters[0])
            {
                case "Equals":          return System.Convert.ToInt32(value) == (int)double.Parse(parameters[1]);
                case "Greater":         return System.Convert.ToInt32(value)  > (int)double.Parse(parameters[1]);
                case "Lower":           return System.Convert.ToInt32(value)  < (int)double.Parse(parameters[1]);
                case "GreaterOrEquals": return System.Convert.ToInt32(value) >= (int)double.Parse(parameters[1]);
                case "LowerOrEquals":   return System.Convert.ToInt32(value) <= (int)double.Parse(parameters[1]);
                default:                throw new ArgumentException($"Incorrect formatting of {nameof(parameter)}.", nameof(parameter));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
