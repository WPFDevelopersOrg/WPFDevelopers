using System.Windows.Media;
using System.Windows;
using WPFDevelopers.Utilities;
using System;

namespace WPFDevelopers.Helpers
{
    public static class GeometryHelper
    {
        public static void GenerateGeometry(StreamGeometryContext ctx, Rect rect, Radii radii)
        {
            var point = new Point(radii.LeftTop, 0.0);
            var point2 = new Point(rect.Width - radii.RightTop, 0.0);
            var point3 = new Point(rect.Width, radii.TopRight);
            var point4 = new Point(rect.Width, rect.Height - radii.BottomRight);
            var point5 = new Point(rect.Width - radii.RightBottom, rect.Height);
            var point6 = new Point(radii.LeftBottom, rect.Height);
            var point7 = new Point(0.0, rect.Height - radii.BottomLeft);
            var point8 = new Point(0.0, radii.TopLeft);
            if (point.X > point2.X)
            {
                var x = radii.LeftTop / (radii.LeftTop + radii.RightTop) * rect.Width;
                point.X = x;
                point2.X = x;
            }
            if (point3.Y > point4.Y)
            {
                var y = radii.TopRight / (radii.TopRight + radii.BottomRight) * rect.Height;
                point3.Y = y;
                point4.Y = y;
            }
            if (point5.X < point6.X)
            {
                var x2 = radii.LeftBottom / (radii.LeftBottom + radii.RightBottom) * rect.Width;
                point5.X = x2;
                point6.X = x2;
            }
            if (point7.Y < point8.Y)
            {
                var y2 = radii.TopLeft / (radii.TopLeft + radii.BottomLeft) * rect.Height;
                point7.Y = y2;
                point8.Y = y2;
            }
            var vector = new Vector(rect.TopLeft.X, rect.TopLeft.Y);
            point += vector;
            point2 += vector;
            point3 += vector;
            point4 += vector;
            point5 += vector;
            point6 += vector;
            point7 += vector;
            point8 += vector;
            ctx.BeginFigure(point, true, true);
            ctx.LineTo(point2, true, false);
            var width = rect.TopRight.X - point2.X;
            var height = point3.Y - rect.TopRight.Y;
            if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height))
            {
                ctx.ArcTo(point3, new Size(width, height), 0.0, false, SweepDirection.Clockwise, true, false);
            }
            ctx.LineTo(point4, true, false);
            width = rect.BottomRight.X - point5.X;
            height = rect.BottomRight.Y - point4.Y;
            if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height))
            {
                ctx.ArcTo(point5, new Size(width, height), 0.0, false, SweepDirection.Clockwise, true, false);
            }
            ctx.LineTo(point6, true, false);
            width = point6.X - rect.BottomLeft.X;
            height = rect.BottomLeft.Y - point7.Y;
            if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height))
            {
                ctx.ArcTo(point7, new Size(width, height), 0.0, false, SweepDirection.Clockwise, true, false);
            }
            ctx.LineTo(point8, true, false);
            width = point.X - rect.TopLeft.X;
            height = point8.Y - rect.TopLeft.Y;
            if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height))
            {
                ctx.ArcTo(point, new Size(width, height), 0.0, false, SweepDirection.Clockwise, true, false);
            }
        }
        public struct Radii
        {
            internal Radii(CornerRadius radii, Thickness borders, bool outer)
            {
                var left = 0.5 * borders.Left;
                var top = 0.5 * borders.Top;
                var right = 0.5 * borders.Right;
                var bottom = 0.5 * borders.Bottom;
                if (!outer)
                {
                    LeftTop = Math.Max(0.0, radii.TopLeft - left);
                    TopLeft = Math.Max(0.0, radii.TopLeft - top);
                    TopRight = Math.Max(0.0, radii.TopRight - top);
                    RightTop = Math.Max(0.0, radii.TopRight - right);
                    RightBottom = Math.Max(0.0, radii.BottomRight - right);
                    BottomRight = Math.Max(0.0, radii.BottomRight - bottom);
                    BottomLeft = Math.Max(0.0, radii.BottomLeft - bottom);
                    LeftBottom = Math.Max(0.0, radii.BottomLeft - left);
                    return;
                }
                if (DoubleUtil.IsZero(radii.TopLeft))
                {
                    LeftTop = (TopLeft = 0.0);
                }
                else
                {
                    LeftTop = radii.TopLeft + left;
                    TopLeft = radii.TopLeft + top;
                }
                if (DoubleUtil.IsZero(radii.TopRight))
                {
                    TopRight = (RightTop = 0.0);
                }
                else
                {
                    TopRight = radii.TopRight + top;
                    RightTop = radii.TopRight + right;
                }
                if (DoubleUtil.IsZero(radii.BottomRight))
                {
                    RightBottom = (BottomRight = 0.0);
                }
                else
                {
                    RightBottom = radii.BottomRight + right;
                    BottomRight = radii.BottomRight + bottom;
                }
                if (DoubleUtil.IsZero(radii.BottomLeft))
                {
                    BottomLeft = (LeftBottom = 0.0);
                    return;
                }
                BottomLeft = radii.BottomLeft + bottom;
                LeftBottom = radii.BottomLeft + left;
            }

            internal double LeftTop;

            internal double TopLeft;

            internal double TopRight;

            internal double RightTop;

            internal double RightBottom;

            internal double BottomRight;

            internal double BottomLeft;

            internal double LeftBottom;
        }
    }
}
