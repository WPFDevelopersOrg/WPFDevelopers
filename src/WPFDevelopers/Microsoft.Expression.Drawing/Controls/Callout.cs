using Microsoft.Expression.Media;
using System.Windows;

namespace Microsoft.Expression.Drawing.Controls
{
    public sealed class Callout : CompositeContentShape, ICalloutGeometrySourceParameters, IGeometrySourceParameters
    {
        public Point AnchorPoint
        {
            get => (Point)GetValue(AnchorPointProperty);
            set => SetValue(AnchorPointProperty, value);
        }

        public CalloutStyle CalloutStyle
        {
            get => (CalloutStyle)GetValue(CalloutStyleProperty);
            set => SetValue(CalloutStyleProperty, value);
        }

        public Callout()
        {
            DefaultStyleKey = typeof(Callout);
        }

        protected override IGeometrySource CreateGeometrySource() => new CalloutGeometrySource();

        public static readonly DependencyProperty AnchorPointProperty = DependencyProperty.Register("AnchorPoint", typeof(Point), typeof(Callout), new DrawingPropertyMetadata(default(Point), DrawingPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CalloutStyleProperty = DependencyProperty.Register("CalloutStyle", typeof(CalloutStyle), typeof(Callout), new DrawingPropertyMetadata(CalloutStyle.RoundedRectangle, DrawingPropertyMetadataOptions.AffectsRender));
    }
}
