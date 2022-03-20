using System;

namespace WPFDevelopers.Helpers
{
    public static partial class SnowCanvasHelper
    {
        static SnowCanvasHelper()
        {
            _random = new Random((int)DateTime.Now.Ticks);
        }
        public static double Randomise(double lower, double higher)
        {
            return (lower + (_random.NextDouble() * (higher - lower)));
        }
        static Random _random;
    }
}
