using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Drawing.Core
{
    public class SimpleSegment
    {
        public SegmentType Type { get; private set; }

        public Point[] Points { get; private set; }

        public void Flatten(IList<Point> resultPolyline, double tolerance, IList<double> resultParameters)
        {
            switch (Type)
            {
                case SegmentType.Line:
                    resultPolyline.Add(Points[1]);
                    if (resultParameters != null)
                    {
                        resultParameters.Add(1.0);
                        return;
                    }
                    break;
                case SegmentType.CubicBeizer:
                    BezierCurveFlattener.FlattenCubic(Points, tolerance, resultPolyline, true, resultParameters);
                    break;
                default:
                    return;
            }
        }

        private SimpleSegment()
        {
        }

        public static SimpleSegment Create(Point point0, Point point1)
        {
            return new SimpleSegment
            {
                Type = SegmentType.Line,
                Points = new Point[]
                {
                    point0,
                    point1
                }
            };
        }

        public static SimpleSegment Create(Point point0, Point point1, Point point2)
        {
            Point point3 = GeometryHelper.Lerp(point0, point1, 0.6666666666666666);
            Point point4 = GeometryHelper.Lerp(point1, point2, 0.3333333333333333);
            return new SimpleSegment
            {
                Type = SegmentType.CubicBeizer,
                Points = new Point[]
                {
                    point0,
                    point3,
                    point4,
                    point2
                }
            };
        }

        public static SimpleSegment Create(Point point0, Point point1, Point point2, Point point3)
        {
            return new SimpleSegment
            {
                Type = SegmentType.CubicBeizer,
                Points = new Point[]
                {
                    point0,
                    point1,
                    point2,
                    point3
                }
            };
        }

        public enum SegmentType
        {
            Line,
            CubicBeizer
        }
    }
}
