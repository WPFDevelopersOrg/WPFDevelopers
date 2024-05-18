using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class ChartLine : ChartBase
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Datas == null || Datas.Count() == 0)
                return;
            base.OnRender(drawingContext);
            var dicts = new Dictionary<Rect, string>();
            var x = StartX;
            var interval = Interval;
            var drawingPen = new Pen
            {
                Thickness = 1,
                Brush = NormalBrush
            };
            drawingPen.Freeze();
            var firstDataPoint = Datas.FirstOrDefault();
            if (firstDataPoint.Equals(default(KeyValuePair<string, double>)))
                return;
            double proportion = firstDataPoint.Value / IntervalY;
            double yPositionFromBottom = StartY - proportion * (ScaleFactor * Rows);
            var startPoint = new Point(x + Interval / 2, yPositionFromBottom);
            foreach (var item in Datas)
            {
                var formattedText = DrawingContextHelper.GetFormattedText(item.Key,
                   ChartFill, FlowDirection.LeftToRight);
                var point = new Point(x + interval / 2 - formattedText.Width / 2, StartY + 4);
                drawingContext.DrawText(formattedText, point);

                var y = StartY - (item.Value / IntervalY) * (ScaleFactor * Rows);
                var endPoint = new Point(x + Interval / 2, y);
                drawingContext.DrawLine(drawingPen, startPoint, endPoint);
                var ellipsePoint = new Point(endPoint.X - EllipseSize / 2, endPoint.Y - EllipseSize / 2);
                var rect = new Rect(ellipsePoint, new Size(EllipseSize, EllipseSize));
                var ellipseGeom = new EllipseGeometry(rect);
                drawingContext.DrawGeometry(drawingPen.Brush, drawingPen, ellipseGeom);
                startPoint = endPoint;
                x += interval;
                var nRect = new Rect(rect.Left - EllipsePadding, rect.Top - EllipsePadding, rect.Width + EllipsePadding, rect.Height + EllipsePadding);
                dicts.Add(nRect, $"{item.Key} : {item.Value}");
            }
            PointCache = dicts;
        }
    }
}