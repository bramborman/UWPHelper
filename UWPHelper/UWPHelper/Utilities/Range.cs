using System;

namespace UWPHelper.Utilities
{
    public class Range : NotifyPropertyChangedBase, IEquatable<Range>
    {
        public int Min
        {
            get { return (int)GetValue(nameof(Min)); }
            set
            {
                CheckValues(value, Max);
                SetValue(nameof(Min), ref value);
            }
        }
        public int Max
        {
            get { return (int)GetValue(nameof(Max)); }
            set
            {
                CheckValues(Min, value);
                SetValue(nameof(Max), ref value);
            }
        }

        public Range() : this(0, 0)
        {

        }

        public Range(int min, int max)
        {
            CheckValues(min, max);

            RegisterProperty(nameof(Min), typeof(int), min);
            RegisterProperty(nameof(Max), typeof(int), max);
        }

        private void CheckValues(int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException($"{nameof(Max)} value must be greater than {nameof(Min)} value.");
            }
        }

        public bool ContainsNumber(int number)
        {
            return number >= Min && number <= Max;
        }

        public override int GetHashCode()
        {
            return Min ^ (Max * 17);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Range))
            {
                return false;
            }

            return this == (Range)obj;
        }

        public bool Equals(Range other)
        {
            return this == other;
        }

        public static bool operator ==(Range r1, Range r2)
        {
            if (ReferenceEquals(r1, r2))
            {
                return true;
            }

            if ((object)r1 == null || (object)r2 == null)
            {
                return false;
            }

            return r1.Min == r2.Min && r1.Max == r2.Max;
        }

        public static bool operator !=(Range r1, Range r2)
        {
            return !(r1 == r2);
        }
    }
}
