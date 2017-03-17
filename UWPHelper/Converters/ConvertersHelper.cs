namespace UWPHelper.Converters
{
    internal static class ConvertersHelper
    {
        internal static void TryInvertBool(object parameter, ref bool value)
        {
            if (TryParseInvert(parameter))
            {
                value = !value;
            }
        }

        internal static bool TryParseInvert(object parameter)
        {
            return parameter as string == "Invert";
        }
    }
}
