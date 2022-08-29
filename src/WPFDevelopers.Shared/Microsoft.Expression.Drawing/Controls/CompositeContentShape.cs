using Microsoft.Expression.Drawing.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Expression.Drawing.Media;

namespace Microsoft.Expression.Drawing.Controls
{
    public abstract class CompositeContentShape : ContentControl, IGeometrySourceParameters, IShape
    {
        private Path PartPath { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartPath = this.FindVisualDesendent((Path child) => child.Name == "PART_Path").FirstOrDefault<Path>();
            GeometrySource.InvalidateGeometry(InvalidateGeometryReasons.TemplateChanged);
        }

        public Brush Fill
        {
            get
            {
                return (Brush)GetValue(FillProperty);
            }
            set
            {
                SetValue(FillProperty, value);
            }
        }

        public Brush Stroke
        {
            get
            {
                return (Brush)GetValue(StrokeProperty);
            }
            set
            {
                SetValue(StrokeProperty, value);
            }
        }

        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }
            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        public Stretch Stretch
        {
            get
            {
                return (Stretch)GetValue(StretchProperty);
            }
            set
            {
                SetValue(StretchProperty, value);
            }
        }

        public PenLineCap StrokeStartLineCap
        {
            get
            {
                return (PenLineCap)GetValue(StrokeStartLineCapProperty);
            }
            set
            {
                SetValue(StrokeStartLineCapProperty, value);
            }
        }

        public PenLineCap StrokeEndLineCap
        {
            get
            {
                return (PenLineCap)GetValue(StrokeEndLineCapProperty);
            }
            set
            {
                SetValue(StrokeEndLineCapProperty, value);
            }
        }

        public PenLineJoin StrokeLineJoin
        {
            get
            {
                return (PenLineJoin)GetValue(StrokeLineJoinProperty);
            }
            set
            {
                SetValue(StrokeLineJoinProperty, value);
            }
        }

        public double StrokeMiterLimit
        {
            get
            {
                return (double)GetValue(StrokeMiterLimitProperty);
            }
            set
            {
                SetValue(StrokeMiterLimitProperty, value);
            }
        }

        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)GetValue(StrokeDashArrayProperty);
            }
            set
            {
                SetValue(StrokeDashArrayProperty, value);
            }
        }

        public PenLineCap StrokeDashCap
        {
            get
            {
                return (PenLineCap)GetValue(StrokeDashCapProperty);
            }
            set
            {
                SetValue(StrokeDashCapProperty, value);
            }
        }

        public double StrokeDashOffset
        {
            get
            {
                return (double)GetValue(StrokeDashOffsetProperty);
            }
            set
            {
                SetValue(StrokeDashOffsetProperty, value);
            }
        }

        public Geometry RenderedGeometry
        {
            get
            {
                return GeometrySource.Geometry;
            }
        }

        public Thickness GeometryMargin
        {
            get
            {
                if (PartPath == null || PartPath.Data == null)
                {
                    return default(Thickness);
                }
                return GeometrySource.LogicalBounds.Subtract(PartPath.Data.Bounds);
            }
        }

        public object InternalContent
        {
            get
            {
                return GetValue(InternalContentProperty);
            }
            private set
            {
                SetValue(InternalContentProperty, value);
            }
        }

        public event EventHandler RenderedGeometryChanged;

        protected abstract IGeometrySource CreateGeometrySource();

        private IGeometrySource GeometrySource
        {
            get
            {
                IGeometrySource result;
                if ((result = geometrySource) == null)
                {
                    result = (geometrySource = CreateGeometrySource());
                }
                return result;
            }
        }

        public void InvalidateGeometry(InvalidateGeometryReasons reasons)
        {
            if (GeometrySource.InvalidateGeometry(reasons))
            {
                InvalidateArrange();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (GeometrySource.UpdateGeometry(this, finalSize.Bounds()))
            {
                RealizeGeometry();
            }
            return base.ArrangeOverride(finalSize);
        }

        private void RealizeGeometry()
        {
            if (PartPath != null)
            {
                PartPath.Data = RenderedGeometry.CloneCurrentValue();
                PartPath.Margin = GeometryMargin;
            }
            if (RenderedGeometryChanged != null)
            {
                RenderedGeometryChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            IFormattable formattable = Content as IFormattable;
            string text = Content as string;
            if (formattable != null || text != null)
            {
                TextBlock textBlock = InternalContent as TextBlock;
                if (textBlock == null)
                {
                    textBlock = ((TextBlock)(InternalContent = new TextBlock()));
                }
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextTrimming = TextTrimming.WordEllipsis;
                textBlock.Text = (text ?? formattable.ToString(null, null));
                return;
            }
            InternalContent = Content;
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(CompositeContentShape), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(CompositeContentShape), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CompositeContentShape), new DrawingPropertyMetadata(1.0, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(CompositeContentShape), new DrawingPropertyMetadata(Stretch.Fill, DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(CompositeContentShape), new PropertyMetadata(PenLineCap.Flat));

        public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(CompositeContentShape), new PropertyMetadata(PenLineCap.Flat));

        public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(CompositeContentShape), new PropertyMetadata(PenLineJoin.Miter));

        public static readonly DependencyProperty StrokeMiterLimitProperty = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(CompositeContentShape), new PropertyMetadata(10.0));

        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(CompositeContentShape), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(CompositeContentShape), new PropertyMetadata(PenLineCap.Flat));

        public static readonly DependencyProperty StrokeDashOffsetProperty = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(CompositeContentShape), new PropertyMetadata(0.0));

        public static readonly DependencyProperty InternalContentProperty = DependencyProperty.Register("InternalContent", typeof(object), typeof(CompositeContentShape), new PropertyMetadata(null));

        private IGeometrySource geometrySource;
    }
}
