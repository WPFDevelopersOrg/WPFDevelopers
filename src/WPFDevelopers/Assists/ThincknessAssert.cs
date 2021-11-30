using System.Windows;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Assists
{
    public static class ThincknessAssert
    {
        public static bool IsZero(this Thickness thickness)
        {
            return DoubleUtil.IsZero(thickness.Left)
                   && DoubleUtil.IsZero(thickness.Top)
                   && DoubleUtil.IsZero(thickness.Right)
                   && DoubleUtil.IsZero(thickness.Bottom);
        }

        public static bool IsUniform(this Thickness thickness)
        {
            return DoubleUtil.AreClose(thickness.Left, thickness.Top)
                   && DoubleUtil.AreClose(thickness.Left, thickness.Right)
                   && DoubleUtil.AreClose(thickness.Left, thickness.Bottom);
        }

    }
}
