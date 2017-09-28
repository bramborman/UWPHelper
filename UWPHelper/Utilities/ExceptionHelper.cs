using System;
using System.Collections.Generic;
using System.Linq;

namespace UWPHelper.Utilities
{
    public static class ExceptionHelper
    {
        public static void ValidateObjectNotNull(object obj, string parameterName = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ValidateIEnumerableNotEmpty<T>(IEnumerable<T> enumerable, string parameterName = null)
        {
            if (!enumerable.Any())
            {
                throw new ArgumentException("Collection cannot be empty.", parameterName);
            }
        }

        public static void ValidateIEnumerableNotNullOrEmpty<T>(IEnumerable<T> enumerable, string parameterName = null)
        {
            if (enumerable?.Any() != true)
            {
                throw new ArgumentException("Collection cannot be null or empty.", parameterName);
            }
        }

        public static void ValidateStringNotNullOrEmpty(string str, string parameterName = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("Value cannot be null or empty.", parameterName);
            }
        }

        public static void ValidateStringNotNullOrWhiteSpace(string str, string parameterName = null)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Value cannot be null or white space.", parameterName);
            }
        }

        public static void ValidateEnumValueDefined(Enum enumValue, string parameterName = null)
        {
            if (!Enum.IsDefined(enumValue.GetType(), enumValue))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void ValidateIsGreaterOrEqual<T>(T value, T min, string parameterName = null) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"Value ({value}) is out of range (smaller than {min}).");
            }
        }

        public static void ValidateIsSmallerOrEqual<T>(T value, T max, string parameterName = null) where T : IComparable<T>
        {
            if (value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"Value ({value}) is out of range (greater than {max}).");
            }
        }

        public static void ValidateIsInRange<T>(T value, T min, T max, string parameterName = null) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"Value ({value}) is out of range (smaller than {min} or greater than {max}).");
            }
        }
    }
}
