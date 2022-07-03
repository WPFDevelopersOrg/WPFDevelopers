using Microsoft.Expression.Drawing.Core;

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Expression.Drawing.Media;

namespace Microsoft.Expression.Drawing.Shapes
{
	public abstract class PrimitiveShape : Shape, IGeometrySourceParameters, IShape
	{
		static PrimitiveShape()
		{
			StretchProperty.OverrideMetadata(typeof(PrimitiveShape), new DrawingPropertyMetadata(Stretch.Fill, DrawingPropertyMetadataOptions.AffectsRender));
			StrokeThicknessProperty.OverrideMetadata(typeof(PrimitiveShape), new DrawingPropertyMetadata(1.0, DrawingPropertyMetadataOptions.AffectsRender));
		}

		public event EventHandler RenderedGeometryChanged;
		protected abstract IGeometrySource CreateGeometrySource();
		private IGeometrySource GeometrySource
		{
			get
			{
				IGeometrySource result;
				if ((result = geometrySource) == null)
					result = geometrySource = CreateGeometrySource();
				return result;
			}
		}

		public void InvalidateGeometry(InvalidateGeometryReasons reasons)
		{
			if (GeometrySource.InvalidateGeometry(reasons))
                InvalidateArrange();
		}

		protected override Size MeasureOverride(Size availableSize) => new Size(StrokeThickness, StrokeThickness);

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (GeometrySource.UpdateGeometry(this, finalSize.Bounds()))
			{
				RealizeGeometry();
			}
			base.ArrangeOverride(finalSize);
			return finalSize;
		}

		private void RealizeGeometry() => RenderedGeometryChanged?.Invoke(this, EventArgs.Empty);

		protected sealed override Geometry DefiningGeometry => GeometrySource.Geometry ?? Geometry.Empty;

		public Thickness GeometryMargin
		{
			get
			{
				if (RenderedGeometry == null)
					return default(Thickness);

				return GeometrySource.LogicalBounds.Subtract(RenderedGeometry.Bounds);
			}
		}

		private IGeometrySource geometrySource;
	}
}
