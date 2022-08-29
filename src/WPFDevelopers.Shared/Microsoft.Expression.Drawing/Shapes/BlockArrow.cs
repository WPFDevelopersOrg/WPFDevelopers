using System.Windows;
using Microsoft.Expression.Drawing.Media;

namespace Microsoft.Expression.Drawing.Shapes
{
    public sealed class BlockArrow : PrimitiveShape, IBlockArrowGeometrySourceParameters, IGeometrySourceParameters
    {

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(ArrowOrientation), typeof(BlockArrow), new DrawingPropertyMetadata(ArrowOrientation.Right, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowheadAngleProperty = DependencyProperty.Register("ArrowheadAngle", typeof(double), typeof(BlockArrow), new DrawingPropertyMetadata(90.0, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowBodySizeProperty = DependencyProperty.Register("ArrowBodySize", typeof(double), typeof(BlockArrow), new DrawingPropertyMetadata(0.5, DrawingPropertyMetadataOptions.AffectsRender));

        public ArrowOrientation Orientation
        {
            get => (ArrowOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public double ArrowheadAngle
        {
            get => (double)GetValue(ArrowheadAngleProperty);
            set => SetValue(ArrowheadAngleProperty, value);
        }

        public double ArrowBodySize
        {
            get => (double)GetValue(ArrowBodySizeProperty);
            set => SetValue(ArrowBodySizeProperty, value);
        }

        protected override IGeometrySource CreateGeometrySource()
        {
            return new BlockArrowGeometrySource();
        }



    }
}
