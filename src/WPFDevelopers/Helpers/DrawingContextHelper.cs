using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WPFDevelopers
{
    public static class DrawingContextHelper
    {
        /// <summary>
        /// 字体资源
        /// </summary>
        private static FontFamily fontFamily = Application.Current.Resources["NormalFontFamily"] as FontFamily;
        /// <summary>
        /// 颜色转换
        /// </summary>
        private static BrushConverter brushConverter = new BrushConverter();

        /// <summary>
        /// 绘制Line
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="pen"></param>
        /// <param name="lineThickness"></param>
        /// <param name="points"></param>
        public static void DrawSnappedLinesBetweenPoints(this DrawingContext dc,
                Pen pen, double lineThickness, params Point[] points)
        {
            var guidelineSet = new GuidelineSet();
            foreach (var point in points)
            {
                guidelineSet.GuidelinesX.Add(point.X);
                guidelineSet.GuidelinesY.Add(point.Y);
            }
            var half = lineThickness / 2;
            points = points.Select(p => new Point(p.X + half, p.Y + half)).ToArray();
            dc.PushGuidelineSet(guidelineSet);
            for (var i = 0; i < points.Length - 1; i = i + 2)
            {
                dc.DrawLine(pen, points[i], points[i + 1]);
            }
            dc.Pop();
        }
        /// <summary>
        /// 返回FormattedText
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="flowDirection"></param>
        /// <returns></returns>
        public static FormattedText GetFormattedText(string text,string color = null, FlowDirection flowDirection = FlowDirection.RightToLeft,double textSize = 12.0D)
        {
            return new FormattedText(
                  text,
                  CultureInfo.CurrentCulture,
                  flowDirection,
                  new Typeface(fontFamily, FontStyles.Normal, FontWeights.Thin, FontStretches.Normal),
                  textSize, color == null ? Brushes.Black : (Brush)brushConverter.ConvertFromString(color))
            {
                MaxLineCount = 1,
                TextAlignment = TextAlignment.Justify,
                Trimming = TextTrimming.CharacterEllipsis
            };
        }
    }
}
