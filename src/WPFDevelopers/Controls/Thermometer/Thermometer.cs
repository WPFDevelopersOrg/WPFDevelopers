using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Thermometer : Control
    {
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(Thermometer), new UIPropertyMetadata(40.0));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }

            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(Thermometer), new UIPropertyMetadata(-10.0));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }

            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(Thermometer), new UIPropertyMetadata(OnCurrentValueChanged));

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thermometer thermometer = d as Thermometer;
            thermometer.CurrentValue = Convert.ToDouble(e.NewValue);
        }

        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }

            set
            {
                SetValue(CurrentValueProperty, value);

                PaintPath();
            }
        }

        /// <summary>
        /// 步长
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(Thermometer), new UIPropertyMetadata(10.0));

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }

            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// 当前值的图形坐标点
        /// </summary>
        public static readonly DependencyProperty CurrentGeometryProperty =
            DependencyProperty.Register("CurrentGeometry", typeof(Geometry), typeof(Thermometer), new PropertyMetadata(Geometry.Parse(@"M 2 132.8
                              a 4 4 0 0 1 4 -4
                              h 18
                              a 4 4 0 0 1 4 4
                              v 32.2
                              a 4 4 0 0 1 -4 4
                              h -18
                              a 4 4 0 0 1 -4 -4 z")));

        public Geometry CurrentGeometry
        {
            get { return (Geometry)GetValue(CurrentGeometryProperty); }

            set { SetValue(CurrentGeometryProperty, value); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        static Thermometer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Thermometer), new FrameworkPropertyMetadata(typeof(Thermometer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PaintPath();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#82848A"));
            Rect rect = new Rect();
            rect.Width = 30;
            rect.Height = 169;
            drawingContext.DrawRoundedRectangle(Brushes.Transparent,
                new Pen(brush, 2d),
                rect, 8d, 8d);

            #region 华氏温度

            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("华", color: "#82848A", textSize: 14D), new Point(-49, 115));


            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("氏", color: "#82848A", textSize: 14D), new Point(-49, 115 + 14));


            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("温", color: "#82848A", textSize: 14D), new Point(-49, 115 + 28));


            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("度", color: "#82848A", textSize: 14D), new Point(-49, 115 + 42));

            #endregion

            #region 摄氏温度


            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("摄", "#82848A", FlowDirection.LeftToRight, textSize: 14D), new Point(75, 115));


            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("氏", "#82848A", FlowDirection.LeftToRight, textSize: 14D), new Point(75, 115 + 14));


            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("温", "#82848A", FlowDirection.LeftToRight, textSize: 14D), new Point(75, 115 + 28));


            drawingContext.DrawText(DrawingContextHelper.GetFormattedText("度", "#82848A", FlowDirection.LeftToRight, textSize: 14D), new Point(75, 115 + 42));

            #endregion

            #region 画刻度

            var total_Value = MaxValue - MinValue;

            var cnt = total_Value / Interval;

            var one_value = 161d / cnt;

            for (int i = 0; i <= cnt; i++)
            {
                var formattedText = DrawingContextHelper.GetFormattedText($"{MaxValue - (i * Interval)}", "#82848A", FlowDirection.LeftToRight,14D);

                drawingContext.DrawText(formattedText, new Point(43, i * one_value - (formattedText.Height / 2d)));//减去字体高度的一半

                formattedText = DrawingContextHelper.GetFormattedText($"{(MaxValue - (i * Interval)) * 1.8d + 32d}", color: "#82848A", textSize: 14D);

                drawingContext.DrawText(formattedText, new Point(-13, i * one_value - (formattedText.Height / 2d)));

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
        /// 动态计算当前值图形坐标点
        /// </summary>
        private void PaintPath()
        {
            var one_value = 161d / ((MaxValue - MinValue) / Interval);

            var width = 26d;

            var height = 169d - (MaxValue - CurrentValue) * (one_value / Interval);

            var x = 2d;

            var y = 169d - (169d - (MaxValue - CurrentValue) * (one_value / Interval));


            CurrentGeometry = Geometry.Parse($@"M 2 {y + 4}
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
