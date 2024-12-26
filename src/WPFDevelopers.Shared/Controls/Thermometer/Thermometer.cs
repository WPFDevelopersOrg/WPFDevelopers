using System.Windows;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Thermometer : ScaleBase 
    {
        public new double Maximum
        {
            get { return base.Maximum; }
           
        }
        public new double Minimum
        {
            get { return base.Minimum; }
           
        }
        static Thermometer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Thermometer),
                new FrameworkPropertyMetadata(typeof(Thermometer)));
        }
        public Thermometer()
        {
            SetValue(MaximumProperty, 40.0);
            SetValue(MinimumProperty, -10.0);
            SetValue(IntervalProperty, 10.0);
            SetValue(GeometryProperty, Geometry.Parse(@"M 2 132.8
                              a 4 4 0 0 1 4 -4
                              h 18
                              a 4 4 0 0 1 4 4
                              v 32.2
                              a 4 4 0 0 1 -4 4
                              h -18
                              a 4 4 0 0 1 -4 -4 z"));
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PaintPath();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#82848A"));
            var rect = new Rect();
            rect.Width = 30;
            rect.Height = 169;
            drawingContext.DrawRoundedRectangle(Brushes.Transparent,
                new Pen(brush, 2d),
                rect, 8d, 8d);

            #region 华氏温度

            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("华",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), textSize: 14D),
                new Point(-49, 115));


            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("氏",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), textSize: 14D),
                new Point(-49, 115 + 14));


            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("温",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), textSize: 14D),
                new Point(-49, 115 + 28));


            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("度",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), textSize: 14D),
                new Point(-49, 115 + 42));

            #endregion

            #region 摄氏温度

            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("摄",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), FlowDirection.LeftToRight,
                    14D), new Point(75, 115));


            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("氏",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), FlowDirection.LeftToRight,
                    14D), new Point(75, 115 + 14));


            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("温",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), FlowDirection.LeftToRight,
                    14D), new Point(75, 115 + 28));


            drawingContext.DrawText(
                DrawingContextHelper.GetFormattedText("度",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), FlowDirection.LeftToRight,
                    14D), new Point(75, 115 + 42));

            #endregion

            #region 画刻度

            var total_Value = Maximum - Minimum;

            var cnt = total_Value / Interval;

            var one_value = 161d / cnt;

            for (var i = 0; i <= cnt; i++)
            {
                var formattedText = DrawingContextHelper.GetFormattedText($"{Maximum - i * Interval}",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), FlowDirection.LeftToRight,
                    14D);

                drawingContext.DrawText(formattedText,
                    new Point(43, i * one_value - formattedText.Height / 2d)); //减去字体高度的一半

                formattedText = DrawingContextHelper.GetFormattedText($"{(Maximum - i * Interval) * 1.8d + 32d}",
                    (Brush)DrawingContextHelper.BrushConverter.ConvertFromString("#82848A"), textSize: 14D);

                drawingContext.DrawText(formattedText, new Point(-13, i * one_value - formattedText.Height / 2d));

                if (i != 0 && i != 5)
                {
                    drawingContext.DrawLine(new Pen(Brushes.Black, 1d),
                        new Point(4, i * one_value), new Point(6, i * one_value));

                    drawingContext.DrawLine(new Pen(Brushes.Black, 1d),
                        new Point(24, i * one_value), new Point(26, i * one_value));
                }
            }

            #endregion
        }

        /// <summary>
        ///     动态计算当前值图形坐标点
        /// </summary>
        private void PaintPath()
        {
            var one_value = 161d / ((Maximum - Minimum) / Interval);
            var width = 26d;
            var height = 169d - (Maximum - Value) * (one_value / Interval);
            var y = 169d - (169d - (Maximum - Value) * (one_value / Interval));
            Geometry = Geometry.Parse($@"M 2 {y + 4}
                                  a 4 4 0 0 1 4 -4
                                  h {width - 8}
                                  a 4 4 0 0 1 4 4
                                  v {height - 8}
                                  a 4 4 0 0 1 -4 4
                                  h -{width - 8}
                                  a 4 4 0 0 1 -4 -4 z");
        }
    }
}