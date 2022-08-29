using System.Windows;
using Microsoft.Expression.Drawing.Media;

namespace Microsoft.Expression.Drawing.Shapes
{
    public sealed class RegularPolygon : PrimitiveShape, IPolygonGeometrySourceParameters, IGeometrySourceParameters
    {
        public static readonly DependencyProperty PointCountProperty = DependencyProperty.Register("PointCount", typeof(double), typeof(RegularPolygon), new DrawingPropertyMetadata(6.0, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register("InnerRadius", typeof(double), typeof(RegularPolygon), new DrawingPropertyMetadata(1.0, DrawingPropertyMetadataOptions.AffectsRender));
        public double PointCount
        {
            get => (double)GetValue(PointCountProperty);
            set => SetValue(PointCountProperty, value);
        }

        public double InnerRadius
        {
            get => (double)GetValue(InnerRadiusProperty);
            set => SetValue(InnerRadiusProperty, value);
        }

        protected override IGeometrySource CreateGeometrySource()
        {
            return new PolygonGeometrySource();
        }
    }
}
