using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Drawing.Core
{
    public class PolylineData
    {
        public PolylineData(IList<Point> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Count <= 1)
            {
                throw new ArgumentOutOfRangeException("points");
            }
            this.points = points;
        }

        public bool IsClosed => points[0] == points.Last<Point>();

        public int Count => points.Count;

        public double TotalLength
        {
            get
            {
                double? num = totalLength;
                if (num == null)
                    return ComputeTotalLength();

                return num.GetValueOrDefault();
            }
        }

        public IList<Point> Points => points;
       

        public IList<double> Lengths => lengths ?? ComputeLengths();

        public IList<Vector> Normals => normals ?? ComputeNormals();

        public IList<double> Angles => angles ?? ComputeAngles();

        public IList<double> AccumulatedLength => accumulates ?? ComputeAccumulatedLength();

        public Vector Difference(int index)
        {
            int index2 = (index + 1) % Count;
            return points[index2].Subtract(points[index]);
        }

        public Vector SmoothNormal(int index, double fraction, double cornerRadius)
        {
            if (cornerRadius > 0.0)
            {
                double num = Lengths[index];
                if (MathHelper.IsVerySmall(num))
                {
                    int num2 = index - 1;
                    if (num2 < 0 && IsClosed)
                    {
                        num2 = Count - 1;
                    }
                    int num3 = index + 1;
                    if (IsClosed && num3 >= Count - 1)
                    {
                        num3 = 0;
                    }
                    if (num2 < 0 || num3 >= Count)
                    {
                        return Normals[index];
                    }
                    return GeometryHelper.Lerp(Normals[num3], Normals[num2], 0.5).Normalized();
                }
                else
                {
                    double num4 = Math.Min(cornerRadius / num, 0.5);
                    if (fraction <= num4)
                    {
                        int num5 = index - 1;
                        if (IsClosed && num5 == -1)
                        {
                            num5 = Count - 1;
                        }
                        if (num5 >= 0)
                        {
                            double alpha = (num4 - fraction) / (2.0 * num4);
                            return GeometryHelper.Lerp(Normals[index], Normals[num5], alpha).Normalized();
                        }
                    }
                    else if (fraction >= 1.0 - num4)
                    {
                        int num6 = index + 1;
                        if (IsClosed && num6 >= Count - 1)
                        {
                            num6 = 0;
                        }
                        if (num6 < Count)
                        {
                            double alpha2 = (fraction + num4 - 1.0) / (2.0 * num4);
                            return GeometryHelper.Lerp(Normals[index], Normals[num6], alpha2).Normalized();
                        }
                    }
                }
            }
            return Normals[index];
        }

        private IList<double> ComputeLengths()
        {
            lengths = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                lengths[i] = Difference(i).Length;
            }
            return lengths;
        }

        private IList<Vector> ComputeNormals()
        {
            normals = new Vector[points.Count];
            for (int i = 0; i < Count - 1; i++)
            {
                normals[i] = GeometryHelper.Normal(points[i], points[i + 1]);
            }
            normals[Count - 1] = normals[Count - 2];
            return normals;
        }

        private IList<double> ComputeAngles()
        {
            angles = new double[Count];
            for (int i = 1; i < Count - 1; i++)
            {
                angles[i] = -GeometryHelper.Dot(Normals[i - 1], Normals[i]);
            }
            if (IsClosed)
            {
                double value = -GeometryHelper.Dot(Normals[0], Normals[Count - 2]);
                angles[0] = angles[Count - 1] = value;
            }
            else
            {
                angles[0] = angles[Count - 1] = 1.0;
            }
            return angles;
        }

        private IList<double> ComputeAccumulatedLength()
        {
            accumulates = new double[Count];
            accumulates[0] = 0.0;
            for (int i = 1; i < Count; i++)
            {
                accumulates[i] = accumulates[i - 1] + Lengths[i - 1];
            }
            totalLength = new double?(accumulates.Last<double>());
            return accumulates;
        }

        private double ComputeTotalLength()
        {
            ComputeAccumulatedLength();
            return totalLength.Value;
        }

        private IList<Point> points;

        private IList<Vector> normals;

        private IList<double> angles;

        private IList<double> lengths;

        private IList<double> accumulates;

        private double? totalLength;
    }
}
