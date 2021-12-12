using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFDevelopers.Helpers
{
    public static class SnowCanvasHelper
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
