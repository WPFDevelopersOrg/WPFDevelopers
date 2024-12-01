using System.Windows;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Ruler : ScaleBase
    {
        public static readonly DependencyProperty SpanIntervalProperty =
            DependencyProperty.Register("SpanInterval", typeof(double), typeof(Ruler), new UIPropertyMetadata(5.0, OnSpanIntervalChanged));

        private static void OnSpanIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Ruler;
            ctrl?.InvalidateVisual();
        }

        public static readonly DependencyProperty MiddleMaskProperty =
            DependencyProperty.Register("MiddleMask", typeof(int), typeof(Ruler), new UIPropertyMetadata(2, OnMiddleMaskChanged));

        private static void OnMiddleMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Ruler;
            ctrl?.InvalidateVisual();
        }

        static Ruler()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(typeof(Ruler)));
        }

        public Ruler()
        {
            SetValue(MaximumProperty, 240.0);
            SetValue(MinimumProperty, 120.0);
            SetValue(IntervalProperty, 30.0);
            SetValue(GeometryProperty, Geometry.Parse(@"M 257,0 257,25 264,49 250,49 257,25"));
            Loaded += Ruler_Loaded;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            PaintPath();
        }
        protected override void OnMaximumChanged(double oldValue, double newValue)
        {
            InvalidateVisual();
        }

        protected override void OnMinimumChanged(double oldValue, double newValue)
        {
            InvalidateVisual();
        }

        protected override void OnIntervalChanged(double oldValue, double newValue)
        {
            InvalidateVisual();
        }

        public double SpanInterval
        {
            get => (double)GetValue(SpanIntervalProperty);

            set => SetValue(SpanIntervalProperty, value);
        }

        public int MiddleMask
        {
            get => (int)GetValue(MiddleMaskProperty);

            set => SetValue(MiddleMaskProperty, value);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            var nextLineValue = 0d;
            var one_Width = ActualWidth / ((Maximum - Minimum) / Interval);

            for (var i = 0; i <= (Maximum - Minimum) / Interval; i++)
            {
                var numberText = DrawingContextHelper.GetFormattedText((Minimum + i * Interval).ToString(),
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#FFFFFF"), FlowDirection.LeftToRight,
                    10);
                drawingContext.DrawText(numberText, new Point(i * one_Width - 8, 0));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.White), 1), new Point(i * one_Width, 25),
                    new Point(i * one_Width, ActualHeight - 2));
                var cnt = Interval / SpanInterval;
                for (var j = 1; j <= cnt; j++)
                    if (j % MiddleMask == 0)
                        drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.White), 1),
                            new Point(j * (one_Width / cnt) + nextLineValue, ActualHeight - 2),
                            new Point(j * (one_Width / cnt) + nextLineValue, ActualHeight - 10));
                    else
                        drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.White), 1),
                            new Point(j * (one_Width / cnt) + nextLineValue, ActualHeight - 2),
                            new Point(j * (one_Width / cnt) + nextLineValue, ActualHeight - 5));

                nextLineValue = i * one_Width;
            }
        }

        void Ruler_Loaded(object sender, RoutedEventArgs e)
        {
            PaintPath();
        }

        void PaintPath()
        {
            if (Parent == null)
                return;
            var d_Value = Value - Minimum;
            var one_Value = ActualWidth / (Maximum - Minimum);
            var x_Point = one_Value * d_Value + ((double)Parent.GetValue(ActualWidthProperty) - ActualWidth) / 2d;
            Geometry =
                Geometry.Parse($"M {x_Point},0 {x_Point},25 {x_Point + 7},49 {x_Point - 7},49 {x_Point},25");
        }
    }
}