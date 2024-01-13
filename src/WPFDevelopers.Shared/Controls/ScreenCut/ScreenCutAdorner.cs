using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class ScreenCutAdorner : Adorner
    {
        private const double THUMB_SIZE = 15;
        private const double MINIMAL_SIZE = 20;
        private const double LINE_Size = 6;
        private readonly Thumb lc;
        private readonly Thumb tl;
        private readonly Thumb tc;
        private readonly Thumb tr;
        private readonly Thumb rc;
        private readonly Thumb br;
        private readonly Thumb bc;
        private readonly Thumb bl;
        private readonly VisualCollection visCollec;
        private readonly Canvas canvas;
        private readonly bool _isRatioScale;
        private readonly Size _scaleSize;

        public ScreenCutAdorner(UIElement adorned, bool isRatioScale = false, Size scaleSize = default) : base(adorned)
        {
            canvas = FindParent(adorned) as Canvas;
            _isRatioScale = isRatioScale;
            _scaleSize = scaleSize;
            visCollec = new VisualCollection(this);
            visCollec.Add(lc = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center));
            visCollec.Add(tl = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top));
            visCollec.Add(tc = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Top));
            visCollec.Add(tr = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
            visCollec.Add(rc = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center));
            visCollec.Add(br = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));
            visCollec.Add(bc = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Bottom));
            visCollec.Add(bl = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));
        }
        private static UIElement FindParent(UIElement element)
        {
            DependencyObject obj = element;
            obj = VisualTreeHelper.GetParent(obj);
            return obj as UIElement;
        }
        protected override int VisualChildrenCount => visCollec.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var offset = THUMB_SIZE / 2;
            var sz = new Size(THUMB_SIZE, THUMB_SIZE);
            if (!_isRatioScale)
            {
                lc.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height / 2 - offset), sz));
                tc.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - offset, -offset), sz));
                rc.Arrange(new Rect(
               new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height / 2 - offset),
               sz));
                bc.Arrange(new Rect(
              new Point(AdornedElement.RenderSize.Width / 2 - offset, AdornedElement.RenderSize.Height - offset),
              sz));
            }
            else
            {
                lc.Height = AdornedElement.RenderSize.Height;
                lc.Width = LINE_Size;
                lc.Arrange(new Rect(new Point(0, 0), new Size(lc.Width, lc.Height)));
                tc.Height = LINE_Size;
                tc.Width = AdornedElement.RenderSize.Width;
                tc.Arrange(new Rect(new Point(0, 0), new Size(tc.Width, tc.Height)));
                rc.Width = LINE_Size;
                rc.Height = AdornedElement.RenderSize.Height;
                rc.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - LINE_Size, 0), new Size(rc.Width, rc.Height)));
                bc.Height = LINE_Size;
                bc.Width = AdornedElement.RenderSize.Width;
                bc.Arrange(new Rect(new Point(0, AdornedElement.RenderSize.Height - LINE_Size), new Size(bc.Width, bc.Height)));
            }

            tl.Arrange(new Rect(new Point(-offset, -offset), sz));
            tr.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));

            br.Arrange(new Rect(
                new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));
            bl.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));
            return finalSize;
        }

        private void Resize(FrameworkElement frameworkElement)
        {
            if (double.IsNaN(frameworkElement.Width))
                frameworkElement.Width = frameworkElement.RenderSize.Width;
            if (double.IsNaN(frameworkElement.Height))
                frameworkElement.Height = frameworkElement.RenderSize.Height;
        }

        private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment hor, VerticalAlignment ver)
        {
            var thumb = new Thumb();
            if (_isRatioScale
                && (
                hor == HorizontalAlignment.Left && ver == VerticalAlignment.Center
                ||
                hor == HorizontalAlignment.Center && ver == VerticalAlignment.Top
                ||
                hor == HorizontalAlignment.Right && ver == VerticalAlignment.Center
                ||
                hor == HorizontalAlignment.Center && ver == VerticalAlignment.Bottom))
            {
                thumb = new Thumb
                {
                    HorizontalAlignment = hor == HorizontalAlignment.Center ? HorizontalAlignment.Stretch : hor,
                    VerticalAlignment = ver == VerticalAlignment.Center ? VerticalAlignment.Stretch : ver,
                    Cursor = cur,
                    Template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = GetFactoryRectangle()
                    }
                };
            }
            else
            {
                thumb = new Thumb
                {
                    Width = THUMB_SIZE,
                    Height = THUMB_SIZE,
                    HorizontalAlignment = hor,
                    VerticalAlignment = ver,
                    Cursor = cur,
                    Template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = GetFactory(new SolidColorBrush(Colors.White))
                    }
                };
            }


            var maxWidth = double.IsNaN(canvas.Width) ? canvas.ActualWidth : canvas.Width;
            var maxHeight = double.IsNaN(canvas.Height) ? canvas.ActualHeight : canvas.Height;
            thumb.DragDelta += (s, e) =>
            {
                var element = AdornedElement as FrameworkElement;
                if (element == null)
                    return;
                Resize(element);

                switch (thumb.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        if (element.Height + e.VerticalChange > MINIMAL_SIZE)
                        {
                            var newHeight = element.Height + e.VerticalChange;
                            var top = Canvas.GetTop(element) + newHeight;
                            if (newHeight > 0 && top <= canvas.ActualHeight)
                            {
                                element.Height = newHeight;
                                if(_isRatioScale)
                                    ScaleWidth(thumb, element, newHeight);
                            }
                        }
                        break;

                    case VerticalAlignment.Top:
                        if (element.Height - e.VerticalChange > MINIMAL_SIZE)
                        {
                            var newHeight = element.Height - e.VerticalChange;
                            var top = Canvas.GetTop(element);
                            if (newHeight > 0 && top + e.VerticalChange >= 0)
                            {
                                element.Height = newHeight;
                                Canvas.SetTop(element, top + e.VerticalChange);
                                if (_isRatioScale)
                                    ScaleWidth(thumb, element, newHeight);
                            }
                        }

                        break;
                }

                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (element.Width - e.HorizontalChange > MINIMAL_SIZE)
                        {
                            var newWidth = element.Width - e.HorizontalChange;
                            var left = Canvas.GetLeft(element);
                            if (newWidth > 0 && left + e.HorizontalChange >= 0)
                            {
                                element.Width = newWidth;
                                Canvas.SetLeft(element, left + e.HorizontalChange);
                                if (_isRatioScale)
                                    ScaleHeight(thumb, element, newWidth);
                            }
                        }

                        break;
                    case HorizontalAlignment.Right:
                        if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
                        {
                            var newWidth = element.Width + e.HorizontalChange;
                            var left = Canvas.GetLeft(element) + newWidth;
                            if (newWidth > 0 && left <= canvas.ActualWidth)
                            {
                                element.Width = newWidth;
                                if (_isRatioScale)
                                    ScaleHeight(thumb, element, newWidth);
                            }
                        }
                        break;
                }
                e.Handled = true;
            };
            return thumb;
        }

        void ScaleWidth(Thumb thumb, FrameworkElement element, double newHeight)
        {
            if (_isRatioScale
                                &&
                                thumb.VerticalAlignment != VerticalAlignment.Top
                                ||
                                (thumb.HorizontalAlignment != HorizontalAlignment.Left
                                &&
                                thumb.HorizontalAlignment != HorizontalAlignment.Right))
            {
                if (!_scaleSize.IsEmpty
                &&
                _scaleSize.Width > double.MinValue
                &&
               _scaleSize.Height > double.MinValue)
                {
                    var newWidth = _scaleSize.Width * newHeight;
                    var left = Canvas.GetLeft(element) + newWidth;
                    if (newWidth > 0 && left <= canvas.ActualWidth)
                        element.Width = newWidth;
                }
            }
        }

        void ScaleHeight(Thumb thumb, FrameworkElement element, double newWidth)
        {
            if (_isRatioScale
                               &&
            thumb.VerticalAlignment != VerticalAlignment.Top
                               ||
                               (thumb.HorizontalAlignment != HorizontalAlignment.Left
            &&
                               thumb.HorizontalAlignment != HorizontalAlignment.Right))
            {
                if (!_scaleSize.IsEmpty
                &&
                _scaleSize.Width > double.MinValue
                &&
               _scaleSize.Height > double.MinValue)
                {
                    var newHeight = newWidth / _scaleSize.Width;
                    var top = Canvas.GetTop(element) + newHeight;
                    if (newHeight > 0 && top <= canvas.ActualHeight)
                        element.Height = newHeight;
                }
            }
        }

        FrameworkElementFactory GetFactoryRectangle()
        {
            var fef = new FrameworkElementFactory(typeof(Rectangle));
            fef.SetValue(Shape.FillProperty, Brushes.Transparent);
            return fef;
        }

        private FrameworkElementFactory GetFactory(Brush back)
        {
            var fef = new FrameworkElementFactory(typeof(Ellipse));
            fef.SetValue(Shape.FillProperty, back);
            fef.SetValue(Shape.StrokeProperty, ControlsHelper.PrimaryNormalBrush);
            fef.SetValue(Shape.StrokeThicknessProperty, (double)2);
            return fef;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visCollec[index];
        }
    }
}