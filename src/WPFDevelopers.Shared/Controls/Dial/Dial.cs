using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPFDevelopers.Controls
{
    public class Dial : RangeBase
    {
        static Dial()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dial),
                new FrameworkPropertyMetadata(typeof(Dial)));
        }

        public static new readonly DependencyProperty BorderThicknessProperty = 
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(Dial), new PropertyMetadata(1.0, OnBorderThicknessChanged));


        public new double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        private Point _centerPoint;
        private double _radius;
        private bool _isDragging;
        private Point _previousMousePosition;
        private double _dialRadius;

        public Dial()
        {
            SetValue(ValueProperty, 0.0);
            SetValue(MinimumProperty, 0.0);
            SetValue(MaximumProperty, 100.0);
            if(Background == null)
            {
                var backgroundBrush = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromRgb(220, 220, 220), 0.0),
                    new GradientStop(Color.FromRgb(200, 200, 200), 0.5),
                    new GradientStop(Color.FromRgb(180, 180, 180), 1.0)
                },
                    GradientOrigin = new Point(0.3, 0.3),
                    Center = new Point(0.5, 0.5),
                    RadiusX = 0.8,
                    RadiusY = 0.8
                };
                backgroundBrush.Freeze();
                Background = backgroundBrush;
            }
            if(BorderBrush == null)
            {
                var borderBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                borderBrush.Freeze();
                BorderBrush = borderBrush;
            }
            if (Foreground == null)
                Foreground = Brushes.DarkGray;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            _centerPoint = new Point(ActualWidth / 2, ActualHeight / 2);
            _radius = Math.Min(ActualWidth, ActualHeight) / 2 - 25;
            _dialRadius = Math.Min(ActualWidth, ActualHeight) / 2 - 5;

            DrawBackground(drawingContext);
            DrawNotches(drawingContext);
            DrawPointer(drawingContext);
        }

        private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Dial;
            ctrl?.InvalidateVisual();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            InvalidateVisual();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            InvalidateVisual();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            CaptureMouse();
            _isDragging = true;
            _previousMousePosition = e.GetPosition(this);
            UpdateValueFromMouse(_previousMousePosition);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                var currentPosition = e.GetPosition(this);
                UpdateValueFromMouse(currentPosition);
                _previousMousePosition = currentPosition;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _isDragging = false;
            ReleaseMouseCapture();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var step = (Maximum - Minimum) / 100.0;
            var delta = e.Delta > 0 ? step : -step;
            Value = Math.Max(Minimum, Math.Min(Maximum, Value + delta));
        }

       
        private void DrawBackground(DrawingContext dc)
        {
            var borderPen = new Pen(BorderBrush,BorderThickness);
            borderPen.Freeze();
            dc.DrawEllipse(Background, borderPen, _centerPoint, _radius, _radius);
        }


        private void DrawNotches(DrawingContext dc)
        {
            //var notchCount = (int)((Maximum - Minimum) / 1);
            //if (notchCount > 100) notchCount = 100;

            //var angleStep = 300.0 / notchCount;
            //var startAngle = -240.0; 

            //for (int i = 0; i <= notchCount; i++)
            //{
            //    var angle = startAngle + i * angleStep;
            //    var radian = angle * Math.PI / 180.0;

            //    var outerRadius = _dialRadius;

            //    double innerRadius = _dialRadius - (i % 5 == 0 ? 15 : 10); 

            //    var outerPoint = new Point(
            //        _centerPoint.X + outerRadius * Math.Cos(radian),
            //        _centerPoint.Y + outerRadius * Math.Sin(radian));

            //    var innerPoint = new Point(
            //        _centerPoint.X + innerRadius * Math.Cos(radian),
            //        _centerPoint.Y + innerRadius * Math.Sin(radian));
            //    var notchBrush = Foreground;
            //    var thickness = 1;
            //    dc.DrawLine(new Pen(notchBrush, thickness),
            //        outerPoint, innerPoint);
            //}
            // 修正1：计算总刻度数时使用完整范围
            double totalRange = Maximum - Minimum;

            // 假设我们希望每1个单位一个刻度，但限制最大刻度数
            int notchCount = (int)totalRange;
            if (notchCount > 100) notchCount = 100;
            if (notchCount < 10) notchCount = 10; // 保证最小刻度数

            var angleStep = 300.0 / notchCount;
            var startAngle = -240.0;

            for (int i = 0; i <= notchCount; i++)
            {
                var angle = startAngle + i * angleStep;
                var radian = angle * Math.PI / 180.0;
                var outerRadius = _dialRadius;
                double notchLength;
                if (i % 10 == 0)
                    notchLength = 15;
                else if (i % 5 == 0)
                    notchLength = 12; 
                else
                    notchLength = 10; 

                double innerRadius = _dialRadius - notchLength;

                var outerPoint = new Point(
                    _centerPoint.X + outerRadius * Math.Cos(radian),
                    _centerPoint.Y + outerRadius * Math.Sin(radian));

                var innerPoint = new Point(
                    _centerPoint.X + innerRadius * Math.Cos(radian),
                    _centerPoint.Y + innerRadius * Math.Sin(radian));
                var pen = new Pen(Foreground, 1);
                pen.Freeze();
                dc.DrawLine(pen, outerPoint, innerPoint);
            }
        }

        private void DrawPointer(DrawingContext dc)
        {
            var normalizedValue = (Value - Minimum) / (Maximum - Minimum);
            var angle = -240.0 + normalizedValue * 300.0;
            var radian = angle * Math.PI / 180.0;

            var pointerEnd = new Point(
                _centerPoint.X + (_radius - 20) * Math.Cos(radian),
                _centerPoint.Y + (_radius - 20) * Math.Sin(radian));

            var lineEnd = new Point(pointerEnd.X + 20 * Math.Cos(radian),pointerEnd.Y + 20 * Math.Sin(radian));
            dc.DrawLine(new Pen(BorderBrush, 3), pointerEnd, lineEnd);
            dc.DrawEllipse(Background, new Pen(BorderBrush, 1), pointerEnd, 8, 8);
        }

        private void UpdateValueFromMouse(Point mousePosition)
        {
            var vector = mousePosition - _centerPoint;
            var angle = Math.Atan2(vector.Y, vector.X) * 180.0 / Math.PI;
            if (angle < 0) angle += 360;
            var dialAngle = angle + 240;
            if (dialAngle > 360) dialAngle -= 360;
            dialAngle = Math.Max(0, Math.Min(300, dialAngle));
            var normalized = dialAngle / 300.0;
            Value = Minimum + normalized * (Maximum - Minimum);
        }

    }
}
