using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class BasicBarChart : Control
    {
        public static readonly DependencyProperty SeriesArrayProperty =
            DependencyProperty.Register("SeriesArray", typeof(IEnumerable<KeyValuePair<string, double>>),
                typeof(BasicBarChart), new UIPropertyMetadata(SeriesArrayChanged));


        static BasicBarChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicBarChart),
                new FrameworkPropertyMetadata(typeof(BasicBarChart)));
        }

        public IEnumerable<KeyValuePair<string, double>> SeriesArray
        {
            get => (IEnumerable<KeyValuePair<string, double>>)GetValue(SeriesArrayProperty);
            set => SetValue(SeriesArrayProperty, value);
        }

        private static void SeriesArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var radarChart = d as BasicBarChart;
            if (e.NewValue != null)
                radarChart.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (SeriesArray == null || SeriesArray.Count() == 0)
                return;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;
            var brushConverter = new BrushConverter();
            var myPen = new Pen
            {
                Thickness = 1,
                Brush = (Brush)brushConverter.ConvertFromString("#6E7079")
            };
            myPen.Freeze();

            var h = ActualHeight / 2 + 160;
            var w = ActualWidth / 2;
            var startX = w / 3;
            var width = SeriesArray.Count() * 120 + startX;
            var stratNum = 0;

            drawingContext.DrawSnappedLinesBetweenPoints(myPen, myPen.Thickness, new Point(startX, h),
                new Point(width, h));
            var formattedText = DrawingContextHelper.GetFormattedText(stratNum.ToString(),
                (Brush)brushConverter.ConvertFromString("#6E7079"), FlowDirection.LeftToRight);
            drawingContext.DrawText(formattedText,
                new Point(startX - formattedText.Width * 2 - 10, h - formattedText.Height / 2));
            var x = startX;
            var y = h + myPen.Thickness;
            var points = new List<Point>();
            var rectBrush = ControlsHelper.PrimaryNormalBrush;
            for (var i = 0; i < SeriesArray.Count() + 1; i++)
            {
                points.Add(new Point(x, y));
                points.Add(new Point(x, y + 4));
                x = x + 120;
            }

            drawingContext.DrawSnappedLinesBetweenPoints(myPen, myPen.Thickness, points.ToArray());

            var xAxisPen = new Pen
            {
                Thickness = 1,
                Brush = (Brush)brushConverter.ConvertFromString("#E0E6F1")
            };
            xAxisPen.Freeze();
            var xAxis = h - 80;
            var max = Convert.ToInt32(SeriesArray.Max(kvp => kvp.Value));
            max = (max / 50 + (max % 50 == 0 ? 0 : 1)) * 50 / 50;
            var min = Convert.ToInt32(SeriesArray.Min(kvp => kvp.Value));
            points.Clear();
            for (var i = 0; i < max; i++)
            {
                points.Add(new Point(startX, xAxis));
                points.Add(new Point(width, xAxis));
                stratNum += 50;
                formattedText = DrawingContextHelper.GetFormattedText(stratNum.ToString(),
                    (Brush)brushConverter.ConvertFromString("#6E7079"), FlowDirection.LeftToRight);
                drawingContext.DrawText(formattedText,
                    new Point(startX - formattedText.Width - 10, xAxis - formattedText.Height / 2));
                xAxis = xAxis - 80;
            }

            drawingContext.DrawSnappedLinesBetweenPoints(xAxisPen, xAxisPen.Thickness, points.ToArray());

            x = startX;
            var rectWidth = 85;
            var rectHeight = 0D;
            for (var i = 0; i < SeriesArray.Count(); i++)
            {
                formattedText = DrawingContextHelper.GetFormattedText(SeriesArray.ToList()[i].Key,
                    (Brush)brushConverter.ConvertFromString("#6E7079"), FlowDirection.LeftToRight);
                drawingContext.DrawText(formattedText, new Point(x + 120 / 2 - formattedText.Width / 2, y + 4));
                var _value = SeriesArray.ToList()[i].Value;
                rectHeight = (_value - 0) / (stratNum - 0) * (80 * max);
                drawingContext.DrawRectangle(rectBrush, null,
                    new Rect(x + (120 - 85) / 2, h - rectHeight, rectWidth, rectHeight));
                x = x + 120;
            }
        }
    }
}