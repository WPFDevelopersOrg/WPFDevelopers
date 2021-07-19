using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    public class ElementAdorner : Adorner
    {
        private const double ThumbSize = 16, ElementMiniSize = 20;
        private Thumb tLeft, tRight, bLeftBottom, bRightBottom, tMove;
        private VisualCollection visualCollection;
        public ElementAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualCollection = new VisualCollection(this);
            visualCollection.Add(tMove = CreateMoveThumb());
            visualCollection.Add(tLeft = CreateThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top));
            visualCollection.Add(tRight = CreateThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
            visualCollection.Add(bLeftBottom = CreateThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));
            visualCollection.Add(bRightBottom = CreateThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var offset = ThumbSize / 2;
            var sz = new Size(ThumbSize, ThumbSize);
            var renderPen = new Pen(new SolidColorBrush(Colors.White), 2.0);
            var startPoint = new Point(AdornedElement.RenderSize.Width / 2, AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height);
            var endPoint = new Point(AdornedElement.RenderSize.Width / 2, AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height - 16);
            drawingContext.DrawLine(renderPen, startPoint, endPoint);
            tMove.Arrange(new Rect(new Point(0, 0), new Size(this.RenderSize.Width, this.RenderSize.Height)));
            tLeft.Arrange(new Rect(new Point(-offset, -offset), sz));
            tRight.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));
            bLeftBottom.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));
            bRightBottom.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));

        }
        private Thumb CreateMoveThumb()
        {
            var thumb = new Thumb()
            {
                Cursor = Cursors.SizeAll,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(GetMoveEllipse())
                }
            };
            thumb.DragDelta += (s, e) =>
            {
                var element = AdornedElement as FrameworkElement;
                if (element == null)
                    return;
                Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
            };
            return thumb;
        }
        Brush GetMoveEllipse()
        {
            return new DrawingBrush(new GeometryDrawing(Brushes.Transparent, null, null));
        }
        /// <summary>
        /// 创建Thumb
        /// </summary>
        /// <param name="cursor">鼠标</param>
        /// <param name="horizontal">水平</param>
        /// <param name="vertical">垂直</param>
        /// <returns></returns>
        private Thumb CreateThumb(Cursor cursor, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            var thumb = new Thumb()
            {
                Cursor = cursor,
                Width = ThumbSize,
                Height = ThumbSize,
                HorizontalAlignment = horizontal,
                VerticalAlignment = vertical,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(new SolidColorBrush(Colors.White))
                }
            };

            thumb.DragDelta += (s, e) =>
            {
                var element = AdornedElement as FrameworkElement;
                if (element == null) return;
                Resize(element);
                switch (thumb.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        if (element.Height + e.VerticalChange > ElementMiniSize)
                        {
                            element.Height += e.VerticalChange;

                        }
                        break;
                    case VerticalAlignment.Top:
                        if (element.Height - e.VerticalChange > ElementMiniSize)
                        {
                            element.Height -= e.VerticalChange;
                            Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
                        }
                        break;

                }
                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (element.Width - e.HorizontalChange > ElementMiniSize)
                        {
                            element.Width -= e.HorizontalChange;
                            Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                        }
                        break;
                    case HorizontalAlignment.Right:
                        if (element.Width + e.HorizontalChange > ElementMiniSize)
                        {
                            element.Width += e.HorizontalChange;
                        }
                        break;
                }
                e.Handled = true;

            };
            return thumb;
        }
        private void Resize(FrameworkElement fElement)
        {
            if (Double.IsNaN(fElement.Width))
                fElement.Width = fElement.RenderSize.Width;
            if (Double.IsNaN(fElement.Height))
                fElement.Height = fElement.RenderSize.Height;
        }
        private FrameworkElementFactory GetFactory(Brush back)
        {
            var elementFactory = new FrameworkElementFactory(typeof(Ellipse));
            elementFactory.SetValue(Ellipse.FillProperty, back);
            return elementFactory;
        }


        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return visualCollection.Count;
            }
        }
    }
}
