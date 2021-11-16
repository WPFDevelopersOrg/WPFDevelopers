using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Core
{
    public sealed class PathSegmentData
    {
        public PathSegmentData(Point startPoint, PathSegment pathSegment)
        {
            PathSegment = pathSegment;
            StartPoint = startPoint;
        }

        public Point StartPoint { get; private set; }

        public PathSegment PathSegment { get; private set; }
    }
}
