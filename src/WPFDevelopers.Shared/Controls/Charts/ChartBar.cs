using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class ChartBar : ChartBase
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var dicts = new Dictionary<Rect, string>();
            var x = StartX;
            var rectWidth = 85;
            var interval = Interval;
            foreach (var item in Datas)
            {
                var formattedText = DrawingContextHelper.GetFormattedText(item.Key,
                    ChartFill, FlowDirection.LeftToRight);
                drawingContext.DrawText(formattedText, new Point(x + interval / 2 - formattedText.Width / 2, StartY + 4));
                var _value = item.Value;
                var rectHeight = (_value - 0) / (IntervalY - 0) * (ScaleFactor * Rows);
                var rect = new Rect(x + (interval - rectWidth) / 2, StartY - rectHeight, rectWidth, rectHeight);
                drawingContext.DrawRectangle(NormalBrush, null, rect);
                x += interval;
                var nRect = new Rect(rect.Left - EllipsePadding, rect.Top - EllipsePadding, rect.Width + EllipsePadding, rect.Height + EllipsePadding);
                dicts.Add(nRect, $"{item.Key} : {item.Value}");
            }
            PointCache = dicts;
        }
    }
}