
using System.Windows;
using Microsoft.Expression.Drawing.Media;

namespace Microsoft.Expression.Drawing.Shapes
{
    public sealed class Arc : PrimitiveShape, IArcGeometrySourceParameters, IGeometrySourceParameters
    {
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(double), typeof(Arc), new DrawingPropertyMetadata(0.0, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register("EndAngle", typeof(double), typeof(Arc), new DrawingPropertyMetadata(90.0, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArcThicknessProperty = DependencyProperty.Register("ArcThickness", typeof(double), typeof(Arc), new DrawingPropertyMetadata(0.0, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArcThicknessUnitProperty = DependencyProperty.Register("ArcThicknessUnit", typeof(UnitType), typeof(Arc), new DrawingPropertyMetadata(UnitType.Pixel, DrawingPropertyMetadataOptions.AffectsRender));

        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }

        public double EndAngle
        {
            get => (double)GetValue(EndAngleProperty);
            set => SetValue(EndAngleProperty, value);
        }

        public double ArcThickness
        {
            get => (double)GetValue(ArcThicknessProperty);
            set => SetValue(ArcThicknessProperty, value);
        }

        public UnitType ArcThicknessUnit
        {
            get => (UnitType)GetValue(ArcThicknessUnitProperty);
            set => SetValue(ArcThicknessUnitProperty, value);
        }

        protected override IGeometrySource CreateGeometrySource() => new ArcGeometrySource();
    }
}
