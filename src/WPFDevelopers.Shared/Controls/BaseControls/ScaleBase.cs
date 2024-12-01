using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class ScaleBase : RangeBase//Control
    {
        //public static readonly DependencyProperty MaximumProperty =
        //    DependencyProperty.Register("Maximum", typeof(double), typeof(ScaleBase), new UIPropertyMetadata(0.0, OnMaximumChanged));

        //private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var ctrl = (ScaleBase)d;
        //    ctrl.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        //}
        //protected virtual void OnMaximumChanged(double oldValue, double newValue)
        //{
        //}

        //public static readonly DependencyProperty MinimumProperty =
        //    DependencyProperty.Register("Minimum", typeof(double), typeof(ScaleBase), new UIPropertyMetadata(0.0, OnMinimumChanged));

        //private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var ctrl = (ScaleBase)d;
        //    ctrl.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        //}
        //protected virtual void OnMinimumChanged(double oldValue, double newValue)
        //{
        //}

        //public static readonly DependencyProperty ValueProperty =
        //    DependencyProperty.Register("Value", typeof(double), typeof(ScaleBase), new UIPropertyMetadata(0.0, OnValueChanged));

        //private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var ctrl = (ScaleBase)d;
        //    ctrl.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        //}
        //protected virtual void OnValueChanged(double oldValue, double newValue)
        //{
        //}

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(ScaleBase), new UIPropertyMetadata(0.0, OnIntervalChanged));
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ScaleBase)d;
            ctrl.OnIntervalChanged((double)e.OldValue, (double)e.NewValue);
        }
        protected virtual void OnIntervalChanged(double oldValue, double newValue)
        {
        }

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ScaleBase), new PropertyMetadata(null, OnGeometryChanged));
        private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ScaleBase)d;
            ctrl.OnGeometryChanged((Geometry)e.OldValue, (Geometry)e.NewValue);
        }
        protected virtual void OnGeometryChanged(Geometry oldValue, Geometry newValue)
        {
        }

        //public double Maximum
        //{
        //    get => (double)GetValue(MaximumProperty);
        //    set => SetValue(MaximumProperty, value);
        //}

        //public double Minimum
        //{
        //    get => (double)GetValue(MinimumProperty);
        //    set => SetValue(MinimumProperty, value);
        //}
        //public double Value
        //{
        //    get => (double)GetValue(ValueProperty);
        //    set => SetValue(ValueProperty, value);
        //}
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);

            set => SetValue(IntervalProperty, value);
        }
        public Geometry Geometry
        {
            get => (Geometry)GetValue(GeometryProperty);
            set => SetValue(GeometryProperty, value);
        }
    }
}
