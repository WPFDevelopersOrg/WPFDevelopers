using Microsoft.Expression.Drawing.Core;
using Microsoft.Expression.Drawing.Media;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Drawing.Controls
{
    public abstract class CompositeShape : Control, IGeometrySourceParameters, IShape
    {
        private Path PartPath { get; set; }

        private IGeometrySource geometrySource;


        #region Dp
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(CompositeShape), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(CompositeShape), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CompositeShape), new DrawingPropertyMetadata(1.0, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(CompositeShape), new DrawingPropertyMetadata(Stretch.Fill, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(CompositeShape), new PropertyMetadata(PenLineCap.Flat));

        public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(CompositeShape), new PropertyMetadata(PenLineCap.Flat));

        public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(CompositeShape), new PropertyMetadata(PenLineJoin.Miter));

        public static readonly DependencyProperty StrokeMiterLimitProperty = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(CompositeShape), new PropertyMetadata(10.0));

        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(CompositeShape), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(CompositeShape), new PropertyMetadata(PenLineCap.Flat));

        public static readonly DependencyProperty StrokeDashOffsetProperty = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(CompositeShape), new PropertyMetadata(0.0));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public PenLineCap StrokeStartLineCap
        {
            get => (PenLineCap)GetValue(StrokeStartLineCapProperty);
            set => SetValue(StrokeStartLineCapProperty, value);
        }

        public PenLineCap StrokeEndLineCap
        {
            get => (PenLineCap)GetValue(StrokeEndLineCapProperty);
            set => SetValue(StrokeEndLineCapProperty, value);
        }

        public PenLineJoin StrokeLineJoin
        {
            get => (PenLineJoin)GetValue(StrokeLineJoinProperty);
            set => SetValue(StrokeLineJoinProperty, value);
        }

        public double StrokeMiterLimit
        {
            get => (double)GetValue(StrokeMiterLimitProperty);
            set => SetValue(StrokeMiterLimitProperty, value);
        }

        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        public PenLineCap StrokeDashCap
        {
            get => (PenLineCap)GetValue(StrokeDashCapProperty);
            set => SetValue(StrokeDashCapProperty, value);
        }

        public double StrokeDashOffset
        {
            get => (double)GetValue(StrokeDashOffsetProperty);
            set => SetValue(StrokeDashOffsetProperty, value);
        }

        #endregion

        public event EventHandler RenderedGeometryChanged;

        protected abstract IGeometrySource CreateGeometrySource();

       
        public Geometry RenderedGeometry => GeometrySource.Geometry;

        public Thickness GeometryMargin
        {
            get
            {
                if (PartPath == null || PartPath.Data == null)
                    return default(Thickness); 
                return GeometrySource.LogicalBounds.Subtract(PartPath.Data.Bounds);
            }
        }

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


        #region override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartPath = this.FindVisualDesendent((Path child) => child.Name == "PART_Path").FirstOrDefault<Path>();
            GeometrySource.InvalidateGeometry(InvalidateGeometryReasons.TemplateChanged);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            if (GeometrySource.UpdateGeometry(this, finalSize.Bounds()))
                RealizeGeometry();

            return base.ArrangeOverride(finalSize);
        }

        #endregion

        public void InvalidateGeometry(InvalidateGeometryReasons reasons)
        {
            if (GeometrySource.InvalidateGeometry(reasons))
                InvalidateArrange();
        }

       

        private void RealizeGeometry()
        {
            if (PartPath != null)
            {
                PartPath.Data = RenderedGeometry.CloneCurrentValue();
                PartPath.Margin = GeometryMargin;
            }

            RenderedGeometryChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}
