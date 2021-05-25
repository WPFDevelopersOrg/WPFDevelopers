using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CutImage
{
    public class ImageAdorner : Adorner
    {
        const double THUMB_SIZE = 12;

        const double MINIMAL_SIZE = 20;

        const double MOVE_OFFSET = 20;
        Thumb tl, tr, bl, br;
        Thumb mov;
        VisualCollection visCollec;
        public ImageAdorner(UIElement adornedElement) : base(adornedElement)
        {

            visCollec = new VisualCollection(this);

            visCollec.Add(tl = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top));

            visCollec.Add(tr = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));

            visCollec.Add(bl = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));

            visCollec.Add(br = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));

            visCollec.Add(mov = GetMoveThumb());
        }
        protected override Size ArrangeOverride(Size finalSize)
        {

            double offset = THUMB_SIZE / 2;

            Size sz = new Size(THUMB_SIZE, THUMB_SIZE);

            tl.Arrange(new Rect(new Point(-offset, -offset), sz));

            tr.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));

            bl.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));

            br.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));

            mov.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, -MOVE_OFFSET), sz));

            return finalSize;
        }

        void Resize(FrameworkElement ff)
        {
            if (Double.IsNaN(ff.Width))

                ff.Width = ff.RenderSize.Width;

            if (Double.IsNaN(ff.Height))

                ff.Height = ff.RenderSize.Height;

        }



        Thumb GetMoveThumb()
        {
            var thumb = new Thumb()
            {
                Width = THUMB_SIZE,
                Height = THUMB_SIZE,
                Cursor = Cursors.SizeAll,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(GetMoveEllipseBack())
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



        Thumb GetResizeThumb(Cursor cur, HorizontalAlignment hor, VerticalAlignment ver)
        {

            var thumb = new Thumb()

            {

                Background = Brushes.Red,

                Width = THUMB_SIZE,

                Height = THUMB_SIZE,

                HorizontalAlignment = hor,

                VerticalAlignment = ver,

                Cursor = cur,

                Template = new ControlTemplate(typeof(Thumb))

                {

                    VisualTree = GetFactory(new SolidColorBrush(Colors.Green))

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
                        {

                            element.Height += e.VerticalChange;

                        }

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
                        {

                            element.Width += e.HorizontalChange;

                        }

                        break;

                }



                e.Handled = true;

            };

            return thumb;

        }



        Brush GetMoveEllipseBack()
        {

            string lan = "M 0,5 h 10 M 5,0 v 10";

            var converter = TypeDescriptor.GetConverter(typeof(Geometry));

            var geometry = (Geometry)converter.ConvertFrom(lan);

            TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Green, 2), geometry));

            bsh.Stretch = Stretch.Fill;

            return bsh;

        }



        FrameworkElementFactory GetFactory(Brush back)
        {

            back.Opacity = 0.6;

            var fef = new FrameworkElementFactory(typeof(Ellipse));

            fef.SetValue(Ellipse.FillProperty, back);

            fef.SetValue(Ellipse.StrokeProperty, Brushes.Green);

            fef.SetValue(Ellipse.StrokeThicknessProperty, (double)1);

            return fef;

        }



        protected override Visual GetVisualChild(int index)
        {

            return visCollec[index];

        }



        protected override int VisualChildrenCount
        {

            get
            {

                return visCollec.Count;

            }

        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

        //    SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
        //    Pen renderPen = new Pen(new SolidColorBrush(Colors.Red), 1.5d);
        //    double renderRadius = 5.0;

        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);

        //}
    }
}
