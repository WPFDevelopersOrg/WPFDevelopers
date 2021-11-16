using System;

namespace Microsoft.Expression.Drawing.Core
{
    public class RandomEngine
    {
        public RandomEngine(long seed)
        {
            Initialize(seed);
        }

        public double NextGaussian(double mean, double variance)
        {
            return Gaussian() * variance + mean;
        }

        public double NextUniform(double min, double max)
        {
            return Uniform() * (max - min) + min;
        }

        private void Initialize(long seed)
        {
            random = new Random((int)seed);
        }

        private double Uniform()
        {
            return random.NextDouble();
        }

        private double Gaussian()
        {
            if (anotherSample != null)
            {
                double value = anotherSample.Value;
                anotherSample = null;
                return value;
            }
            double num;
            double num2;
            double num3;
            do
            {
                num = 2.0 * Uniform() - 1.0;
                num2 = 2.0 * Uniform() - 1.0;
                num3 = num * num + num2 * num2;
            }
            while (num3 >= 1.0);
            double num4 = Math.Sqrt(-2.0 * Math.Log(num3) / num3);
            anotherSample = new double?(num * num4);
            return num2 * num4;
        }

        private Random random;

        private double? anotherSample;
    }
}
