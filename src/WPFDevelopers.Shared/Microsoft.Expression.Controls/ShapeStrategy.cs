using Microsoft.Expression.Drawing.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Controls
{
    internal class ShapeStrategy : FrameworkElementStrategy
    {
        private Stretch StretchListener
        {
            get
            {
                return (Stretch)base.GetValue(ShapeStrategy.StretchListenerProperty);
            }
            set
            {
                base.SetValue(ShapeStrategy.StretchListenerProperty, value);
            }
        }

        private double StrokeThicknessListener
        {
            get
            {
                return (double)base.GetValue(ShapeStrategy.StrokeThicknessListenerProperty);
            }
            set
            {
                base.SetValue(ShapeStrategy.StrokeThicknessListenerProperty, value);
            }
        }

        public ShapeStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            this.sourceShape = (Shape)layoutPath.SourceElement;
            base.SetListenerBinding(ShapeStrategy.StretchListenerProperty, "Stretch");
            base.SetListenerBinding(ShapeStrategy.StrokeThicknessListenerProperty, "StrokeThickness");
        }

        protected override PathGeometry UpdateGeometry()
        {
            return this.sourceShape.RenderedGeometry.AsPathGeometry();
        }

        public override IList<GeneralTransform> ComputeTransforms()
        {
            IList<GeneralTransform> list = base.ComputeTransforms() ?? new List<GeneralTransform>();
            list.Add(this.sourceShape.GeometryTransform);
            return list;
        }

        private Shape sourceShape;

        private static readonly DependencyProperty StretchListenerProperty = DependencyProperty.Register("StretchListener", typeof(Stretch), typeof(ShapeStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));

        private static readonly DependencyProperty StrokeThicknessListenerProperty = DependencyProperty.Register("StrokeThicknessListener", typeof(double), typeof(ShapeStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));
    }
}
