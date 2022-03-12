using Microsoft.Expression.Drawing.Core;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Controls
{
    internal class PolylineStrategy : ShapeStrategy
    {
        private PointCollection PointsListener
        {
            get
            {
                return (PointCollection)base.GetValue(PolylineStrategy.PointsListenerProperty);
            }
            set
            {
                base.SetValue(PolylineStrategy.PointsListenerProperty, value);
            }
        }

        public PolylineStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            this.sourcePolyline = (Polyline)layoutPath.SourceElement;
            base.SetListenerBinding(PolylineStrategy.PointsListenerProperty, "Points");
        }

        public override bool HasGeometryChanged()
        {
            return base.HasGeometryChanged() || !PolylineStrategy.PointCollectionsEqual(this.oldPoints, this.sourcePolyline.Points);
        }

        protected override PathGeometry UpdateGeometry()
        {
            this.oldPoints = PolylineStrategy.ClonePointCollection(this.sourcePolyline.Points);
            return base.UpdateGeometry();
        }

        public static PointCollection ClonePointCollection(PointCollection points)
        {
            if (points == null)
            {
                return null;
            }
            PointCollection pointCollection = new PointCollection();
            foreach (Point value in points)
            {
                pointCollection.Add(value);
            }
            return pointCollection;
        }

        public static bool PointCollectionsEqual(PointCollection collectionOne, PointCollection collectionTwo)
        {
            if (collectionOne == collectionTwo)
            {
                return true;
            }
            if (collectionOne == null || collectionTwo == null)
            {
                return false;
            }
            if (collectionOne.Count != collectionTwo.Count)
            {
                return false;
            }
            for (int i = 0; i < collectionOne.Count; i++)
            {
                if (collectionOne[i] != collectionTwo[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static PathGeometry CreatePolylinePathGeometry(PointCollection points, bool isClosed)
        {
            if (points == null || points.Count == 0)
            {
                return new PathGeometry();
            }
            PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
            if (points.Count > 1)
            {
                pathSegmentCollection.Add(PathSegmentHelper.CreatePolylineSegment(points, 1, points.Count - 1, false));
            }
            return new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    new PathFigure
                    {
                        StartPoint = points[0],
                        IsClosed = isClosed,
                        Segments = pathSegmentCollection
                    }
                }
            };
        }

        private PointCollection oldPoints;

        private Polyline sourcePolyline;

        private static readonly DependencyProperty PointsListenerProperty = DependencyProperty.Register("PointsListener", typeof(PointCollection), typeof(PolylineStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));
    }
}
