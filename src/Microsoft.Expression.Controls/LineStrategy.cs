using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Controls
{
    internal class LineStrategy : ShapeStrategy
    {
        private double X1Listener
        {
            get
            {
                return (double)base.GetValue(LineStrategy.X1ListenerProperty);
            }
            set
            {
                base.SetValue(LineStrategy.X1ListenerProperty, value);
            }
        }

        private double X2Listener
        {
            get
            {
                return (double)base.GetValue(LineStrategy.X2ListenerProperty);
            }
            set
            {
                base.SetValue(LineStrategy.X2ListenerProperty, value);
            }
        }

        private double Y1Listener
        {
            get
            {
                return (double)base.GetValue(LineStrategy.Y1ListenerProperty);
            }
            set
            {
                base.SetValue(LineStrategy.Y1ListenerProperty, value);
            }
        }

        private double Y2Listener
        {
            get
            {
                return (double)base.GetValue(LineStrategy.Y2ListenerProperty);
            }
            set
            {
                base.SetValue(LineStrategy.Y2ListenerProperty, value);
            }
        }

        public LineStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            base.SetListenerBinding(LineStrategy.X1ListenerProperty, "X1");
            base.SetListenerBinding(LineStrategy.X2ListenerProperty, "X2");
            base.SetListenerBinding(LineStrategy.Y1ListenerProperty, "Y1");
            base.SetListenerBinding(LineStrategy.Y2ListenerProperty, "Y2");
        }

        protected override PathGeometry UpdateGeometry()
        {
            return base.UpdateGeometry();
        }

        private static readonly DependencyProperty X1ListenerProperty = DependencyProperty.Register("X1Listener", typeof(double), typeof(LineStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));

        private static readonly DependencyProperty X2ListenerProperty = DependencyProperty.Register("X2Listener", typeof(double), typeof(LineStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));

        private static readonly DependencyProperty Y1ListenerProperty = DependencyProperty.Register("Y1Listener", typeof(double), typeof(LineStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));

        private static readonly DependencyProperty Y2ListenerProperty = DependencyProperty.Register("Y2Listener", typeof(double), typeof(LineStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));
    }
}
