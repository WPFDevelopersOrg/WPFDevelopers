using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Core
{
    public static class PathSegmentHelper
    {
        public static PathSegment ArcToBezierSegments(ArcSegment arcSegment, Point startPoint)
        {
            bool isStroked = arcSegment.IsStroked();
            Point[] array;
            int num;
            ArcToBezierHelper.ArcToBezier(startPoint.X, startPoint.Y, arcSegment.Size.Width, arcSegment.Size.Height, arcSegment.RotationAngle, arcSegment.IsLargeArc, arcSegment.SweepDirection == SweepDirection.Clockwise, arcSegment.Point.X, arcSegment.Point.Y, out array, out num);
            if (num == -1)
            {
                return null;
            }
            if (num == 0)
            {
                return CreateLineSegment(arcSegment.Point, isStroked);
            }
            if (num == 1)
            {
                return CreateBezierSegment(array[0], array[1], array[2], isStroked);
            }
            return CreatePolyBezierSegment(array, 0, num * 3, isStroked);
        }

        private static void SetIsStroked(this PathSegment segment, bool isStroked)
        {
            if (segment.IsStroked != isStroked)
            {
                segment.IsStroked = isStroked;
            }
        }

        public static LineSegment CreateLineSegment(Point point, bool isStroked = true)
        {
            LineSegment lineSegment = new LineSegment();
            lineSegment.Point = point;
            lineSegment.SetIsStroked(isStroked);
            return lineSegment;
        }

        public static QuadraticBezierSegment CreateQuadraticBezierSegment(Point point1, Point point2, bool isStroked = true)
        {
            QuadraticBezierSegment quadraticBezierSegment = new QuadraticBezierSegment();
            quadraticBezierSegment.Point1 = point1;
            quadraticBezierSegment.Point2 = point2;
            quadraticBezierSegment.SetIsStroked(isStroked);
            return quadraticBezierSegment;
        }

        public static BezierSegment CreateBezierSegment(Point point1, Point point2, Point point3, bool isStroked = true)
        {
            BezierSegment bezierSegment = new BezierSegment();
            bezierSegment.Point1 = point1;
            bezierSegment.Point2 = point2;
            bezierSegment.Point3 = point3;
            bezierSegment.SetIsStroked(isStroked);
            return bezierSegment;
        }

        public static PolyBezierSegment CreatePolyBezierSegment(IList<Point> points, int start, int count, bool isStroked = true)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            count = count / 3 * 3;
            if (count < 0 || points.Count < start + count)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            PolyBezierSegment polyBezierSegment = new PolyBezierSegment();
            polyBezierSegment.Points = new PointCollection();
            for (int i = 0; i < count; i++)
            {
                polyBezierSegment.Points.Add(points[start + i]);
            }
            polyBezierSegment.SetIsStroked(isStroked);
            return polyBezierSegment;
        }

        public static PolyQuadraticBezierSegment CreatePolyQuadraticBezierSegment(IList<Point> points, int start, int count, bool isStroked = true)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            count = count / 2 * 2;
            if (count < 0 || points.Count < start + count)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            PolyQuadraticBezierSegment polyQuadraticBezierSegment = new PolyQuadraticBezierSegment();
            polyQuadraticBezierSegment.Points = new PointCollection();
            for (int i = 0; i < count; i++)
            {
                polyQuadraticBezierSegment.Points.Add(points[start + i]);
            }
            polyQuadraticBezierSegment.SetIsStroked(isStroked);
            return polyQuadraticBezierSegment;
        }

        public static PolyLineSegment CreatePolylineSegment(IList<Point> points, int start, int count, bool isStroked = true)
        {
            if (count < 0 || points.Count < start + count)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            polyLineSegment.Points = new PointCollection();
            for (int i = 0; i < count; i++)
            {
                polyLineSegment.Points.Add(points[start + i]);
            }
            polyLineSegment.SetIsStroked(isStroked);
            return polyLineSegment;
        }

        public static ArcSegment CreateArcSegment(Point point, Size size, bool isLargeArc, bool clockwise, double rotationAngle = 0.0, bool isStroked = true)
        {
            ArcSegment arcSegment = new ArcSegment();
            arcSegment.SetIfDifferent(ArcSegment.PointProperty, point);
            arcSegment.SetIfDifferent(ArcSegment.SizeProperty, size);
            arcSegment.SetIfDifferent(ArcSegment.IsLargeArcProperty, isLargeArc);
            arcSegment.SetIfDifferent(ArcSegment.SweepDirectionProperty, clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise);
            arcSegment.SetIfDifferent(ArcSegment.RotationAngleProperty, rotationAngle);
            arcSegment.SetIsStroked(isStroked);
            return arcSegment;
        }

        public static bool SyncPolylineSegment(PathSegmentCollection collection, int index, IList<Point> points, int start, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (index < 0 || index >= collection.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException("start");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (points.Count < start + count)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            bool flag = false;
            PolyLineSegment polyLineSegment;
            if ((polyLineSegment = collection[index] as PolyLineSegment) == null)
            {
                polyLineSegment = (PolyLineSegment)(collection[index] = new PolyLineSegment());
                flag = true;
            }
            flag |= polyLineSegment.Points.EnsureListCount(count, null);
            for (int i = 0; i < count; i++)
            {
                if (polyLineSegment.Points[i] != points[i + start])
                {
                    polyLineSegment.Points[i] = points[i + start];
                    flag = true;
                }
            }
            return flag;
        }

        public static bool SyncPolyBezierSegment(PathSegmentCollection collection, int index, IList<Point> points, int start, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (index < 0 || index >= collection.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException("start");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (points.Count < start + count)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            bool result = false;
            count = count / 3 * 3;
            PolyBezierSegment polyBezierSegment;
            if ((polyBezierSegment = collection[index] as PolyBezierSegment) == null)
            {
                polyBezierSegment = (PolyBezierSegment)(collection[index] = new PolyBezierSegment());
                result = true;
            }
            polyBezierSegment.Points.EnsureListCount(count, null);
            for (int i = 0; i < count; i++)
            {
                if (polyBezierSegment.Points[i] != points[i + start])
                {
                    polyBezierSegment.Points[i] = points[i + start];
                    result = true;
                }
            }
            return result;
        }

        public static bool IsEmpty(this PathSegment segment)
        {
            return segment.GetPointCount() == 0;
        }

        public static int GetPointCount(this PathSegment segment)
        {
            if (segment is ArcSegment)
            {
                return 1;
            }
            if (segment is LineSegment)
            {
                return 1;
            }
            if (segment is QuadraticBezierSegment)
            {
                return 2;
            }
            if (segment is BezierSegment)
            {
                return 3;
            }
            PolyLineSegment polyLineSegment;
            if ((polyLineSegment = segment as PolyLineSegment) != null)
            {
                return polyLineSegment.Points.Count;
            }
            PolyQuadraticBezierSegment polyQuadraticBezierSegment;
            if ((polyQuadraticBezierSegment = segment as PolyQuadraticBezierSegment) != null)
            {
                return polyQuadraticBezierSegment.Points.Count / 2 * 2;
            }
            PolyBezierSegment polyBezierSegment;
            if ((polyBezierSegment = segment as PolyBezierSegment) != null)
            {
                return polyBezierSegment.Points.Count / 3 * 3;
            }
            return 0;
        }

        public static Point GetLastPoint(this PathSegment segment)
        {
            return segment.GetPoint(-1);
        }

        public static Point GetPoint(this PathSegment segment, int index)
        {
            return PathSegmentImplementation.Create(segment).GetPoint(index);
        }

        public static void FlattenSegment(this PathSegment segment, IList<Point> points, Point start, double tolerance)
        {
            PathSegmentImplementation.Create(segment, start).Flatten(points, tolerance);
        }

        public static IEnumerable<SimpleSegment> GetSimpleSegments(this PathSegment segment, Point start)
        {
            PathSegmentImplementation implementation = PathSegmentImplementation.Create(segment, start);
            foreach (SimpleSegment simpleSegment in implementation.GetSimpleSegments())
            {
                yield return simpleSegment;
            }
            yield break;
        }

        private static class ArcToBezierHelper
        {
            public static void ArcToBezier(double xStart, double yStart, double xRadius, double yRadius, double rRotation, bool fLargeArc, bool fSweepUp, double xEnd, double yEnd, out Point[] pPt, out int cPieces)
            {
                double num = 1E-06;
                pPt = new Point[12];
                double num2 = num * num;
                bool flag = false;
                cPieces = -1;
                double num3 = 0.5 * (xEnd - xStart);
                double num4 = 0.5 * (yEnd - yStart);
                double num5 = num3 * num3 + num4 * num4;
                if (num5 < num2)
                {
                    return;
                }
                if (!AcceptRadius(num5, num2, ref xRadius) || !AcceptRadius(num5, num2, ref yRadius))
                {
                    cPieces = 0;
                    return;
                }
                double num6;
                double num7;
                if (Math.Abs(rRotation) < num)
                {
                    num6 = 1.0;
                    num7 = 0.0;
                }
                else
                {
                    rRotation = -rRotation * 3.141592653589793 / 180.0;
                    num6 = Math.Cos(rRotation);
                    num7 = Math.Sin(rRotation);
                    double num8 = num3 * num6 - num4 * num7;
                    num4 = num3 * num7 + num4 * num6;
                    num3 = num8;
                }
                num3 /= xRadius;
                num4 /= yRadius;
                num5 = num3 * num3 + num4 * num4;
                double num11;
                double num10;
                if (num5 > 1.0)
                {
                    double num9 = Math.Sqrt(num5);
                    xRadius *= num9;
                    yRadius *= num9;
                    num10 = (num11 = 0.0);
                    flag = true;
                    num3 /= num9;
                    num4 /= num9;
                }
                else
                {
                    double num12 = Math.Sqrt((1.0 - num5) / num5);
                    if (fLargeArc != fSweepUp)
                    {
                        num11 = -num12 * num4;
                        num10 = num12 * num3;
                    }
                    else
                    {
                        num11 = num12 * num4;
                        num10 = -num12 * num3;
                    }
                }
                Point point = new Point(-num3 - num11, -num4 - num10);
                Point point2 = new Point(num3 - num11, num4 - num10);
                Matrix matrix = new Matrix(num6 * xRadius, -num7 * xRadius, num7 * yRadius, num6 * yRadius, 0.5 * (xEnd + xStart), 0.5 * (yEnd + yStart));
                if (!flag)
                {
                    matrix.OffsetX += matrix.M11 * num11 + matrix.M21 * num10;
                    matrix.OffsetY += matrix.M12 * num11 + matrix.M22 * num10;
                }
                double num13;
                double num14;
                GetArcAngle(point, point2, fLargeArc, fSweepUp, out num13, out num14, out cPieces);
                double num15 = GetBezierDistance(num13, 1.0);
                if (!fSweepUp)
                {
                    num15 = -num15;
                }
                Point rhs = new Point(-num15 * point.Y, num15 * point.X);
                int num16 = 0;
                pPt = new Point[cPieces * 3];
                Point point4;
                for (int i = 1; i < cPieces; i++)
                {
                    Point point3 = new Point(point.X * num13 - point.Y * num14, point.X * num14 + point.Y * num13);
                    point4 = new Point(-num15 * point3.Y, num15 * point3.X);
                    pPt[num16++] = matrix.Transform(point.Plus(rhs));
                    pPt[num16++] = matrix.Transform(point3.Minus(point4));
                    pPt[num16++] = matrix.Transform(point3);
                    point = point3;
                    rhs = point4;
                }
                point4 = new Point(-num15 * point2.Y, num15 * point2.X);
                pPt[num16++] = matrix.Transform(point.Plus(rhs));
                pPt[num16++] = matrix.Transform(point2.Minus(point4));
                pPt[num16] = new Point(xEnd, yEnd);
            }

            private static void GetArcAngle(Point ptStart, Point ptEnd, bool fLargeArc, bool fSweepUp, out double rCosArcAngle, out double rSinArcAngle, out int cPieces)
            {
                rCosArcAngle = GeometryHelper.Dot(ptStart, ptEnd);
                rSinArcAngle = GeometryHelper.Determinant(ptStart, ptEnd);
                if (rCosArcAngle >= 0.0)
                {
                    if (!fLargeArc)
                    {
                        cPieces = 1;
                        return;
                    }
                    cPieces = 4;
                }
                else if (fLargeArc)
                {
                    cPieces = 3;
                }
                else
                {
                    cPieces = 2;
                }
                double num = Math.Atan2(rSinArcAngle, rCosArcAngle);
                if (fSweepUp)
                {
                    if (num < 0.0)
                    {
                        num += 6.283185307179586;
                    }
                }
                else if (num > 0.0)
                {
                    num -= 6.283185307179586;
                }
                num /= (double)cPieces;
                rCosArcAngle = Math.Cos(num);
                rSinArcAngle = Math.Sin(num);
            }

            private static double GetBezierDistance(double rDot, double rRadius = 1.0)
            {
                double num = rRadius * rRadius;
                double result = 0.0;
                double num2 = 0.5 * (num + rDot);
                if (num2 >= 0.0)
                {
                    double num3 = num - num2;
                    if (num3 > 0.0)
                    {
                        double num4 = Math.Sqrt(num3);
                        double num5 = 4.0 * (rRadius - Math.Sqrt(num2)) / 3.0;
                        if (num5 <= num4 * 1E-06)
                        {
                            result = 0.0;
                        }
                        else
                        {
                            result = num5 / num4;
                        }
                    }
                }
                return result;
            }

            private static bool AcceptRadius(double rHalfChord2, double rFuzz2, ref double rRadius)
            {
                bool flag = rRadius * rRadius > rHalfChord2 * rFuzz2;
                if (flag && rRadius < 0.0)
                {
                    rRadius = -rRadius;
                }
                return flag;
            }
        }

        private abstract class PathSegmentImplementation
        {
            public Point Start { get; private set; }

            public abstract void Flatten(IList<Point> points, double tolerance);

            public abstract Point GetPoint(int index);

            public abstract IEnumerable<SimpleSegment> GetSimpleSegments();

            public static PathSegmentImplementation Create(PathSegment segment, Point start)
            {
                PathSegmentImplementation pathSegmentImplementation = PathSegmentImplementation.Create(segment);
                pathSegmentImplementation.Start = start;
                return pathSegmentImplementation;
            }

            public static PathSegmentImplementation Create(PathSegment segment)
            {
                PathSegmentImplementation result;
                if ((result = BezierSegmentImplementation.Create(segment as BezierSegment)) == null && (result = LineSegmentImplementation.Create(segment as LineSegment)) == null && (result = ArcSegmentImplementation.Create(segment as ArcSegment)) == null && (result = PolyLineSegmentImplementation.Create(segment as PolyLineSegment)) == null && (result = PolyBezierSegmentImplementation.Create(segment as PolyBezierSegment)) == null && (result = QuadraticBezierSegmentImplementation.Create(segment as QuadraticBezierSegment)) == null && (result = PolyQuadraticBezierSegmentImplementation.Create(segment as PolyQuadraticBezierSegment)) == null)
                {
                    throw new NotImplementedException();
                }
                return result;
            }
        }

        private class BezierSegmentImplementation : PathSegmentImplementation
        {
            public static PathSegmentImplementation Create(BezierSegment source)
            {
                if (source != null)
                {
                    return new BezierSegmentImplementation
                    {
                        segment = source
                    };
                }
                return null;
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                Point[] controlPoints = new Point[]
                {
                    Start,
                    segment.Point1,
                    segment.Point2,
                    segment.Point3
                };
                List<Point> list = new List<Point>();
                BezierCurveFlattener.FlattenCubic(controlPoints, tolerance, list, true, null);
                points.AddRange(list);
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 2)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (index == 0)
                {
                    return segment.Point1;
                }
                if (index == 1)
                {
                    return segment.Point2;
                }
                return segment.Point3;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                yield return SimpleSegment.Create(Start, segment.Point1, segment.Point2, segment.Point3);
                yield break;
            }

            private BezierSegment segment;
        }

        private class QuadraticBezierSegmentImplementation : PathSegmentImplementation
        {
            public static PathSegmentImplementation Create(QuadraticBezierSegment source)
            {
                if (source != null)
                {
                    return new QuadraticBezierSegmentImplementation
                    {
                        segment = source
                    };
                }
                return null;
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                Point[] controlPoints = new Point[]
                {
                    Start,
                    segment.Point1,
                    segment.Point2
                };
                List<Point> list = new List<Point>();
                BezierCurveFlattener.FlattenQuadratic(controlPoints, tolerance, list, true, null);
                points.AddRange(list);
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 1)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (index == 0)
                {
                    return segment.Point1;
                }
                return segment.Point2;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                yield return SimpleSegment.Create(Start, segment.Point1, segment.Point2);
                yield break;
            }

            private QuadraticBezierSegment segment;
        }

        private class PolyBezierSegmentImplementation : PathSegmentImplementation
        {
            public static PathSegmentImplementation Create(PolyBezierSegment source)
            {
                if (source != null)
                {
                    return new PolyBezierSegmentImplementation
                    {
                        segment = source
                    };
                }
                return null;
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                Point point = Start;
                int num = segment.Points.Count / 3 * 3;
                for (int i = 0; i < num; i += 3)
                {
                    Point[] controlPoints = new Point[]
                    {
                        point,
                        segment.Points[i],
                        segment.Points[i + 1],
                        segment.Points[i + 2]
                    };
                    List<Point> list = new List<Point>();
                    BezierCurveFlattener.FlattenCubic(controlPoints, tolerance, list, true, null);
                    points.AddRange(list);
                    point = segment.Points[i + 2];
                }
            }

            public override Point GetPoint(int index)
            {
                int num = segment.Points.Count / 3 * 3;
                if (index < -1 || index > num - 1)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (index != -1)
                {
                    return segment.Points[index];
                }
                return segment.Points[num - 1];
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                Point point0 = Start;
                IList<Point> points = segment.Points;
                int count = segment.Points.Count / 3;
                for (int i = 0; i < count; i++)
                {
                    int k3 = i * 3;
                    yield return SimpleSegment.Create(point0, points[k3], points[k3 + 1], points[k3 + 2]);
                    point0 = points[k3 + 2];
                }
                yield break;
            }

            private PolyBezierSegment segment;
        }

        private class PolyQuadraticBezierSegmentImplementation : PathSegmentImplementation
        {
            public static PathSegmentImplementation Create(PolyQuadraticBezierSegment source)
            {
                if (source != null)
                {
                    return new PolyQuadraticBezierSegmentImplementation
                    {
                        segment = source
                    };
                }
                return null;
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                Point point = Start;
                int num = segment.Points.Count / 2 * 2;
                for (int i = 0; i < num; i += 2)
                {
                    Point[] controlPoints = new Point[]
                    {
                        point,
                        segment.Points[i],
                        segment.Points[i + 1]
                    };
                    List<Point> list = new List<Point>();
                    BezierCurveFlattener.FlattenQuadratic(controlPoints, tolerance, list, true, null);
                    points.AddRange(list);
                    point = segment.Points[i + 1];
                }
            }

            public override Point GetPoint(int index)
            {
                int num = segment.Points.Count / 2 * 2;
                if (index < -1 || index > num - 1)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (index != -1)
                {
                    return segment.Points[index];
                }
                return segment.Points[num - 1];
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                Point point0 = Start;
                IList<Point> points = segment.Points;
                int count = segment.Points.Count / 2;
                for (int i = 0; i < count; i++)
                {
                    int k2 = i * 2;
                    yield return SimpleSegment.Create(point0, points[k2], points[k2 + 1]);
                    point0 = points[k2 + 1];
                }
                yield break;
            }

            private PolyQuadraticBezierSegment segment;
        }

        private class ArcSegmentImplementation : PathSegmentImplementation
        {
            public static PathSegmentImplementation Create(ArcSegment source)
            {
                if (source != null)
                {
                    return new ArcSegmentImplementation
                    {
                        segment = source
                    };
                }
                return null;
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                PathSegment pathSegment = ArcToBezierSegments(segment, Start);
                if (pathSegment != null)
                {
                    pathSegment.FlattenSegment(points, Start, tolerance);
                }
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return segment.Point;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                PathSegment pathSegment = ArcToBezierSegments(segment, Start);
                if (pathSegment != null)
                {
                    return pathSegment.GetSimpleSegments(Start);
                }
                return Enumerable.Empty<SimpleSegment>();
            }

            private ArcSegment segment;
        }

        private class LineSegmentImplementation : PathSegmentImplementation
        {
            public static PathSegmentImplementation Create(LineSegment source)
            {
                if (source != null)
                {
                    return new LineSegmentImplementation
                    {
                        segment = source
                    };
                }
                return null;
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                points.Add(segment.Point);
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return segment.Point;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                yield return SimpleSegment.Create(Start, segment.Point);
                yield break;
            }

            private LineSegment segment;
        }

        private class PolyLineSegmentImplementation : PathSegmentImplementation
        {
            public static PathSegmentImplementation Create(PolyLineSegment source)
            {
                if (source != null)
                {
                    return new PolyLineSegmentImplementation
                    {
                        segment = source
                    };
                }
                return null;
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                points.AddRange(segment.Points);
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > segment.Points.Count - 1)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (index != -1)
                {
                    return segment.Points[index];
                }
                return segment.Points.Last();
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                Point point0 = Start;
                foreach (Point point in segment.Points)
                {
                    yield return SimpleSegment.Create(point0, point);
                    point0 = point;
                }
                yield break;
            }

            private PolyLineSegment segment;
        }
    }
}
