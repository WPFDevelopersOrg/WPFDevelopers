using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    public class ScreenCutAdorner : Adorner
    {
        private const double THUMB_SIZE = 15;
        private const double MINIMAL_SIZE = 20;
        private readonly Thumb lc;
        private readonly Thumb tl;
        private readonly Thumb tc;
        private readonly Thumb tr;
        private readonly Thumb rc;
        private readonly Thumb br;
        private readonly Thumb bc;
        private readonly Thumb bl;
        private readonly VisualCollection visCollec;

        public ScreenCutAdorner(UIElement adorned) : base(adorned)
        {
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

        protected override int VisualChildrenCount => visCollec.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var offset = THUMB_SIZE / 2;
            var sz = new Size(THUMB_SIZE, THUMB_SIZE);
            lc.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height / 2 - offset), sz));
            tl.Arrange(new Rect(new Point(-offset, -offset), sz));
            tc.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - offset, -offset), sz));
            tr.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));
            rc.Arrange(new Rect(
                new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height / 2 - offset),
                sz));
            br.Arrange(new Rect(
                new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));
            bc.Arrange(new Rect(
                new Point(AdornedElement.RenderSize.Width / 2 - offset, AdornedElement.RenderSize.Height - offset),
                sz));
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
            var thumb = new Thumb
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
                            element.Height += e.VerticalChange;
                        break;

                    case VerticalAlignment.Top:
                        if (element.Height - e.VerticalChange > MINIMAL_SIZE)
                        {
                            element.Height -= e.VerticalChange;
                            Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
                        }

                        break;
                }

                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (element.Width - e.HorizontalChange > MINIMAL_SIZE)
                        {
                            element.Width -= e.HorizontalChange;
                            Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                        }

                        break;
                    case HorizontalAlignment.Right:
                        if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
                            element.Width += e.HorizontalChange;
                        break;
                }

                e.Handled = true;
            };
            return thumb;
        }

        private FrameworkElementFactory GetFactory(Brush back)
        {
            var fef = new FrameworkElementFactory(typeof(Ellipse));
            fef.SetValue(Shape.FillProperty, back);
            fef.SetValue(Shape.StrokeProperty, DrawingContextHelper.Brush);
            fef.SetValue(Shape.StrokeThicknessProperty, (double)2);
            return fef;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visCollec[index];
        }
    }
}