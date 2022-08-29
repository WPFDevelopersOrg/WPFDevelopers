using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Ruler : Control
    {
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(Ruler), new UIPropertyMetadata(30.0));


        public static readonly DependencyProperty SpanIntervalProperty =
            DependencyProperty.Register("SpanInterval", typeof(double), typeof(Ruler), new UIPropertyMetadata(5.0));


        public static readonly DependencyProperty MiddleMaskProperty =
            DependencyProperty.Register("MiddleMask", typeof(int), typeof(Ruler), new UIPropertyMetadata(2));

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(Ruler),
                new UIPropertyMetadata(OnCurrentValueChanged));

        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(Ruler), new UIPropertyMetadata(120.0));

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(Ruler), new UIPropertyMetadata(240.0));

        public static readonly DependencyProperty CurrentGeometryProperty =
            DependencyProperty.Register("CurrentGeometry", typeof(Geometry), typeof(Ruler),
                new PropertyMetadata(Geometry.Parse("M 257,0 257,25 264,49 250,49 257,25")));

        static Ruler()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(typeof(Ruler)));
        }

        public Ruler()
        {
            Loaded += Ruler_Loaded;
        }

        public double Interval
        {
            get => (double)GetValue(IntervalProperty);

            set => SetValue(IntervalProperty, value);
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

        public double CurrentValue
        {
            get => (double)GetValue(CurrentValueProperty);

            set
            {
                SetValue(CurrentValueProperty, value);
                PaintPath();
            }
        }

        public double StartValue
        {
            get => (double)GetValue(StartValueProperty);

            set => SetValue(StartValueProperty, value);
        }

        public double EndValue
        {
            get => (double)GetValue(EndValueProperty);

            set => SetValue(EndValueProperty, value);
        }

        public Geometry CurrentGeometry
        {
            get => (Geometry)GetValue(CurrentGeometryProperty);

            set => SetValue(CurrentGeometryProperty, value);
        }

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ruler = d as Ruler;
            ruler.CurrentValue = Convert.ToDouble(e.NewValue);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            var nextLineValue = 0d;
            var one_Width = ActualWidth / ((EndValue - StartValue) / Interval);

            for (var i = 0; i <= (EndValue - StartValue) / Interval; i++)
            {
                //var numberText = new FormattedText((StartValue + (i * Interval)).ToString(),
                //    CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Microsoft YaHei"), 10, Brushes.White);
                var numberText = DrawingContextHelper.GetFormattedText((StartValue + i * Interval).ToString(),
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

        private void Ruler_Loaded(object sender, RoutedEventArgs e)
        {
            PaintPath();
        }

        private void PaintPath()
        {
            var d_Value = CurrentValue - StartValue;
            var one_Value = ActualWidth / (EndValue - StartValue);
            var x_Point = one_Value * d_Value + ((double)Parent.GetValue(ActualWidthProperty) - ActualWidth) / 2d;
            CurrentGeometry =
                Geometry.Parse($"M {x_Point},0 {x_Point},25 {x_Point + 7},49 {x_Point - 7},49 {x_Point},25");
        }
    }
}