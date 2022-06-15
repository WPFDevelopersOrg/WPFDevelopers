using System;

namespace WPFDevelopers.Helpers
{
    public static class SnowCanvasHelper
    {
        private static readonly Random _random;

        static SnowCanvasHelper()
        {
            _random = new Random((int)DateTime.Now.Ticks);
        }

        public static double Randomise(double lower, double higher)
        {
            return lower + _random.NextDouble() * (higher - lower);
        }
    }
}