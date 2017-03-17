using System;

namespace UWPHelper.Utilities
{
    public static class ExceptionHelper
    {
        public static void ValidateNotNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ValidateNotNullOrWhiteSpace(string str, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Value cannot be white space or null.", parameterName);
            }
        }

        public static void ValidateEnumValueDefined(Enum enumValue, string parameterName)
        {
            if (!Enum.IsDefined(enumValue.GetType(), enumValue))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
