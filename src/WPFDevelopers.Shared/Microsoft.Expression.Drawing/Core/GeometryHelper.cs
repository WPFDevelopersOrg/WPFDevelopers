using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Core
{
    public static class GeometryHelper
    {
        public static Rect Bounds(this Size size) => new Rect(0.0, 0.0, size.Width, size.Height);

        public static bool HasValidArea(this Size size) => size.Width > 0.0 && size.Height > 0.0 && !double.IsInfinity(size.Width) && !double.IsInfinity(size.Height);

        public static Size Size(this Rect rect) => new Size(rect.Width, rect.Height);

        public static Point TopLeft(this Rect rect) => new Point(rect.Left, rect.Top);

        public static Point TopRight(this Rect rect) => new Point(rect.Right, rect.Top);

        public static Point BottomRight(this Rect rect) => new Point(rect.Right, rect.Bottom);

        public static Point BottomLeft(this Rect rect) => new Point(rect.Left, rect.Bottom);

        public static Point Center(this Rect rect) => new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);

        public static Thickness Subtract(this Rect lhs, Rect rhs) => new Thickness(rhs.Left - lhs.Left, rhs.Top - lhs.Top, lhs.Right - rhs.Right, lhs.Bottom - rhs.Bottom);

        public static Rect Resize(this Rect rect, double ratio) => rect.Resize(ratio, ratio);

        public static Rect Resize(this Rect rect, double ratioX, double ratioY)
        {
            Point point = rect.Center();
            double num = rect.Width * ratioX;
            double num2 = rect.Height * ratioY;
            return new Rect(point.X - num / 2.0, point.Y - num2 / 2.0, num, num2);
        }

        public static Rect ActualBounds(this FrameworkElement element) => new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight);

        public static Thickness Negate(this Thickness thickness) => new Thickness(-thickness.Left, -thickness.Top, -thickness.Right, -thickness.Bottom);

        public static Vector Subtract(this Point lhs, Point rhs) => new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y);

        public static Point Plus(this Point lhs, Point rhs) => new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);

        public static Point Minus(this Point lhs, Point rhs) => new Point(lhs.X - rhs.X, lhs.Y - rhs.Y);

        public static Vector Normalized(this Vector vector)
        {
            Vector vector2 = new Vector(vector.X, vector.Y);
            double length = vector2.Length;
            if (MathHelper.IsVerySmall(length))
                return new Vector(0.0, 1.0);

            return vector2 / length;
        }

        public static void ApplyTransform(this IList<Point> points, GeneralTransform transform)
        {
            for (int i = 0; i < points.Count; i++)
                points[i] = transform.Transform(points[i]);
        }

        public static PathGeometry ConvertToPathGeometry(string abbreviatedGeometry)
        {
            return PathGeometryHelper.ConvertToPathGeometry(abbreviatedGeometry);
        }

        public static void FlattenFigure(PathFigure figure, IList<Point> points, double tolerance)
        {
            bool removeRepeat = false;
            PathFigureHelper.FlattenFigure(figure, points, tolerance, removeRepeat);
        }

        public static Point Lerp(Point pointA, Point pointB, double alpha) => new Point(MathHelper.Lerp(pointA.X, pointB.X, alpha), MathHelper.Lerp(pointA.Y, pointB.Y, alpha));

        public static Vector Lerp(Vector vectorA, Vector vectorB, double alpha) => new Vector(MathHelper.Lerp(vectorA.X, vectorB.X, alpha), MathHelper.Lerp(vectorA.Y, vectorB.Y, alpha));

        public static Rect Inflate(Rect rect, double offset) => Inflate(rect, new Thickness(offset));

        public static Rect Inflate(Rect rect, double offsetX, double offsetY) => Inflate(rect, new Thickness(offsetX, offsetY, offsetX, offsetY));

        public static Rect Inflate(Rect rect, Size size) => Inflate(rect, new Thickness(size.Width, size.Height, size.Width, size.Height));

        public static Rect Inflate(Rect rect, Thickness thickness)
        {
            double num = rect.Width + thickness.Left + thickness.Right;
            double num2 = rect.Height + thickness.Top + thickness.Bottom;
            double num3 = rect.X - thickness.Left;
            if (num < 0.0)
            {
                num3 += num / 2.0;
                num = 0.0;
            }
            double num4 = rect.Y - thickness.Top;
            if (num2 < 0.0)
            {
                num4 += num2 / 2.0;
                num2 = 0.0;
            }
            return new Rect(num3, num4, num, num2);
        }

        public static Point GetArcPoint(double degree)
        {
            double num = degree * 3.141592653589793 / 180.0;
            return new Point(0.5 + 0.5 * Math.Sin(num), 0.5 - 0.5 * Math.Cos(num));
        }

        public static Point GetArcPoint(double degree, Rect bound)
        {
            Point arcPoint = GetArcPoint(degree);
            return RelativeToAbsolutePoint(bound, arcPoint);
        }

        public static double GetArcAngle(Point point) => Math.Atan2(point.Y - 0.5, point.X - 0.5) * 180.0 / 3.141592653589793 + 90.0;

        public static double GetArcAngle(Point point, Rect bound)
        {
            Point point2 = AbsoluteToRelativePoint(bound, point);
            return GetArcAngle(point2);
        }

        public static Transform RelativeTransform(Rect from, Rect to)
        {
            Point point = from.Center();
            Point point2 = to.Center();
            return new TransformGroup
            {
                Children = new TransformCollection
                {
                    new TranslateTransform
                    {
                        X = -point.X,
                        Y = -point.Y
                    },
                    new ScaleTransform
                    {
                        ScaleX = MathHelper.SafeDivide(to.Width, from.Width, 1.0),
                        ScaleY = MathHelper.SafeDivide(to.Height, from.Height, 1.0)
                    },
                    new TranslateTransform
                    {
                        X = point2.X,
                        Y = point2.Y
                    }
                }
            };
        }

        public static GeneralTransform RelativeTransform(UIElement from, UIElement to)
        {
            if (from == null || to == null)
            {
                return null;
            }
            GeneralTransform result;
            try
            {
                result = from.TransformToVisual(to);
            }
            catch (ArgumentException)
            {
                result = null;
            }
            catch (InvalidOperationException)
            {
                result = null;
            }
            return result;
        }

        public static Point SafeTransform(GeneralTransform transform, Point point)
        {
            Point result = point;
            if (transform != null && transform.TryTransform(point, out result))
                return result;

            return point;
        }

        public static Point RelativeToAbsolutePoint(Rect bound, Point relative) => new Point(bound.X + relative.X * bound.Width, bound.Y + relative.Y * bound.Height);

        public static Point AbsoluteToRelativePoint(Rect bound, Point absolute) => new Point(MathHelper.SafeDivide(absolute.X - bound.X, bound.Width, 1.0), MathHelper.SafeDivide(absolute.Y - bound.Y, bound.Height, 1.0));

        public static Rect GetStretchBound(Rect logicalBound, Stretch stretch, Size aspectRatio)
        {
            if (stretch == Stretch.None)
                stretch = Stretch.Fill;

            if (stretch == Stretch.Fill || !aspectRatio.HasValidArea())
                return logicalBound;

            Point point = logicalBound.Center();
            if (stretch == Stretch.Uniform)
            {
                if (aspectRatio.Width * logicalBound.Height < logicalBound.Width * aspectRatio.Height)
                    logicalBound.Width = logicalBound.Height * aspectRatio.Width / aspectRatio.Height;
                else
                    logicalBound.Height = logicalBound.Width * aspectRatio.Height / aspectRatio.Width;
            }
            else if (stretch == Stretch.UniformToFill)
            {
                if (aspectRatio.Width * logicalBound.Height < logicalBound.Width * aspectRatio.Height)
                    logicalBound.Height = logicalBound.Width * aspectRatio.Height / aspectRatio.Width;
                else
                    logicalBound.Width = logicalBound.Height * aspectRatio.Width / aspectRatio.Height;
            }

            return new Rect(point.X - logicalBound.Width / 2.0, point.Y - logicalBound.Height / 2.0, logicalBound.Width, logicalBound.Height);
        }

        public static Point Midpoint(Point lhs, Point rhs) => new Point((lhs.X + rhs.X) / 2.0, (lhs.Y + rhs.Y) / 2.0);

        public static double Dot(Vector lhs, Vector rhs) => lhs.X * rhs.X + lhs.Y * rhs.Y;

        public static double Dot(Point lhs, Point rhs) => lhs.X * rhs.X + lhs.Y * rhs.Y;

        public static double Distance(Point lhs, Point rhs)
        {
            double num = lhs.X - rhs.X;
            double num2 = lhs.Y - rhs.Y;
            return Math.Sqrt(num * num + num2 * num2);
        }

        public static double SquaredDistance(Point lhs, Point rhs)
        {
            double num = lhs.X - rhs.X;
            double num2 = lhs.Y - rhs.Y;
            return num * num + num2 * num2;
        }

        public static double Determinant(Point lhs, Point rhs) => lhs.X * rhs.Y - lhs.Y * rhs.X;

        public static Vector Normal(Point lhs, Point rhs) => new Vector(lhs.Y - rhs.Y, rhs.X - lhs.X).Normalized();

        public static Vector Perpendicular(this Vector vector) => new Vector(-vector.Y, vector.X);

        public static bool GeometryEquals(Geometry firstGeometry, Geometry secondGeometry)
        {
            if (firstGeometry == secondGeometry)
                return true;

            if (firstGeometry == null || secondGeometry == null)
                return false;

            if (firstGeometry.GetType() != secondGeometry.GetType())
                return false;

            if (!firstGeometry.Transform.TransformEquals(secondGeometry.Transform))
                return false;

            StreamGeometry streamGeometry = firstGeometry as StreamGeometry;
            StreamGeometry streamGeometry2 = secondGeometry as StreamGeometry;
            if (streamGeometry != null && streamGeometry2 != null)
                return streamGeometry.ToString() == streamGeometry2.ToString();

            PathGeometry pathGeometry = firstGeometry as PathGeometry;
            PathGeometry pathGeometry2 = secondGeometry as PathGeometry;
            if (pathGeometry != null && pathGeometry2 != null)
                return PathGeometryEquals(pathGeometry, pathGeometry2);

            LineGeometry lineGeometry = firstGeometry as LineGeometry;
            LineGeometry lineGeometry2 = secondGeometry as LineGeometry;
            if (lineGeometry != null && lineGeometry2 != null)
                return LineGeometryEquals(lineGeometry, lineGeometry2);

            RectangleGeometry rectangleGeometry = firstGeometry as RectangleGeometry;
            RectangleGeometry rectangleGeometry2 = secondGeometry as RectangleGeometry;
            if (rectangleGeometry != null && rectangleGeometry2 != null)
                return RectangleGeometryEquals(rectangleGeometry, rectangleGeometry2);

            EllipseGeometry ellipseGeometry = firstGeometry as EllipseGeometry;
            EllipseGeometry ellipseGeometry2 = secondGeometry as EllipseGeometry;
            if (ellipseGeometry != null && ellipseGeometry2 != null)
                return EllipseGeometryEquals(ellipseGeometry, ellipseGeometry2);

            GeometryGroup geometryGroup = firstGeometry as GeometryGroup;
            GeometryGroup geometryGroup2 = secondGeometry as GeometryGroup;
            return geometryGroup != null && geometryGroup2 != null && GeometryGroupEquals(geometryGroup, geometryGroup2);
        }

        public static bool PathGeometryEquals(PathGeometry firstGeometry, PathGeometry secondGeometry)
        {
            if (firstGeometry.FillRule != secondGeometry.FillRule)
                return false;

            if (firstGeometry.Figures.Count != secondGeometry.Figures.Count)
                return false;

            for (int i = 0; i < firstGeometry.Figures.Count; i++)
            {
                if (!PathFigureEquals(firstGeometry.Figures[i], secondGeometry.Figures[i]))
                    return false;
            }

            return true;
        }

        private static bool PathFigureEquals(PathFigure firstFigure, PathFigure secondFigure)
        {
            if (firstFigure.IsClosed != secondFigure.IsClosed)
                return false;

            if (firstFigure.IsFilled != secondFigure.IsFilled)
                return false;

            if (firstFigure.StartPoint != secondFigure.StartPoint)
                return false;

            for (int i = 0; i < firstFigure.Segments.Count; i++)
            {
                if (!PathSegmentEquals(firstFigure.Segments[i], secondFigure.Segments[i]))
                    return false;
            }

            return true;
        }

        private static bool PathSegmentEquals(PathSegment firstSegment, PathSegment secondSegment)
        {
            if (firstSegment == secondSegment)
                return true;

            if (firstSegment == null || secondSegment == null)
                return false;

            if (firstSegment.GetType() != secondSegment.GetType())
                return false;

            if (firstSegment.IsStroked() != secondSegment.IsStroked())
                return false;

            if (firstSegment.IsSmoothJoin() != secondSegment.IsSmoothJoin())
                return false;

            LineSegment lineSegment = firstSegment as LineSegment;
            LineSegment lineSegment2 = secondSegment as LineSegment;
            if (lineSegment != null && lineSegment2 != null)
                return LineSegmentEquals(lineSegment, lineSegment2);

            BezierSegment bezierSegment = firstSegment as BezierSegment;
            BezierSegment bezierSegment2 = secondSegment as BezierSegment;
            if (bezierSegment != null && bezierSegment2 != null)
                return BezierSegmentEquals(bezierSegment, bezierSegment2);

            QuadraticBezierSegment quadraticBezierSegment = firstSegment as QuadraticBezierSegment;
            QuadraticBezierSegment quadraticBezierSegment2 = secondSegment as QuadraticBezierSegment;
            if (quadraticBezierSegment != null && quadraticBezierSegment2 != null)
                return QuadraticBezierSegmentEquals(quadraticBezierSegment, quadraticBezierSegment2);

            ArcSegment arcSegment = firstSegment as ArcSegment;
            ArcSegment arcSegment2 = secondSegment as ArcSegment;
            if (arcSegment != null && arcSegment2 != null)
                return ArcSegmentEquals(arcSegment, arcSegment2);

            PolyLineSegment polyLineSegment = firstSegment as PolyLineSegment;
            PolyLineSegment polyLineSegment2 = secondSegment as PolyLineSegment;
            if (polyLineSegment != null && polyLineSegment2 != null)
                return PolyLineSegmentEquals(polyLineSegment, polyLineSegment2);

            PolyBezierSegment polyBezierSegment = firstSegment as PolyBezierSegment;
            PolyBezierSegment polyBezierSegment2 = secondSegment as PolyBezierSegment;
            if (polyBezierSegment != null && polyBezierSegment2 != null)
                return PolyBezierSegmentEquals(polyBezierSegment, polyBezierSegment2);

            PolyQuadraticBezierSegment polyQuadraticBezierSegment = firstSegment as PolyQuadraticBezierSegment;
            PolyQuadraticBezierSegment polyQuadraticBezierSegment2 = secondSegment as PolyQuadraticBezierSegment;
            return polyQuadraticBezierSegment != null && polyQuadraticBezierSegment2 != null && GeometryHelper.PolyQuadraticBezierSegmentEquals(polyQuadraticBezierSegment, polyQuadraticBezierSegment2);
        }

        private static bool LineSegmentEquals(LineSegment firstLineSegment, LineSegment secondLineSegment) => firstLineSegment.Point == secondLineSegment.Point;

        private static bool BezierSegmentEquals(BezierSegment firstBezierSegment, BezierSegment secondBezierSegment) => firstBezierSegment.Point1 == secondBezierSegment.Point1 && firstBezierSegment.Point2 == secondBezierSegment.Point2 && firstBezierSegment.Point3 == secondBezierSegment.Point3;

        private static bool QuadraticBezierSegmentEquals(QuadraticBezierSegment firstQuadraticBezierSegment, QuadraticBezierSegment secondQuadraticBezierSegment) => firstQuadraticBezierSegment.Point1 == secondQuadraticBezierSegment.Point1 && firstQuadraticBezierSegment.Point1 == secondQuadraticBezierSegment.Point1;

        private static bool ArcSegmentEquals(ArcSegment firstArcSegment, ArcSegment secondArcSegment) => firstArcSegment.Point == secondArcSegment.Point && firstArcSegment.IsLargeArc == secondArcSegment.IsLargeArc && firstArcSegment.RotationAngle == secondArcSegment.RotationAngle && firstArcSegment.Size == secondArcSegment.Size && firstArcSegment.SweepDirection == secondArcSegment.SweepDirection;

        private static bool PolyLineSegmentEquals(PolyLineSegment firstPolyLineSegment, PolyLineSegment secondPolyLineSegment)
        {
            if (firstPolyLineSegment.Points.Count != secondPolyLineSegment.Points.Count)
                return false;

            for (int i = 0; i < firstPolyLineSegment.Points.Count; i++)
            {
                if (firstPolyLineSegment.Points[i] != secondPolyLineSegment.Points[i])
                    return false;
            }

            return true;
        }

        private static bool PolyBezierSegmentEquals(PolyBezierSegment firstPolyBezierSegment, PolyBezierSegment secondPolyBezierSegment)
        {
            if (firstPolyBezierSegment.Points.Count != secondPolyBezierSegment.Points.Count)
                return false;

            for (int i = 0; i < firstPolyBezierSegment.Points.Count; i++)
            {
                if (firstPolyBezierSegment.Points[i] != secondPolyBezierSegment.Points[i])
                    return false;
            }

            return true;
        }

        private static bool PolyQuadraticBezierSegmentEquals(PolyQuadraticBezierSegment firstPolyQuadraticBezierSegment, PolyQuadraticBezierSegment secondPolyQuadraticBezierSegment)
        {
            if (firstPolyQuadraticBezierSegment.Points.Count != secondPolyQuadraticBezierSegment.Points.Count)
                return false;

            for (int i = 0; i < firstPolyQuadraticBezierSegment.Points.Count; i++)
            {
                if (firstPolyQuadraticBezierSegment.Points[i] != secondPolyQuadraticBezierSegment.Points[i])
                    return false;
            }

            return true;
        }

        private static bool EllipseGeometryEquals(EllipseGeometry firstGeometry, EllipseGeometry secondGeometry) => firstGeometry.Center == secondGeometry.Center && firstGeometry.RadiusX == secondGeometry.RadiusX && firstGeometry.RadiusY == secondGeometry.RadiusY;

        private static bool RectangleGeometryEquals(RectangleGeometry firstGeometry, RectangleGeometry secondGeometry) => firstGeometry.Rect == secondGeometry.Rect && firstGeometry.RadiusX == secondGeometry.RadiusX && firstGeometry.RadiusY == secondGeometry.RadiusY;

        private static bool LineGeometryEquals(LineGeometry firstGeometry, LineGeometry secondGeometry) => firstGeometry.StartPoint == secondGeometry.StartPoint && firstGeometry.EndPoint == secondGeometry.EndPoint;

        private static bool GeometryGroupEquals(GeometryGroup firstGeometry, GeometryGroup secondGeometry)
        {
            if (firstGeometry.FillRule != secondGeometry.FillRule)
                return false;

            if (firstGeometry.Children.Count != secondGeometry.Children.Count)
                return false;

            for (int i = 0; i < firstGeometry.Children.Count; i++)
            {
                if (!GeometryEquals(firstGeometry.Children[i], secondGeometry.Children[i]))
                    return false;
            }

            return true;
        }

        public static bool EnsureGeometryType<T>(out T result, ref Geometry value, Func<T> factory) where T : Geometry
        {
            result = value as T;
            if (result == null)
            {
                value = result = factory();
                return true;
            }

            return false;
        }

        public static bool EnsureSegmentType<T>(out T result, IList<PathSegment> list, int index, Func<T> factory) where T : PathSegment
        {
            result = list[index] as T;
            if (result == null)
            {
                list[index] = result = factory();
                return true;
            }

            return false;
        }
    }
}
