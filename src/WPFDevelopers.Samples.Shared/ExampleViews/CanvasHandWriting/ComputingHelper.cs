using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFDevelopers.Samples.ExampleViews.CanvasHandWriting
{
    public static class ComputingHelper
    {
        public static double AngleToRadian(double angle)
        {
            return angle * (Math.PI / 180);
        }
        public static double RadianToAngle(double radian)
        {
            return radian * (180 / Math.PI);
        }

        /// <summary>
        /// 将一个值从一个范围映射到另一个范围
        /// </summary>
        public static double RangeMapping(double inputValue, double enterLowerLimit, double enterUpperLimit, double outputLowerLimit, double OutputUpperLimit, CurveType curveType = CurveType.None)
        {
            var percentage = (enterUpperLimit - inputValue) / (enterUpperLimit - enterLowerLimit);
            switch (curveType)
            {
                case CurveType.Sine:
                    percentage = Math.Sin(percentage);
                    break;
                case CurveType.CoSine:
                    percentage = Math.Cos(percentage);
                    break;
                case CurveType.Tangent:
                    percentage = Math.Tan(percentage);
                    break;
                case CurveType.Cotangent:
                    percentage = Math.Atan(percentage);
                    break;
                default:
                    break;
            }
            double outputValue = OutputUpperLimit - ((OutputUpperLimit - outputLowerLimit) * percentage);

            return outputValue;
        }

        public static string ByteToKB(double _byte)
        {
            List<string> unit = new List<string>() { "B", "KB", "MB", "GB", "TB", "P", "PB" };
            int i = 0;
            while (_byte > 1024)
            {
                _byte /= 1024;
                i++;
            }
            _byte = Math.Round(_byte, 3);//保留三位小数
            return _byte + unit[i];
        }

        /// <summary>
        /// 缩短一个数组，对其进行平均采样
        /// </summary>
        
        public static double[] AverageSampling(double[] sourceArray, int number)
        {
            if (sourceArray.Length <= number)
            {
                return sourceArray;
                //throw new Exception("新的数组必须比原有的要小!");
            }
            double[] arrayList = new double[number];
            double stride = (double)sourceArray.Length / number;
            for (int i = 0, jIndex = 0; i < number; i++, jIndex++)
            {
                double strideIncrement = i * stride;
                strideIncrement = Math.Round(strideIncrement, 6);
                double sum = 0;
                int firstIndex = (int)(strideIncrement);
                double firstDecimal = strideIncrement - firstIndex;

                int tailIndex = (int)(strideIncrement + stride);
                double tailDecimal = (strideIncrement + stride) - tailIndex;

                if (firstDecimal != 0)
                    sum += sourceArray[firstIndex] * (1 - firstDecimal);

                if (tailDecimal != 0 && tailIndex != sourceArray.Length)
                    sum += sourceArray[tailIndex] * (tailDecimal);

                int startIndex = firstDecimal == 0 ? firstIndex : firstIndex + 1;
                int endIndex = tailIndex;

                for (int j = startIndex; j < endIndex; j++)
                    sum += sourceArray[j];

                arrayList[jIndex] = sum / stride;
            }
            return arrayList;
        }
        public static List<Vector2D> AverageSampling(List<Vector2D> sourceArray, int number)
        {
            if (sourceArray.Count <= number - 2)
            {
                return sourceArray;
            }
            double[] x = new double[sourceArray.Count];
            double[] y = new double[sourceArray.Count];
            for (int i = 0; i < sourceArray.Count; i++)
            {
                x[i] = sourceArray[i].X;
                y[i] = sourceArray[i].Y;
            }

            double[] X = AverageSampling(x, number - 2);
            double[] Y = AverageSampling(y, number - 2);

            List<Vector2D> arrayList = new List<Vector2D>();
            for (int i = 0; i < number - 2; i++)
            {
                arrayList.Add(new Vector2D(X[i], Y[i]));
            }

            arrayList.Insert(0, sourceArray[0]);//添加首
            arrayList.Add(sourceArray[sourceArray.Count - 1]);//添加尾

            return arrayList;
        }
    }
    public enum CurveType 
    {
        Sine,
        CoSine,
        Tangent,
        Cotangent,
        None
    }
}
