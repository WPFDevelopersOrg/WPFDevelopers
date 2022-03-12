using Microsoft.Expression.Drawing.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Media
{
    internal class CalloutGeometrySource : GeometrySource<ICalloutGeometrySourceParameters>
    {
        protected override bool UpdateCachedGeometry(ICalloutGeometrySourceParameters parameters)
        {
            bool flag = false;
            switch (parameters.CalloutStyle)
            {
                case CalloutStyle.Rectangle:
                    flag |= UpdateRectangleCallout(parameters);
                    break;
                case CalloutStyle.RoundedRectangle:
                    flag |= UpdateRoundedRectangleCallout(parameters);
                    break;
                default:
                    flag |= UpdateOvalCallout(parameters);
                    break;
                case CalloutStyle.Cloud:
                    flag |= UpdateCloudCallout(parameters);
                    break;
            }
            if (flag)
                cachedGeometry = PathGeometryHelper.FixPathGeometryBoundary(cachedGeometry);

            return flag;
        }

        private bool UpdateOvalCallout(ICalloutGeometrySourceParameters parameters)
        {
            bool flag = false;
            if (IsInside(parameters.CalloutStyle, parameters.AnchorPoint))
            {
                EllipseGeometry ellipseGeometry = cachedGeometry as EllipseGeometry;
                if (ellipseGeometry == null)
                {
                    ellipseGeometry = (EllipseGeometry)(cachedGeometry = new EllipseGeometry());
                    flag = true;
                }
                flag |= ellipseGeometry.SetIfDifferent(EllipseGeometry.CenterProperty, LogicalBounds.Center());
                flag |= ellipseGeometry.SetIfDifferent(EllipseGeometry.RadiusXProperty, LogicalBounds.Width / 2.0);
                flag |= ellipseGeometry.SetIfDifferent(EllipseGeometry.RadiusYProperty, LogicalBounds.Height / 2.0);
            }
            else
            {
                PathGeometry pathGeometry = cachedGeometry as PathGeometry;
                PathFigure pathFigure;
                ArcSegment arcSegment;
                LineSegment dependencyObject;
                if (pathGeometry == null || pathGeometry.Figures.Count != 1 || (pathFigure = pathGeometry.Figures[0]).Segments.Count != 2 || (arcSegment = pathFigure.Segments[0] as ArcSegment) == null || (dependencyObject = pathFigure.Segments[1] as LineSegment) == null)
                {
                    pathGeometry = (PathGeometry)(cachedGeometry = new PathGeometry());
                    pathGeometry.Figures.Add(pathFigure = new PathFigure());
                    pathFigure.Segments.Add(arcSegment = new ArcSegment());
                    pathFigure.Segments.Add(dependencyObject = new LineSegment());
                    pathFigure.IsClosed = true;
                    arcSegment.IsLargeArc = true;
                    arcSegment.SweepDirection = SweepDirection.Clockwise;
                    flag = true;
                }
                double arcAngle = GeometryHelper.GetArcAngle(parameters.AnchorPoint);
                double degree = arcAngle + 10.0;
                double degree2 = arcAngle - 10.0;
                flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, GeometryHelper.GetArcPoint(degree, LogicalBounds));
                flag |= arcSegment.SetIfDifferent(ArcSegment.PointProperty, GeometryHelper.GetArcPoint(degree2, LogicalBounds));
                flag |= arcSegment.SetIfDifferent(ArcSegment.SizeProperty, LogicalBounds.Resize(0.5).Size());
                flag |= dependencyObject.SetIfDifferent(LineSegment.PointProperty, GetAbsoluteAnchorPoint(parameters.AnchorPoint));
                cachedGeometry = pathGeometry;
            }
            return flag;
        }

        private static Point ClosestConnectionPoint(Point relativePoint)
        {
            double num = double.MaxValue;
            Point result = connectionPoints[0];
            foreach (Point point in connectionPoints)
            {
                double num2 = GeometryHelper.Distance(relativePoint, point);
                if (num > num2)
                {
                    num = num2;
                    result = point;
                }
            }
            return result;
        }

        private static bool UpdateEdge(PathSegmentCollection segments, int index, Point start, Point end, Point anchorPoint, double connection, bool connectToAnchor)
        {
            bool flag = false;
            if (connectToAnchor)
            {
                flag |= UpdatePolylineSegment(segments, index, start, end, anchorPoint, connection);
            }
            else
            {
                flag |= UpdateLineSegment(segments, index, end);
            }
            return flag;
        }

        private static bool UpdatePolylineSegment(PathSegmentCollection segments, int index, Point start, Point end, Point anchor, double connection)
        {
            bool flag = false;
            Point[] array = new Point[]
            {
                GeometryHelper.Lerp(start, end, connection - 0.1),
                anchor,
                GeometryHelper.Lerp(start, end, connection + 0.1),
                end
            };
            return flag | PathSegmentHelper.SyncPolylineSegment(segments, index, array, 0, array.Length);
        }

        private static bool UpdateLineSegment(PathSegmentCollection segments, int index, Point point)
        {
            bool flag = false;
            LineSegment lineSegment = segments[index] as LineSegment;
            if (lineSegment == null)
            {
                lineSegment = (LineSegment)(segments[index] = new LineSegment());
                flag = true;
            }
            return flag | lineSegment.SetIfDifferent(LineSegment.PointProperty, point);
        }

        private bool UpdateRectangleCallout(ICalloutGeometrySourceParameters parameters)
        {
            bool flag = false;
            PathGeometry pathGeometry = cachedGeometry as PathGeometry;
            PathFigure pathFigure;
            PathSegmentCollection segments;
            if (pathGeometry == null || pathGeometry.Figures.Count != 1 || (pathFigure = pathGeometry.Figures[0]) == null || (segments = pathFigure.Segments).Count != 4)
            {
                pathGeometry = (PathGeometry)(cachedGeometry = new PathGeometry());
                segments = new PathSegmentCollection
                {
                    new LineSegment(),
                    new LineSegment(),
                    new LineSegment(),
                    new LineSegment()
                };
                pathFigure = new PathFigure
                {
                    Segments = segments
                };
                pathGeometry.Figures.Add(pathFigure);
                flag = true;
            }
            Point anchorPoint = parameters.AnchorPoint;
            Point point = ClosestConnectionPoint(anchorPoint);
            bool flag2 = IsInside(parameters.CalloutStyle, anchorPoint);
            Point absoluteAnchorPoint = GetAbsoluteAnchorPoint(anchorPoint);
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, LogicalBounds.TopLeft());
            flag |= pathFigure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            flag |= UpdateEdge(segments, 0, LogicalBounds.TopLeft(), LogicalBounds.TopRight(), absoluteAnchorPoint, point.X, !flag2 && point.Y == 0.0);
            flag |= UpdateEdge(segments, 1, LogicalBounds.TopRight(), LogicalBounds.BottomRight(), absoluteAnchorPoint, point.Y, !flag2 && point.X == 1.0);
            flag |= UpdateEdge(segments, 2, LogicalBounds.BottomRight(), LogicalBounds.BottomLeft(), absoluteAnchorPoint, 1.0 - point.X, !flag2 && point.Y == 1.0);
            return flag | UpdateEdge(segments, 3, LogicalBounds.BottomLeft(), LogicalBounds.TopLeft(), absoluteAnchorPoint, 1.0 - point.Y, !flag2 && point.X == 0.0);
        }

        private Point[] ComputeCorners(double radius)
        {
            double left = LogicalBounds.Left;
            double top = LogicalBounds.Top;
            double right = LogicalBounds.Right;
            double bottom = LogicalBounds.Bottom;
            return new Point[]
            {
                new Point(left, top + radius),
                new Point(left + radius, top),
                new Point(right - radius, top),
                new Point(right, top + radius),
                new Point(right, bottom - radius),
                new Point(right - radius, bottom),
                new Point(left + radius, bottom),
                new Point(left, bottom - radius)
            };
        }

        private static bool UpdateCornerArc(PathSegmentCollection segments, int index, Point start, Point end)
        {
            bool flag = false;
            ArcSegment arcSegment = segments[index] as ArcSegment;
            if (arcSegment == null)
            {
                arcSegment = (ArcSegment)(segments[index] = new ArcSegment());
                flag = true;
            }
            double width = Math.Abs(end.X - start.X);
            double height = Math.Abs(end.Y - start.Y);
            flag |= arcSegment.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            flag |= arcSegment.SetIfDifferent(ArcSegment.PointProperty, end);
            flag |= arcSegment.SetIfDifferent(ArcSegment.SizeProperty, new Size(width, height));
            return flag | arcSegment.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
        }

        private bool UpdateRoundedRectangleCallout(ICalloutGeometrySourceParameters parameters)
        {
            bool flag = false;
            double radius = Math.Min(LogicalBounds.Width, LogicalBounds.Height) / 10.0;
            Point[] array = ComputeCorners(radius);
            PathGeometry pathGeometry = cachedGeometry as PathGeometry;
            PathFigure pathFigure;
            PathSegmentCollection segments;
            if (pathGeometry == null || pathGeometry.Figures.Count != 1 || (pathFigure = pathGeometry.Figures[0]) == null || (segments = pathFigure.Segments).Count != 8)
            {
                pathGeometry = (PathGeometry)(cachedGeometry = new PathGeometry());
                segments = new PathSegmentCollection
                {
                    new ArcSegment(),
                    new LineSegment(),
                    new ArcSegment(),
                    new LineSegment(),
                    new ArcSegment(),
                    new LineSegment(),
                    new ArcSegment(),
                    new LineSegment()
                };
                pathFigure = new PathFigure
                {
                    Segments = segments
                };
                pathGeometry.Figures.Add(pathFigure);
                flag = true;
            }
            Point anchorPoint = parameters.AnchorPoint;
            Point point = ClosestConnectionPoint(anchorPoint);
            bool flag2 = IsInside(parameters.CalloutStyle, anchorPoint);
            Point absoluteAnchorPoint = GetAbsoluteAnchorPoint(anchorPoint);
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, array[0]);
            flag |= pathFigure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            flag |= UpdateCornerArc(segments, 0, array[0], array[1]);
            flag |= UpdateEdge(segments, 1, array[1], array[2], absoluteAnchorPoint, point.X, !flag2 && point.Y == 0.0);
            flag |= UpdateCornerArc(segments, 2, array[2], array[3]);
            flag |= UpdateEdge(segments, 3, array[3], array[4], absoluteAnchorPoint, point.Y, !flag2 && point.X == 1.0);
            flag |= UpdateCornerArc(segments, 4, array[4], array[5]);
            flag |= UpdateEdge(segments, 5, array[5], array[6], absoluteAnchorPoint, 1.0 - point.X, !flag2 && point.Y == 1.0);
            flag |= UpdateCornerArc(segments, 6, array[6], array[7]);
            return flag | UpdateEdge(segments, 7, array[7], array[0], absoluteAnchorPoint, 1.0 - point.Y, !flag2 && point.X == 0.0);
        }

        private bool UpdateCloudCallout(ICalloutGeometrySourceParameters parameters)
        {
            bool flag = false;
            int num = 3;
            if (IsInside(parameters.CalloutStyle, parameters.AnchorPoint))
            {
                num = 0;
            }
            PathGeometry pathGeometry;
            flag |= GeometryHelper.EnsureGeometryType<PathGeometry>(out pathGeometry, ref cachedGeometry, () => new PathGeometry());
            flag |= pathGeometry.SetIfDifferent(PathGeometry.FillRuleProperty, FillRule.Nonzero);
            flag |= pathGeometry.Figures.EnsureListCount(1 + num, () => new PathFigure());
            Point[] array = cloudPoints.ToArray<Point>();
            Transform transform = GeometryHelper.RelativeTransform(cloudBounds, LogicalBounds);
            array.ApplyTransform(transform);
            PathFigure pathFigure = pathGeometry.Figures[0];
            flag |= pathFigure.SetIfDifferent(PathFigure.IsFilledProperty, true);
            flag |= pathFigure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, transform.Transform(cloudStartPoint));
            flag |= pathFigure.Segments.EnsureListCount(1, () => new PolyBezierSegment());
            flag |= PathSegmentHelper.SyncPolyBezierSegment(pathFigure.Segments, 0, array, 0, array.Length);
            for (int i = 0; i < num; i++)
            {
                double alpha = (double)i / (double)num;
                Point point = GeometryHelper.Lerp(GetAbsoluteAnchorPoint(parameters.AnchorPoint), LogicalBounds.Center(), MathHelper.Lerp(0.0, 0.5, alpha));
                double alpha2 = MathHelper.Lerp(0.05, 0.2, alpha);
                double num2 = MathHelper.Lerp(0.0, LogicalBounds.Width / 2.0, alpha2);
                double num3 = MathHelper.Lerp(0.0, LogicalBounds.Height / 2.0, alpha2);
                Rect bounds = new Rect(point.X - num2, point.Y - num3, 2.0 * num2, 2.0 * num3);
                bool flag2 = flag;
                bool isFilled = true;
                flag = flag2 | PathFigureHelper.SyncEllipseFigure(pathGeometry.Figures[i + 1], bounds, SweepDirection.Counterclockwise, isFilled);
            }
            return flag;
        }

        private static bool IsInside(CalloutStyle style, Point point)
        {
            switch (style)
            {
                default:
                    return Math.Abs(point.X - 0.5) <= 0.5 && Math.Abs(point.Y - 0.5) <= 0.5;
                case CalloutStyle.Oval:
                case CalloutStyle.Cloud:
                    return GeometryHelper.Distance(point, new Point(0.5, 0.5)) <= 0.5;
            }
        }

        private Point GetAbsoluteAnchorPoint(Point relativePoint)
        {
            return GeometryHelper.RelativeToAbsolutePoint(LayoutBounds, relativePoint);
        }

        private const double centerRatioA = 0.0;

        private const double centerRatioB = 0.5;

        private const double radiusRatioA = 0.05;

        private const double radiusRatioB = 0.2;

        private static readonly Point[] connectionPoints = new Point[]
        {
            new Point(0.25, 0.0),
            new Point(0.75, 0.0),
            new Point(0.25, 1.0),
            new Point(0.75, 1.0),
            new Point(0.0, 0.25),
            new Point(0.0, 0.75),
            new Point(1.0, 0.25),
            new Point(1.0, 0.75)
        };

        private static readonly Point cloudStartPoint = new Point(86.42, 23.3);

        private static readonly Point[] cloudPoints = new Point[]
        {
            new Point(86.42, 23.18),
            new Point(86.44, 23.07),
            new Point(86.44, 22.95),
            new Point(86.44, 16.53),
            new Point(81.99, 11.32),
            new Point(76.51, 11.32),
            new Point(75.12, 11.32),
            new Point(73.79, 11.66),
            new Point(72.58, 12.27),
            new Point(70.81, 5.74),
            new Point(65.59, 1.0),
            new Point(59.43, 1.0),
            new Point(54.48, 1.0),
            new Point(50.15, 4.06),
            new Point(47.71, 8.65),
            new Point(46.62, 7.08),
            new Point(44.97, 6.08),
            new Point(43.11, 6.08),
            new Point(41.21, 6.08),
            new Point(39.53, 7.13),
            new Point(38.45, 8.76),
            new Point(35.72, 5.49),
            new Point(31.93, 3.46),
            new Point(27.73, 3.46),
            new Point(21.26, 3.46),
            new Point(15.77, 8.27),
            new Point(13.67, 14.99),
            new Point(13.36, 14.96),
            new Point(13.05, 14.94),
            new Point(12.73, 14.94),
            new Point(6.25, 14.94),
            new Point(1.0, 21.1),
            new Point(1.0, 28.69),
            new Point(1.0, 35.68),
            new Point(5.45, 41.44),
            new Point(11.21, 42.32),
            new Point(11.65, 47.61),
            new Point(15.45, 51.74),
            new Point(20.08, 51.74),
            new Point(22.49, 51.74),
            new Point(24.66, 50.63),
            new Point(26.27, 48.82),
            new Point(27.38, 53.36),
            new Point(30.95, 56.69),
            new Point(35.18, 56.69),
            new Point(39.0, 56.69),
            new Point(42.27, 53.98),
            new Point(43.7, 50.13),
            new Point(45.33, 52.69),
            new Point(47.92, 54.35),
            new Point(50.86, 54.35),
            new Point(55.0, 54.35),
            new Point(58.48, 51.03),
            new Point(59.49, 46.53),
            new Point(61.53, 51.17),
            new Point(65.65, 54.35),
            new Point(70.41, 54.35),
            new Point(77.09, 54.35),
            new Point(82.51, 48.1),
            new Point(82.69, 40.32),
            new Point(83.3, 40.51),
            new Point(83.95, 40.63),
            new Point(84.62, 40.63),
            new Point(88.77, 40.63),
            new Point(92.13, 36.69),
            new Point(92.13, 31.83),
            new Point(92.13, 27.7),
            new Point(89.69, 24.25),
            new Point(86.42, 23.3)
        };

        private static readonly Rect cloudBounds = new Rect(1.0, 1.0, 91.129997253418, 55.689998626709);
    }
}
