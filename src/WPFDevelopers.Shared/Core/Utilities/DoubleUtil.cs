using System;
using System.Runtime.InteropServices;

namespace WPFDevelopers.Utilities
{
    public static class DoubleUtil
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
        internal const float FLT_MIN = 1.175494351e-38F; /* Number close to zero, where float.MinValue is -float.MaxValue */


        public static bool IsZero(double value) => Math.Abs(value) < 10.0 * DBL_EPSILON;

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2) return true;
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        public static bool LessThan(double value1, double value2) => (value1 < value2) && !AreClose(value1, value2);

        public static bool GreaterThan(double value1, double value2) => (value1 > value2) && !AreClose(value1, value2);

        public static bool LessThanOrClose(double value1, double value2) => (value1 < value2) || AreClose(value1, value2);

        public static bool GreaterThanOrClose(double value1, double value2) => (value1 > value2) || AreClose(value1, value2);

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal UInt64 UintValue;
        }

        public static bool IsNaN(double value)
        {
            NanUnion t = new NanUnion();
            t.DoubleValue = value;

            UInt64 exp = t.UintValue & 0xfff0000000000000;
            UInt64 man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }

        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * DBL_EPSILON;
        }
    }
}
