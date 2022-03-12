using Microsoft.Expression.Drawing.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Media
{
    internal class LineArrowGeometrySource : GeometrySource<ILineArrowGeometrySourceParameters>
    {
        protected override bool UpdateCachedGeometry(ILineArrowGeometrySourceParameters parameters)
        {
            bool flag = false;
            PathGeometry pathGeometry;
            flag |= GeometryHelper.EnsureGeometryType<PathGeometry>(out pathGeometry, ref cachedGeometry, () => new PathGeometry());
            flag |= pathGeometry.Figures.EnsureListCount(3, () => new PathFigure());
            Point startPoint = GetStartPoint(parameters);
            Point endPoint = GetEndPoint(parameters);
            Point middlePoint = GetMiddlePoint(parameters);
            PathFigure pathFigure = pathGeometry.Figures[0];
            flag |= pathFigure.SetIfDifferent(PathFigure.StartPointProperty, startPoint);
            flag |= pathFigure.SetIfDifferent(PathFigure.IsClosedProperty, false);
            flag |= pathFigure.SetIfDifferent(PathFigure.IsFilledProperty, false);
            flag |= pathFigure.Segments.EnsureListCount(1, () => new QuadraticBezierSegment());
            QuadraticBezierSegment dependencyObject;
            flag |= GeometryHelper.EnsureSegmentType<QuadraticBezierSegment>(out dependencyObject, pathFigure.Segments, 0, () => new QuadraticBezierSegment());
            flag |= dependencyObject.SetIfDifferent(QuadraticBezierSegment.Point1Property, middlePoint);
            flag |= dependencyObject.SetIfDifferent(QuadraticBezierSegment.Point2Property, endPoint);
            flag |= UpdateArrow(parameters.StartArrow, parameters.ArrowSize, pathGeometry.Figures[1], startPoint, startPoint.Subtract(middlePoint).Normalized());
            flag |= UpdateArrow(parameters.EndArrow, parameters.ArrowSize, pathGeometry.Figures[2], endPoint, endPoint.Subtract(middlePoint).Normalized());
            return true;
        }

        private static bool UpdateArrow(ArrowType arrowType, double size, PathFigure figure, Point startPoint, Vector tangent)
        {
            bool flag = false;
            switch (arrowType)
            {
                case ArrowType.NoArrow:
                    flag |= figure.SetIfDifferent(PathFigure.StartPointProperty, startPoint);
                    flag |= figure.Segments.EnsureListCount(0, null);
                    break;
                default:
                    {
                        Point[] pointTrio = GetPointTrio(startPoint, tangent, size);
                        if (arrowType == ArrowType.StealthArrow)
                        {
                            flag |= PathFigureHelper.SyncPolylineFigure(figure, new List<Point>(pointTrio)
                    {
                        startPoint - tangent * size * 2.0 / 3.0
                    }, true, true);
                        }
                        else
                        {
                            bool flag2 = arrowType == ArrowType.OpenArrow;
                            flag |= PathFigureHelper.SyncPolylineFigure(figure, pointTrio, !flag2, !flag2);
                        }
                        break;
                    }
                case ArrowType.OvalArrow:
                    {
                        Rect bounds = new Rect(startPoint.X - size / 2.0, startPoint.Y - size / 2.0, size, size);
                        bool flag3 = flag;
                        bool isFilled = true;
                        flag = flag3 | PathFigureHelper.SyncEllipseFigure(figure, bounds, SweepDirection.Clockwise, isFilled);
                        break;
                    }
            }
            return flag;
        }

        private static Point[] GetPointTrio(Point startPoint, Vector tangent, double size)
        {
            Vector vector = tangent.Perpendicular().Normalized() * 0.57735;
            return new Point[]
            {
                startPoint - tangent * size + vector * size,
                startPoint,
                startPoint - tangent * size - vector * size
            };
        }

        private Point GetMiddlePoint(ILineArrowGeometrySourceParameters parameters)
        {
            Rect logicalBounds = LogicalBounds;
            double alpha = (parameters.BendAmount + 1.0) / 2.0;
            switch (parameters.StartCorner)
            {
                case CornerType.TopLeft:
                    return GeometryHelper.Lerp(logicalBounds.BottomLeft(), logicalBounds.TopRight(), alpha);
                case CornerType.TopRight:
                    return GeometryHelper.Lerp(logicalBounds.TopLeft(), logicalBounds.BottomRight(), alpha);
                case CornerType.BottomRight:
                    return GeometryHelper.Lerp(logicalBounds.TopRight(), logicalBounds.BottomLeft(), alpha);
                case CornerType.BottomLeft:
                    return GeometryHelper.Lerp(logicalBounds.BottomRight(), logicalBounds.TopLeft(), alpha);
                default:
                    return logicalBounds.Center();
            }
        }

        private Point GetEndPoint(ILineArrowGeometrySourceParameters parameters)
        {
            Rect logicalBounds = LogicalBounds;
            switch (parameters.StartCorner)
            {
                case CornerType.TopLeft:
                    return logicalBounds.BottomRight();
                case CornerType.TopRight:
                    return logicalBounds.BottomLeft();
                case CornerType.BottomRight:
                    return logicalBounds.TopLeft();
                case CornerType.BottomLeft:
                    return logicalBounds.TopRight();
                default:
                    return logicalBounds.BottomRight();
            }
        }

        private Point GetStartPoint(ILineArrowGeometrySourceParameters parameters)
        {
            Rect logicalBounds = LogicalBounds;
            switch (parameters.StartCorner)
            {
                case CornerType.TopLeft:
                    return logicalBounds.TopLeft();
                case CornerType.TopRight:
                    return logicalBounds.TopRight();
                case CornerType.BottomRight:
                    return logicalBounds.BottomRight();
                case CornerType.BottomLeft:
                    return logicalBounds.BottomLeft();
                default:
                    return logicalBounds.BottomRight();
            }
        }
    }
}
