using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Controls
{
    internal class RectangleStrategy : ShapeStrategy
    {
        private double RadiusXListener
        {
            get
            {
                return (double)base.GetValue(RectangleStrategy.RadiusXListenerProperty);
            }
            set
            {
                base.SetValue(RectangleStrategy.RadiusXListenerProperty, value);
            }
        }

        private double RadiusYListener
        {
            get
            {
                return (double)base.GetValue(RectangleStrategy.RadiusYListenerProperty);
            }
            set
            {
                base.SetValue(RectangleStrategy.RadiusYListenerProperty, value);
            }
        }

        public RectangleStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            base.SetListenerBinding(RectangleStrategy.RadiusXListenerProperty, "RadiusX");
            base.SetListenerBinding(RectangleStrategy.RadiusYListenerProperty, "RadiusY");
        }

        protected override PathGeometry UpdateGeometry()
        {
            return base.UpdateGeometry();
        }

        private static readonly DependencyProperty RadiusXListenerProperty = DependencyProperty.Register("RadiusXListener", typeof(double), typeof(RectangleStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));

        private static readonly DependencyProperty RadiusYListenerProperty = DependencyProperty.Register("RadiusYListener", typeof(double), typeof(RectangleStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));
    }
}
