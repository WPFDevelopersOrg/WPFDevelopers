using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Gauge : RangeBase
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("TitleProperty", typeof(string), typeof(Gauge), new PropertyMetadata("WD"));

        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(Gauge),
                new PropertyMetadata("{0:0}%", OnValueFormatChanged));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(Gauge),
                new PropertyMetadata(10.0, OnValueFormatChanged));


        static Gauge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gauge), new FrameworkPropertyMetadata(typeof(Gauge)));
        }

        public Gauge()
        {
            SetValue(ValueProperty, 0.0);
            SetValue(MinimumProperty, 0.0);
            SetValue(MaximumProperty, 100.0);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string ValueFormat
        {
            get => (string) GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        public double Thickness
        {
            get => (double) GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        private static void OnValueFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = d as Gauge;
            gauge?.InvalidateVisual();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            InvalidateVisual();
        }

        protected override void OnMinimumChanged(double oldValue, double newValue)
        {
            InvalidateVisual();
        }

        protected override void OnMaximumChanged(double oldValue, double newValue)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (Background == null)
                Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#293950"));
            var width = ActualWidth;
            var height = ActualHeight;
            var radius = Math.Min(width, height) / 2;
            drawingContext.DrawEllipse(Background, new Pen(Background, Thickness), new Point(width / 2, height / 2),
                radius, radius);
            var normalizedValue = (Value - Minimum) / (Maximum - Minimum);
            var mappedAngle = -220 + normalizedValue * 260;
            var angleInRadians = mappedAngle * Math.PI / 180;
            var pointerLength = radius * 0.7;
            var pointerX = width / 2 + pointerLength * Math.Cos(angleInRadians);
            var pointerY = height / 2 + pointerLength * Math.Sin(angleInRadians);
            drawingContext.DrawLine(new Pen(Brushes.Red, 2), new Point(width / 2, height / 2),
                new Point(pointerX, pointerY));
            drawingContext.DrawEllipse(Brushes.White, new Pen(Brushes.Red, 2), new Point(width / 2, height / 2),
                width / 20, width / 20);
            var pathGeometry = new PathGeometry();
            var startAngle = -220;
            angleInRadians = startAngle * Math.PI / 180;
            var startX = width / 2 + radius * Math.Cos(angleInRadians);
            var startY = height / 2 + radius * Math.Sin(angleInRadians);

            var pathFigure = new PathFigure
            {
                StartPoint = new Point(startX, startY)
            };

            var endAngle = 40;
            angleInRadians = endAngle * Math.PI / 180;
            var endX = width / 2 + radius * Math.Cos(angleInRadians);
            var endY = height / 2 + radius * Math.Sin(angleInRadians);

            var isLargeArc = endAngle - startAngle > 180;
            var arcSegment = new ArcSegment
            {
                Point = new Point(endX, endY),
                Size = new Size(radius, radius),
                RotationAngle = 0,
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc
            };

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);
            if (BorderBrush == null)
            {
                var gradientBrush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };
                gradientBrush.GradientStops.Add(new GradientStop((Color) ColorConverter.ConvertFromString("#37D2C2"),
                    0.0));
                gradientBrush.GradientStops.Add(new GradientStop((Color) ColorConverter.ConvertFromString("#5AD2B2"),
                    0.01));
                gradientBrush.GradientStops.Add(new GradientStop((Color) ColorConverter.ConvertFromString("#B77D29"),
                    0.49));
                gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));
                gradientBrush.Freeze();
                BorderBrush = gradientBrush;
            }

            drawingContext.DrawGeometry(null, new Pen(BorderBrush, Thickness), pathGeometry);
            var tickLength = radius * 0.1;
            var step = (Maximum - Minimum) / 10;
            for (var i = 0; i <= 10; i++)
            {
                var angle = startAngle + i * (endAngle - startAngle) / 10;
                var tickStartX = width / 2 + (radius - tickLength) * Math.Cos(angle * Math.PI / 180);
                var tickStartY = height / 2 + (radius - tickLength) * Math.Sin(angle * Math.PI / 180);
                var tickEndX = width / 2 + (radius + Thickness / 2) * Math.Cos(angle * Math.PI / 180);
                var tickEndY = height / 2 + (radius + Thickness / 2) * Math.Sin(angle * Math.PI / 180);
                drawingContext.DrawLine(new Pen(Brushes.White, 2), new Point(tickStartX, tickStartY),
                    new Point(tickEndX, tickEndY));

                var labelValue = Minimum + step * i;
                var formattedText = DrawingContextHelper.GetFormattedText(labelValue.ToString(), Brushes.White,
                    FlowDirection.LeftToRight, FontSize);

                var labelRadius = radius - tickLength * 2;
                var labelX = width / 2 + labelRadius * Math.Cos(angle * Math.PI / 180) - formattedText.Width / 2;
                var labelY = height / 2 + labelRadius * Math.Sin(angle * Math.PI / 180) - formattedText.Height / 2;
                drawingContext.DrawText(formattedText, new Point(labelX, labelY));
            }

            var formattedValue = "{0:0}%";
            try
            {
                formattedValue = string.Format(ValueFormat, Value);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Formatting failed ", ex);
            }

            var currentValueText = DrawingContextHelper.GetFormattedText(formattedValue, Brushes.White,
                FlowDirection.LeftToRight, FontSize * 2);
            var valueX = width / 2 - currentValueText.Width / 2;
            var valueY = height / 2 + radius * 0.4;
            drawingContext.DrawText(currentValueText, new Point(valueX, valueY));
            var titleValue =
                DrawingContextHelper.GetFormattedText(Title, Brushes.White, FlowDirection.LeftToRight, FontSize);
            valueX = width / 2 - titleValue.Width / 2;
            valueY = height / 2 + radius * 0.8;
            drawingContext.DrawText(titleValue, new Point(valueX, valueY));
        }
    }
}