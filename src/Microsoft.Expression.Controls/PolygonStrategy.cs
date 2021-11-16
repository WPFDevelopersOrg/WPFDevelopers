using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Controls
{
    internal class PolygonStrategy : ShapeStrategy
    {
        private PointCollection PointsListener
        {
            get
            {
                return (PointCollection)base.GetValue(PolygonStrategy.PointsListenerProperty);
            }
            set
            {
                base.SetValue(PolygonStrategy.PointsListenerProperty, value);
            }
        }

        public PolygonStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            this.sourcePolygon = (Polygon)layoutPath.SourceElement;
            base.SetListenerBinding(PolygonStrategy.PointsListenerProperty, "Points");
        }

        public override bool HasGeometryChanged()
        {
            return base.HasGeometryChanged() || !PolylineStrategy.PointCollectionsEqual(this.oldPoints, this.sourcePolygon.Points);
        }

        protected override PathGeometry UpdateGeometry()
        {
            this.oldPoints = PolylineStrategy.ClonePointCollection(this.sourcePolygon.Points);
            return base.UpdateGeometry();
        }

        private PointCollection oldPoints;

        private Polygon sourcePolygon;

        private static readonly DependencyProperty PointsListenerProperty = DependencyProperty.Register("PointsListener", typeof(PointCollection), typeof(PolygonStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));
    }
}
