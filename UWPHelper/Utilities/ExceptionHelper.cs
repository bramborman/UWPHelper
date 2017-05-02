using System;

namespace UWPHelper.Utilities
{
    public static class ExceptionHelper
    {
        private const string SMALLER_FORMAT      = "Value ({0}) is out of range (smaller than {1}).";
        private const string GREATER_FORMAT      = "Value ({0}) is out of range (greater than {1}).";
        private const string OUT_OF_RANGE_FORMAT = "Value ({0}) is out of range ({1} - {2}).";

        [Obsolete("This method is obsolete and will be removed with version 2.0. Use 'ValidateObjectNotNull' method instead.")]
        public static void ValidateNotNull(object obj, string parameterName)
        {
            ValidateObjectNotNull(obj, parameterName);
        }

        public static void ValidateObjectNotNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [Obsolete("This method is obsolete and will be removed with version 2.0. Use 'ValidateStringNotNullOrWhiteSpace' method instead.")]
        public static void ValidateNotNullOrWhiteSpace(string str, string parameterName)
        {
            ValidateStringNotNullOrWhiteSpace(str, parameterName);
        }
        
        public static void ValidateStringNotNullOrWhiteSpace(string str, string parameterName)
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

        #region long
        public static void ValidateNumberGreaterOrEqual(long value, long min, string parameterName)
        {
            if (value < min)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(SMALLER_FORMAT, value, min));
            }
        }

        public static void ValidateNumberSmallerOrEqual(long value, long max, string parameterName)
        {
            if (value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(GREATER_FORMAT, value, max));
            }
        }

        public static void ValidateNumberInRange(long value, long min, long max, string parameterName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(OUT_OF_RANGE_FORMAT, value, min, max));
            }
        }
        #endregion

        #region ulong
        public static void ValidateNumberGreaterOrEqual(ulong value, ulong min, string parameterName)
        {
            if (value < min)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(SMALLER_FORMAT, value, min));
            }
        }

        public static void ValidateNumberSmallerOrEqual(ulong value, ulong max, string parameterName)
        {
            if (value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(GREATER_FORMAT, value, max));
            }
        }

        public static void ValidateNumberInRange(ulong value, ulong min, ulong max, string parameterName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(OUT_OF_RANGE_FORMAT, value, min, max));
            }
        }
        #endregion

        #region double
        public static void ValidateNumberGreaterOrEqual(double value, double min, string parameterName)
        {
            if (value < min)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(SMALLER_FORMAT, value, min));
            }
        }

        public static void ValidateNumberSmallerOrEqual(double value, double max, string parameterName)
        {
            if (value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(GREATER_FORMAT, value, max));
            }
        }

        public static void ValidateNumberInRange(double value, double min, double max, string parameterName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(OUT_OF_RANGE_FORMAT, value, min, max));
            }
        }
        #endregion

        #region TimeSpan
        public static void ValidateTimeSpanGreaterOrEqual(TimeSpan value, TimeSpan min, string parameterName)
        {
            if (value < min)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(SMALLER_FORMAT, value, min));
            }
        }

        public static void ValidateTimeSpanSmallerOrEqual(TimeSpan value, TimeSpan max, string parameterName)
        {
            if (value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(GREATER_FORMAT, value, max));
            }
        }

        public static void ValidateTimeSpanInRange(TimeSpan value, TimeSpan min, TimeSpan max, string parameterName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, string.Format(OUT_OF_RANGE_FORMAT, value, min, max));
            }
        }
        #endregion
    }
}
