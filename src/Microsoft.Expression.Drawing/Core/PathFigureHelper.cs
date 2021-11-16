using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Core
{
    public static class PathFigureHelper
    {
        public static void FlattenFigure(PathFigure figure, IList<Point> points, double tolerance, bool removeRepeat)
        {
            if (figure == null)
            {
                throw new ArgumentNullException("figure");
            }
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (tolerance < 0.0)
            {
                throw new ArgumentOutOfRangeException("tolerance");
            }
            IList<Point> list = removeRepeat ? new List<Point>() : points;
            list.Add(figure.StartPoint);
            foreach (PathSegmentData pathSegmentData in figure.AllSegments())
            {
                pathSegmentData.PathSegment.FlattenSegment(list, pathSegmentData.StartPoint, tolerance);
            }
            if (figure.IsClosed)
            {
                list.Add(figure.StartPoint);
            }
            if (removeRepeat && list.Count > 0)
            {
                points.Add(list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    double value = GeometryHelper.SquaredDistance(points.Last<Point>(), list[i]);
                    if (!MathHelper.IsVerySmall(value))
                    {
                        points.Add(list[i]);
                    }
                }
            }
        }

        // Token: 0x0600016A RID: 362 RVA: 0x00008EF4 File Offset: 0x000070F4
        public static IEnumerable<PathSegmentData> AllSegments(this PathFigure figure)
        {
            if (figure != null && figure.Segments.Count > 0)
            {
                Point startPoint = figure.StartPoint;
                foreach (PathSegment segment in figure.Segments)
                {
                    Point lastPoint = segment.GetLastPoint();
                    yield return new PathSegmentData(startPoint, segment);
                    startPoint = lastPoint;
                }
            }
            yield break;
        }

        // Token: 0x0600016B RID: 363 RVA: 0x00008F18 File Offset: 0x00007118
        internal static bool SyncPolylineFigure(PathFigure figure, IList<Point> points, bool isClosed, bool isFilled = true)
        {
            if (figure == null)
            {
                throw new ArgumentNullException("figure");
            }
            bool flag = false;
            if (points == null || points.Count == 0)
            {
                flag |= figure.ClearIfSet(PathFigure.StartPointProperty);
                flag |= figure.Segments.EnsureListCount(0, null);
            }
            else
            {
                flag |= figure.SetIfDifferent(PathFigure.StartPointProperty, points[0]);
                flag |= figure.Segments.EnsureListCount(1, () => new PolyLineSegment());
                flag |= PathSegmentHelper.SyncPolylineSegment(figure.Segments, 0, points, 1, points.Count - 1);
            }
            flag |= figure.SetIfDifferent(PathFigure.IsClosedProperty, isClosed);
            return flag | figure.SetIfDifferent(PathFigure.IsFilledProperty, isFilled);
        }

        // Token: 0x0600016C RID: 364 RVA: 0x00009000 File Offset: 0x00007200
        internal static bool SyncEllipseFigure(PathFigure figure, Rect bounds, SweepDirection sweepDirection, bool isFilled = true)
        {
            bool flag = false;
            Point[] array = new Point[2];
            Size size = new Size(bounds.Width / 2.0, bounds.Height / 2.0);
            Point point = bounds.Center();
            if (size.Width > size.Height)
            {
                array[0] = new Point(bounds.Left, point.Y);
                array[1] = new Point(bounds.Right, point.Y);
            }
            else
            {
                array[0] = new Point(point.X, bounds.Top);
                array[1] = new Point(point.X, bounds.Bottom);
            }
            flag |= figure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            flag |= figure.SetIfDifferent(PathFigure.IsFilledProperty, isFilled);
            flag |= figure.SetIfDifferent(PathFigure.StartPointProperty, array[0]);
            flag |= figure.Segments.EnsureListCount(2, () => new ArcSegment());
            ArcSegment dependencyObject;
            flag |= GeometryHelper.EnsureSegmentType<ArcSegment>(out dependencyObject, figure.Segments, 0, () => new ArcSegment());
            flag |= dependencyObject.SetIfDifferent(ArcSegment.PointProperty, array[1]);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.SizeProperty, size);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.SweepDirectionProperty, sweepDirection);
            flag |= GeometryHelper.EnsureSegmentType<ArcSegment>(out dependencyObject, figure.Segments, 1, () => new ArcSegment());
            flag |= dependencyObject.SetIfDifferent(ArcSegment.PointProperty, array[0]);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.SizeProperty, size);
            flag |= dependencyObject.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            return flag | dependencyObject.SetIfDifferent(ArcSegment.SweepDirectionProperty, sweepDirection);
        }
    }
}
