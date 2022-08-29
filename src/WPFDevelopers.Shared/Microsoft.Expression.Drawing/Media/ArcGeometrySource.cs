using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Media
{
    internal class ArcGeometrySource : GeometrySource<IArcGeometrySourceParameters>
    {
        protected override Rect ComputeLogicalBounds(Rect layoutBounds, IGeometrySourceParameters parameters)
        {
            Rect logicalBound = base.ComputeLogicalBounds(layoutBounds, parameters);
            return GeometryHelper.GetStretchBound(logicalBound, parameters.Stretch, new Size(1.0, 1.0));
        }

        protected override bool UpdateCachedGeometry(IArcGeometrySourceParameters parameters)
        {
            bool flag = false;
            NormalizeThickness(parameters);
            bool relativeMode = parameters.ArcThicknessUnit == UnitType.Percent;
            bool flag2 = MathHelper.AreClose(parameters.StartAngle, parameters.EndAngle);
            double num = NormalizeAngle(parameters.StartAngle);
            double num2 = NormalizeAngle(parameters.EndAngle);
            if (num2 < num)
            {
                num2 += 360.0;
            }
            bool flag3 = relativeThickness == 1.0;
            bool flag4 = relativeThickness == 0.0;
            if (flag2)
            {
                flag |= UpdateZeroAngleGeometry(relativeMode, num);
            }
            else if (MathHelper.IsVerySmall((num2 - num) % 360.0))
            {
                if (flag4 || flag3)
                {
                    flag |= UpdateEllipseGeometry(flag3);
                }
                else
                {
                    flag |= UpdateFullRingGeometry(relativeMode);
                }
            }
            else if (flag3)
            {
                flag |= UpdatePieGeometry(num, num2);
            }
            else if (flag4)
            {
                flag |= UpdateOpenArcGeometry(num, num2);
            }
            else
            {
                flag |= UpdateRingArcGeometry(relativeMode, num, num2);
            }
            return flag;
        }

        private void NormalizeThickness(IArcGeometrySourceParameters parameters)
        {
            double val = LogicalBounds.Width / 2.0;
            double val2 = LogicalBounds.Height / 2.0;
            double num = Math.Min(val, val2);
            double num2 = parameters.ArcThickness;
            if (parameters.ArcThicknessUnit == UnitType.Pixel)
            {
                num2 = MathHelper.SafeDivide(num2, num, 0.0);
            }
            relativeThickness = MathHelper.EnsureRange(num2, new double?(0.0), new double?(1.0));
            absoluteThickness = num * relativeThickness;
        }

        private bool UpdateZeroAngleGeometry(bool relativeMode, double angle)
        {
            bool flag = false;
            Point arcPoint = GeometryHelper.GetArcPoint(angle, LogicalBounds);
            Rect logicalBounds = LogicalBounds;
            double num = logicalBounds.Width / 2.0;
            double num2 = logicalBounds.Height / 2.0;
            Point point;
            if (relativeMode || MathHelper.AreClose(num, num2))
            {
                Rect bound = LogicalBounds.Resize(1.0 - relativeThickness);
                point = GeometryHelper.GetArcPoint(angle, bound);
            }
            else
            {
                double intersect = InnerCurveSelfIntersect(num, num2, absoluteThickness);
                double[] array = ComputeAngleRanges(num, num2, intersect, angle, angle);
                double num3 = array[0] * 3.141592653589793 / 180.0;
                Vector vector = new Vector(num2 * Math.Sin(num3), -num * Math.Cos(num3));
                point = GeometryHelper.GetArcPoint(array[0], LogicalBounds) - vector.Normalized() * absoluteThickness;
            }
            LineGeometry dependencyObject;
            flag |= GeometryHelper.EnsureGeometryType<LineGeometry>(out dependencyObject, ref cachedGeometry, () => new LineGeometry());
            flag |= dependencyObject.SetIfDifferent(LineGeometry.StartPointProperty, arcPoint);
            return flag | dependencyObject.SetIfDifferent(LineGeometry.EndPointProperty, point);
        }

        private bool UpdateEllipseGeometry(bool isFilled)
        {
            bool flag = false;
            double y = MathHelper.Lerp(LogicalBounds.Top, LogicalBounds.Bottom, 0.5);
            Point point = new Point(LogicalBounds.Left, y);
            Point point2 = new Point(LogicalBounds.Right, y);
            PathGeometry pathGeometry;
            flag |= GeometryHelper.EnsureGeometryType<PathGeometry>(out pathGeometry, ref cachedGeometry, () => new PathGeometry());
            flag |= pathGeometry.Figures.EnsureListCount(1, () => new PathFigure());
            PathFigure pathFigure = pathGeometry.Figures[0];
            flag |= pathFigure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            flag |= pathFigure.SetIfDifferent(PathFigure.IsFilledProperty, isFilled);
            flag |= pathFigure.Segments.EnsureListCount(2, () => new ArcSegment());
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, point);
            ArcSegment dependencyObject;
            flag |= GeometryHelper.EnsureSegmentType<ArcSegment>(out dependencyObject, pathFigure.Segments, 0, () => new ArcSegment());
            ArcSegment dependencyObject2;
            flag |= GeometryHelper.EnsureSegmentType<ArcSegment>(out dependencyObject2, pathFigure.Segments, 1, () => new ArcSegment());
            Size size = new Size(LogicalBounds.Width / 2.0, LogicalBounds.Height / 2.0);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.SizeProperty, size);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.PointProperty, point2);
            flag |= dependencyObject2.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            flag |= dependencyObject2.SetIfDifferent(ArcSegment.SizeProperty, size);
            flag |= dependencyObject2.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            return flag | dependencyObject2.SetIfDifferent(ArcSegment.PointProperty, point);
        }

        private bool UpdateFullRingGeometry(bool relativeMode)
        {
            bool flag = false;
            PathGeometry pathGeometry;
            flag |= GeometryHelper.EnsureGeometryType<PathGeometry>(out pathGeometry, ref cachedGeometry, () => new PathGeometry());
            flag |= pathGeometry.SetIfDifferent(PathGeometry.FillRuleProperty, FillRule.EvenOdd);
            flag |= pathGeometry.Figures.EnsureListCount(2, () => new PathFigure());
            flag |= PathFigureHelper.SyncEllipseFigure(pathGeometry.Figures[0], LogicalBounds, SweepDirection.Clockwise, true);
            Rect logicalBounds = LogicalBounds;
            double num = logicalBounds.Width / 2.0;
            double num2 = logicalBounds.Height / 2.0;
            if (relativeMode || MathHelper.AreClose(num, num2))
            {
                Rect bounds = LogicalBounds.Resize(1.0 - relativeThickness);
                flag |= PathFigureHelper.SyncEllipseFigure(pathGeometry.Figures[1], bounds, SweepDirection.Counterclockwise, true);
            }
            else
            {
                flag |= pathGeometry.Figures[1].SetIfDifferent(PathFigure.IsClosedProperty, true);
                flag |= pathGeometry.Figures[1].SetIfDifferent(PathFigure.IsFilledProperty, true);
                Point point = default(Point);
                double intersect = InnerCurveSelfIntersect(num, num2, absoluteThickness);
                double[] angles = ComputeAngleRanges(num, num2, intersect, 360.0, 0.0);
                flag |= SyncPieceWiseInnerCurves(pathGeometry.Figures[1], 0, ref point, angles);
                flag |= pathGeometry.Figures[1].SetIfDifferent(PathFigure.StartPointProperty, point);
            }
            return flag;
        }

        private static void IncreaseDuplicatedIndex(IList<double> values, ref int index)
        {
            while (index < values.Count - 1 && values[index] == values[index + 1])
            {
                index++;
            }
        }

        private static void DecreaseDuplicatedIndex(IList<double> values, ref int index)
        {
            while (index > 0 && values[index] == values[index - 1])
            {
                index--;
            }
        }

        internal static double[] ComputeAngleRanges(double radiusX, double radiusY, double intersect, double start, double end)
        {
            List<double> list = new List<double>
            {
                start,
                end,
                intersect,
                180.0 - intersect,
                180.0 + intersect,
                360.0 - intersect,
                360.0 + intersect,
                540.0 - intersect,
                540.0 + intersect,
                720.0 - intersect
            };
            list.Sort();
            int num = list.IndexOf(start);
            int num2 = list.IndexOf(end);
            if (num2 == num)
            {
                num2++;
            }
            else if (start < end)
            {
                IncreaseDuplicatedIndex(list, ref num);
                DecreaseDuplicatedIndex(list, ref num2);
            }
            else if (start > end)
            {
                DecreaseDuplicatedIndex(list, ref num);
                IncreaseDuplicatedIndex(list, ref num2);
            }
            List<double> list2 = new List<double>();
            if (num < num2)
            {
                for (int i = num; i <= num2; i++)
                {
                    list2.Add(list[i]);
                }
            }
            else
            {
                for (int j = num; j >= num2; j--)
                {
                    list2.Add(list[j]);
                }
            }
            double num3 = EnsureFirstQuadrant((list2[0] + list2[1]) / 2.0);
            if ((radiusX < radiusY && num3 < intersect) || (radiusX > radiusY && num3 > intersect))
            {
                list2.RemoveAt(0);
            }
            if (list2.Count % 2 == 1)
            {
                list2.RemoveLast<double>();
            }
            if (list2.Count == 0)
            {
                int num4 = Math.Min(num, num2) - 1;
                if (num4 < 0)
                {
                    num4 = Math.Max(num, num2) + 1;
                }
                list2.Add(list[num4]);
                list2.Add(list[num4]);
            }
            return list2.ToArray();
        }

        internal static double EnsureFirstQuadrant(double angle)
        {
            angle = Math.Abs(angle % 180.0);
            if (angle <= 90.0)
            {
                return angle;
            }
            return 180.0 - angle;
        }

        private bool UpdatePieGeometry(double start, double end)
        {
            bool flag = false;
            PathGeometry pathGeometry = cachedGeometry as PathGeometry;
            PathFigure pathFigure;
            ArcSegment arcSegment;
            LineSegment dependencyObject;
            if (pathGeometry == null || pathGeometry.Figures.Count != 1 || (pathFigure = pathGeometry.Figures[0]).Segments.Count != 2 || (arcSegment = (pathFigure.Segments[0] as ArcSegment)) == null || (dependencyObject = (pathFigure.Segments[1] as LineSegment)) == null)
            {
                var vPathGeometry = new PathGeometry();
                cachedGeometry = vPathGeometry;
                vPathGeometry.Figures.Add(pathFigure = new PathFigure());
                pathFigure.Segments.Add(arcSegment = new ArcSegment());
                pathFigure.Segments.Add(dependencyObject = new LineSegment());
                pathFigure.IsClosed = true;
                arcSegment.SweepDirection = SweepDirection.Clockwise;
                flag = true;
            }
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, GeometryHelper.GetArcPoint(start, LogicalBounds));
            flag |= arcSegment.SetIfDifferent(ArcSegment.PointProperty, GeometryHelper.GetArcPoint(end, LogicalBounds));
            flag |= arcSegment.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(LogicalBounds));
            flag |= arcSegment.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0);
            return flag | dependencyObject.SetIfDifferent(LineSegment.PointProperty, LogicalBounds.Center());
        }

        private bool UpdateOpenArcGeometry(double start, double end)
        {
            bool flag = false;
            PathGeometry pathGeometry = cachedGeometry as PathGeometry;
            PathFigure pathFigure;
            ArcSegment arcSegment;
            if (pathGeometry == null || pathGeometry.Figures.Count != 1 || (pathFigure = pathGeometry.Figures[0]).Segments.Count != 1 || (arcSegment = pathFigure.Segments[0] as ArcSegment) == null)
            {
                var vPathGeometry = new PathGeometry();
                cachedGeometry = vPathGeometry;
                vPathGeometry.Figures.Add(pathFigure = new PathFigure());
                pathFigure.Segments.Add(arcSegment = new ArcSegment());
                pathFigure.IsClosed = false;
                arcSegment.SweepDirection = SweepDirection.Clockwise;
                flag = true;
            }
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, GeometryHelper.GetArcPoint(start, LogicalBounds));
            flag |= pathFigure.SetIfDifferent(PathFigure.IsFilledProperty, false);
            flag |= arcSegment.SetIfDifferent(ArcSegment.PointProperty, GeometryHelper.GetArcPoint(end, LogicalBounds));
            flag |= arcSegment.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(LogicalBounds));
            return flag | arcSegment.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0);
        }

        private bool UpdateRingArcGeometry(bool relativeMode, double start, double end)
        {
            bool flag = false;
            PathGeometry pathGeometry;
            flag |= GeometryHelper.EnsureGeometryType<PathGeometry>(out pathGeometry, ref cachedGeometry, () => new PathGeometry());
            flag |= pathGeometry.SetIfDifferent(PathGeometry.FillRuleProperty, FillRule.Nonzero);
            flag |= pathGeometry.Figures.EnsureListCount(1, () => new PathFigure());
            PathFigure pathFigure = pathGeometry.Figures[0];
            flag |= pathFigure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            flag |= pathFigure.SetIfDifferent(PathFigure.IsFilledProperty, true);
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, GeometryHelper.GetArcPoint(start, LogicalBounds));
            flag |= pathFigure.Segments.EnsureListCountAtLeast(3, () => new ArcSegment());
            ArcSegment dependencyObject;
            flag |= GeometryHelper.EnsureSegmentType<ArcSegment>(out dependencyObject, pathFigure.Segments, 0, () => new ArcSegment());
            flag |= dependencyObject.SetIfDifferent(ArcSegment.PointProperty, GeometryHelper.GetArcPoint(end, LogicalBounds));
            flag |= dependencyObject.SetIfDifferent(ArcSegment.SizeProperty, new Size(LogicalBounds.Width / 2.0, LogicalBounds.Height / 2.0));
            flag |= dependencyObject.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            LineSegment dependencyObject2;
            flag |= GeometryHelper.EnsureSegmentType<LineSegment>(out dependencyObject2, pathFigure.Segments, 1, () => new LineSegment());
            Rect logicalBounds = LogicalBounds;
            double num = logicalBounds.Width / 2.0;
            double num2 = logicalBounds.Height / 2.0;
            if (relativeMode || MathHelper.AreClose(num, num2))
            {
                Rect bound = LogicalBounds.Resize(1.0 - relativeThickness);
                flag |= dependencyObject2.SetIfDifferent(LineSegment.PointProperty, GeometryHelper.GetArcPoint(end, bound));
                flag |= pathFigure.Segments.EnsureListCount(3, () => new ArcSegment());
                ArcSegment dependencyObject3;
                flag |= GeometryHelper.EnsureSegmentType<ArcSegment>(out dependencyObject3, pathFigure.Segments, 2, () => new ArcSegment());
                flag |= dependencyObject3.SetIfDifferent(ArcSegment.PointProperty, GeometryHelper.GetArcPoint(start, bound));
                flag |= dependencyObject3.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(bound));
                flag |= dependencyObject3.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0);
                flag |= dependencyObject3.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Counterclockwise);
            }
            else
            {
                Point point = default(Point);
                double intersect = InnerCurveSelfIntersect(num, num2, absoluteThickness);
                double[] angles = ComputeAngleRanges(num, num2, intersect, end, start);
                flag |= SyncPieceWiseInnerCurves(pathFigure, 2, ref point, angles);
                flag |= dependencyObject2.SetIfDifferent(LineSegment.PointProperty, point);
            }
            return flag;
        }

        private bool SyncPieceWiseInnerCurves(PathFigure figure, int index, ref Point firstPoint, params double[] angles)
        {
            bool flag = false;
            int num = angles.Length;
            Rect logicalBounds = LogicalBounds;
            double offset = absoluteThickness;
            flag |= figure.Segments.EnsureListCount(index + num / 2, () => new PolyBezierSegment());
            for (int i = 0; i < num / 2; i++)
            {
                IList<Point> list = ComputeOneInnerCurve(angles[i * 2], angles[i * 2 + 1], logicalBounds, offset);
                if (i == 0)
                {
                    firstPoint = list[0];
                }
                flag |= PathSegmentHelper.SyncPolyBezierSegment(figure.Segments, index + i, list, 1, list.Count - 1);
            }
            return flag;
        }

        private static IList<Point> ComputeOneInnerCurve(double start, double end, Rect bounds, double offset)
        {
            double num = bounds.Width / 2.0;
            double num2 = bounds.Height / 2.0;
            Point point = bounds.Center();
            start = start * 3.141592653589793 / 180.0;
            end = end * 3.141592653589793 / 180.0;
            double num3 = 0.17453292519943295;
            int num4 = (int)Math.Ceiling(Math.Abs(end - start) / num3);
            num4 = Math.Max(2, num4);
            List<Point> list = new List<Point>(num4);
            List<Vector> list2 = new List<Vector>(num4);
            Point point2 = default(Point);
            Point point3 = default(Point);
            Vector vector = default(Vector);
            Vector vector2 = default(Vector);
            Vector vector3 = default(Vector);
            Vector vector4 = default(Vector);
            for (int i = 0; i < num4; i++)
            {
                double num5 = MathHelper.Lerp(start, end, (double)i / (double)(num4 - 1));
                double num6 = Math.Sin(num5);
                double num7 = Math.Cos(num5);
                point2.X = point.X + num * num6;
                point2.Y = point.Y - num2 * num7;
                vector.X = num * num7;
                vector.Y = num2 * num6;
                vector2.X = -num2 * num6;
                vector2.Y = num * num7;
                double num8 = num2 * num2 * num6 * num6 + num * num * num7 * num7;
                double num9 = Math.Sqrt(num8);
                double num10 = 2.0 * num6 * num7 * (num2 * num2 - num * num);
                vector3.X = -num2 * num7;
                vector3.Y = -num * num6;
                point3.X = point2.X + offset * vector2.X / num9;
                point3.Y = point2.Y + offset * vector2.Y / num9;
                vector4.X = vector.X + offset / num9 * (vector3.X - 0.5 * vector2.X / num8 * num10);
                vector4.Y = vector.Y + offset / num9 * (vector3.Y - 0.5 * vector2.Y / num8 * num10);
                list.Add(point3);
                list2.Add(-vector4.Normalized());
            }
            List<Point> list3 = new List<Point>(num4 * 3 + 1);
            list3.Add(list[0]);
            for (int j = 1; j < num4; j++)
            {
                point2 = list[j - 1];
                point3 = list[j];
                double scalar = GeometryHelper.Distance(point2, point3) / 3.0;
                list3.Add(point2 + list2[j - 1] * scalar);
                list3.Add(point3 - list2[j] * scalar);
                list3.Add(point3);
            }
            return list3;
        }

        internal static double InnerCurveSelfIntersect(double radiusX, double radiusY, double thickness)
        {
            double num = 0.0;
            double num2 = 1.5707963267948966;
            bool flag = radiusX <= radiusY;
            Vector vector = default(Vector);
            while (!AreCloseEnough(num, num2))
            {
                double num3 = (num + num2) / 2.0;
                double num4 = Math.Cos(num3);
                double num5 = Math.Sin(num3);
                vector.X = radiusY * num5;
                vector.Y = radiusX * num4;
                vector.Normalize();
                if (flag)
                {
                    double num6 = radiusX * num5 - vector.X * thickness;
                    if (num6 > 0.0)
                    {
                        num2 = num3;
                    }
                    else if (num6 < 0.0)
                    {
                        num = num3;
                    }
                }
                else
                {
                    double num7 = radiusY * num4 - vector.Y * thickness;
                    if (num7 < 0.0)
                    {
                        num2 = num3;
                    }
                    else if (num7 > 0.0)
                    {
                        num = num3;
                    }
                }
            }
            double num8 = (num + num2) / 2.0;
            if (AreCloseEnough(num8, 0.0))
            {
                return 0.0;
            }
            if (!AreCloseEnough(num8, 1.5707963267948966))
            {
                return num8 * 180.0 / 3.141592653589793;
            }
            return 90.0;
        }

        private static bool AreCloseEnough(double angleA, double angleB)
        {
            return Math.Abs(angleA - angleB) < 0.001;
        }

        private static Size GetArcSize(Rect bound)
        {
            return new Size(bound.Width / 2.0, bound.Height / 2.0);
        }

        private static double NormalizeAngle(double degree)
        {
            if (degree < 0.0 || degree > 360.0)
            {
                degree %= 360.0;
                if (degree < 0.0)
                {
                    degree += 360.0;
                }
            }
            return degree;
        }

        private double relativeThickness;

        private double absoluteThickness;
    }
}
