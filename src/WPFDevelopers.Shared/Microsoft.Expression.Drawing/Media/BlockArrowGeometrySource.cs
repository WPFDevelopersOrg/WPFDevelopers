using Microsoft.Expression.Drawing.Core;
using System;
using System.Windows;

namespace Microsoft.Expression.Drawing.Media
{
    internal class BlockArrowGeometrySource : GeometrySource<IBlockArrowGeometrySourceParameters>
    {
        protected override bool UpdateCachedGeometry(IBlockArrowGeometrySourceParameters parameters)
        {
            bool flag = false;
            ArrowBuilder builder = GetBuilder(parameters.Orientation);
            double num = builder.ArrowLength(LogicalBounds);
            double num2 = builder.ArrowWidth(LogicalBounds);
            double num3 = num2 / 2.0 / num;
            double num4 = MathHelper.EnsureRange(parameters.ArrowheadAngle, new double?(0.0), new double?(180.0));
            double num5 = Math.Tan(num4 * 3.141592653589793 / 180.0 / 2.0);
            if (num5 < num3)
            {
                EnsurePoints(3);
                points[0] = builder.ComputePointA(num, num2);
                points[1] = builder.ComputePointB(num, num);
                points[2] = builder.GetMirrorPoint(points[1], num2);
            }
            else
            {
                double offset = num2 / 2.0 / num5;
                double num6 = MathHelper.EnsureRange(parameters.ArrowBodySize, new double?(0.0), new double?(1.0));
                double thickness = num2 / 2.0 * (1.0 - num6);
                EnsurePoints(7);
                points[0] = builder.ComputePointA(num, num2);
                points[1] = builder.ComputePointB(num, offset);
                Tuple<Point, Point> tuple = builder.ComputePointCD(num, offset, thickness);
                points[2] = tuple.Item1;
                points[3] = tuple.Item2;
                points[4] = builder.GetMirrorPoint(points[3], num2);
                points[5] = builder.GetMirrorPoint(points[2], num2);
                points[6] = builder.GetMirrorPoint(points[1], num2);
            }
            for (int i = 0; i < points.Length; i++)
            {
                Point[] array = points;
                int num7 = i;
                array[num7].X = array[num7].X + LogicalBounds.Left;
                Point[] array2 = points;
                int num8 = i;
                array2[num8].Y = array2[num8].Y + LogicalBounds.Top;
            }
            return flag | PathGeometryHelper.SyncPolylineGeometry(ref cachedGeometry, points, true);
        }

        private static ArrowBuilder GetBuilder(ArrowOrientation orientation)
        {
            switch (orientation)
            {
                case ArrowOrientation.Left:
                    return new LeftArrowBuilder();
                case ArrowOrientation.Up:
                    return new UpArrowBuilder();
                case ArrowOrientation.Down:
                    return new DownArrowBuilder();
            }
            return new RightArrowBuilder();
        }

        private void EnsurePoints(int count)
        {
            if (points == null || points.Length != count)
            {
                points = new Point[count];
            }
        }

        private Point[] points;

        private abstract class ArrowBuilder
        {
            public abstract double ArrowLength(Rect bounds);

            public abstract double ArrowWidth(Rect bounds);

            public abstract Point GetMirrorPoint(Point point, double width);

            public abstract Point ComputePointA(double length, double width);

            public abstract Point ComputePointB(double length, double offset);

            public abstract Tuple<Point, Point> ComputePointCD(double length, double offset, double thickness);
        }

        private abstract class HorizontalArrowBuilder : ArrowBuilder
        {
            public override double ArrowLength(Rect bounds)
            {
                return bounds.Width;
            }

            public override double ArrowWidth(Rect bounds)
            {
                return bounds.Height;
            }

            public override Point GetMirrorPoint(Point point, double width)
            {
                return new Point(point.X, width - point.Y);
            }
        }

        private abstract class VerticalArrowBuilder : ArrowBuilder
        {
            public override double ArrowLength(Rect bounds)
            {
                return bounds.Height;
            }

            public override double ArrowWidth(Rect bounds)
            {
                return bounds.Width;
            }

            public override Point GetMirrorPoint(Point point, double width)
            {
                return new Point(width - point.X, point.Y);
            }
        }

        private class LeftArrowBuilder : HorizontalArrowBuilder
        {
            public override Point ComputePointA(double length, double width)
            {
                return new Point(0.0, width / 2.0);
            }

            public override Point ComputePointB(double length, double offset)
            {
                return new Point(offset, 0.0);
            }

            public override Tuple<Point, Point> ComputePointCD(double length, double offset, double thickness)
            {
                return new Tuple<Point, Point>(new Point(offset, thickness), new Point(length, thickness));
            }
        }

        private class RightArrowBuilder : HorizontalArrowBuilder
        {
            public override Point ComputePointA(double length, double width)
            {
                return new Point(length, width / 2.0);
            }

            public override Point ComputePointB(double length, double offset)
            {
                return new Point(length - offset, 0.0);
            }

            public override Tuple<Point, Point> ComputePointCD(double length, double offset, double thickness)
            {
                return new Tuple<Point, Point>(new Point(length - offset, thickness), new Point(0.0, thickness));
            }
        }

        private class UpArrowBuilder : VerticalArrowBuilder
        {
            public override Point ComputePointA(double length, double width)
            {
                return new Point(width / 2.0, 0.0);
            }

            public override Point ComputePointB(double length, double offset)
            {
                return new Point(0.0, offset);
            }

            public override Tuple<Point, Point> ComputePointCD(double length, double offset, double thickness)
            {
                return new Tuple<Point, Point>(new Point(thickness, offset), new Point(thickness, length));
            }
        }

        private class DownArrowBuilder : VerticalArrowBuilder
        {
            public override Point ComputePointA(double length, double width)
            {
                return new Point(width / 2.0, length);
            }

            public override Point ComputePointB(double length, double offset)
            {
                return new Point(0.0, length - offset);
            }

            public override Tuple<Point, Point> ComputePointCD(double length, double offset, double thickness)
            {
                return new Tuple<Point, Point>(new Point(thickness, length - offset), new Point(thickness, 0.0));
            }
        }
    }
}
