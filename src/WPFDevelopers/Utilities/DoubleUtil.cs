using System;

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
    }
}
