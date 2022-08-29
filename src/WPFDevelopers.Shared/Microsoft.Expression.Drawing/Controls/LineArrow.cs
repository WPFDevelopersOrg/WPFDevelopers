using Microsoft.Expression.Drawing.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Expression.Drawing.Controls
{
	public sealed class LineArrow : CompositeShape, ILineArrowGeometrySourceParameters, IGeometrySourceParameters
	{
		public LineArrow()
		{
			DefaultStyleKey = typeof(LineArrow);
		}


		protected override IGeometrySource CreateGeometrySource() => new LineArrowGeometrySource();

		public double BendAmount
		{
			get => (double)GetValue(BendAmountProperty);
			set => SetValue(BendAmountProperty, value);
		}

		public ArrowType StartArrow
		{
			get => (ArrowType)GetValue(StartArrowProperty);
			set => SetValue(StartArrowProperty, value);
		}

		public ArrowType EndArrow
		{
			get => (ArrowType)GetValue(EndArrowProperty);
			set => SetValue(EndArrowProperty, value);
		}

		public CornerType StartCorner
		{
			get => (CornerType)GetValue(StartCornerProperty);
			set => SetValue(StartCornerProperty, value);
		}

		public double ArrowSize
		{
			get => (double)GetValue(ArrowSizeProperty);
			set => SetValue(ArrowSizeProperty, value);
		}
		protected override Size MeasureOverride(Size availableSize) => base.MeasureOverride(new Size(0.0, 0.0));


		public static readonly DependencyProperty BendAmountProperty = DependencyProperty.Register("BendAmount", typeof(double), typeof(LineArrow), new DrawingPropertyMetadata(0.5, DrawingPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty StartArrowProperty = DependencyProperty.Register("StartArrow", typeof(ArrowType), typeof(LineArrow), new DrawingPropertyMetadata(ArrowType.NoArrow, DrawingPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty EndArrowProperty = DependencyProperty.Register("EndArrow", typeof(ArrowType), typeof(LineArrow), new DrawingPropertyMetadata(ArrowType.Arrow, DrawingPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty StartCornerProperty = DependencyProperty.Register("StartCorner", typeof(CornerType), typeof(LineArrow), new DrawingPropertyMetadata(CornerType.TopLeft, DrawingPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ArrowSizeProperty = DependencyProperty.Register("ArrowSize", typeof(double), typeof(LineArrow), new DrawingPropertyMetadata(10.0, DrawingPropertyMetadataOptions.AffectsRender));
	}
}
