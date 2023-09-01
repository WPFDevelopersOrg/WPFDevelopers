using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.Controls
{
    public class ElementAdorner : Adorner
    {
        private const double ThumbSize = 16, ElementMiniSize = 20;
        private const double RotateThumbSize = 20;
        private readonly Thumb tLeft;
        private readonly Thumb tRight;
        private readonly Thumb bLeftBottom;
        private readonly Thumb bRightBottom;
        private readonly Thumb tMove;
        private readonly VisualCollection visualCollection;
        private readonly Thumb tRotate;

        private Canvas canvas;
        private Point centerPoint;
        private FrameworkElement designerItem;
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;
        public ElementAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualCollection = new VisualCollection(this);
            visualCollection.Add(tMove = CreateMoveThumb());
            visualCollection.Add(tLeft = CreateThumb(Cursors.SizeNWSE, HorizontalAlignment.Left,
                VerticalAlignment.Top));
            visualCollection.Add(tRight =
                CreateThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
            visualCollection.Add(bLeftBottom =
                CreateThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));
            visualCollection.Add(bRightBottom =
                CreateThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));
            visualCollection.Add(tRotate = CreateRotateThumb());
        }

        protected override int VisualChildrenCount => visualCollection.Count;

        protected override void OnRender(DrawingContext drawingContext)
        {
            var offset = ThumbSize / 2;
            var sz = new Size(ThumbSize, ThumbSize);
            var renderPen = new Pen(ControlsHelper.Brush, 2.0);
            var startPoint = new Point(AdornedElement.RenderSize.Width / 2,
                AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height);
            var endPoint = new Point(AdornedElement.RenderSize.Width / 2,
                AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height - 16);
            drawingContext.DrawLine(renderPen, startPoint, endPoint);
            tMove.Arrange(new Rect(new Point(0, 0), new Size(RenderSize.Width, RenderSize.Height)));
            tLeft.Arrange(new Rect(new Point(-offset, -offset), sz));
            tRight.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));
            bLeftBottom.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));
            bRightBottom.Arrange(new Rect(
                new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));
            tRotate.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - 10, -(RotateThumbSize - 5)), new Size(20, 20)));

            var adornedElementRect = new Rect(AdornedElement.RenderSize);
            var pen = new Pen(ControlsHelper.Brush, 2);
            drawingContext.DrawRectangle(null, pen, adornedElementRect);
        }

        private Thumb CreateMoveThumb()
        {
            var thumb = new Thumb
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

                var transform = element.RenderTransform;
                if (transform != null)
                {
                    if (transform as RotateTransform != null)
                    {
                        var rotateTransform = transform as RotateTransform;

                        Point ptChange = rotateTransform.Transform(new Point(e.HorizontalChange, e.VerticalChange));

                        SetPositon(element, Canvas.GetLeft(element) + ptChange.X, Canvas.GetTop(element) + ptChange.Y);
                    }
                    else
                    {
                        SetPositon(element, Canvas.GetLeft(element) + e.HorizontalChange, Canvas.GetTop(element) + e.VerticalChange);
                    }
                }
               
            };
            return thumb;
        }

        private Brush GetMoveEllipse()
        {
            return new DrawingBrush(new GeometryDrawing(Brushes.Transparent, null, null));
        }

        /// <summary>
        ///     创建Thumb
        /// </summary>
        /// <param name="cursor">鼠标</param>
        /// <param name="horizontal">水平</param>
        /// <param name="vertical">垂直</param>
        /// <returns></returns>
        private Thumb CreateThumb(Cursor cursor, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            var thumb = new Thumb
            {
                Cursor = cursor,
                Width = ThumbSize,
                Height = ThumbSize,
                HorizontalAlignment = horizontal,
                VerticalAlignment = vertical,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(ControlsHelper.Brush)
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
                        if (element.Height + e.VerticalChange > ElementMiniSize) element.Height += e.VerticalChange;
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
                        if (element.Width + e.HorizontalChange > ElementMiniSize) element.Width += e.HorizontalChange;
                        break;
                }

                e.Handled = true;
            };
            return thumb;
        }

        private void Resize(FrameworkElement fElement)
        {
            if (double.IsNaN(fElement.Width))
                fElement.Width = fElement.RenderSize.Width;
            if (double.IsNaN(fElement.Height))
                fElement.Height = fElement.RenderSize.Height;
        }

        private FrameworkElementFactory GetFactory(Brush back)
        {
            var elementFactory = new FrameworkElementFactory(typeof(Ellipse));
            elementFactory.SetValue(Shape.FillProperty, back);
            return elementFactory;
        }


        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        private void SetPositon(FrameworkElement element,double left, double top)
        {
            var parent = VisualTreeHelper.GetParent(element) as Canvas;

            if (left <= 0)
            {
                left = 0;
            }

            if (top <= 0)
            {
                top = 0;
            }

            if (left + element.Width > parent.ActualWidth)
            {
                left = parent.ActualWidth - element.Width;
            }
            if (top + element.Height > parent.ActualHeight)
            {
                top = parent.ActualHeight - element.Height;
            }

            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
        }
        /// <summary>
        /// 旋转
        /// </summary>
        /// <returns></returns>
        private Thumb CreateRotateThumb()
        {
            Thumb thumb = new Thumb()
            {
                Cursor = Cursors.Hand,
                Width = RotateThumbSize,
                Height = RotateThumbSize,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, -RotateThumbSize, 0, 0),
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(GetFactoryRotate()),
                },
            };
            thumb.DragDelta += Thumb_DragDelta;
            thumb.DragStarted += Thumb_DragStarted;
            return thumb;
        }

       
        private Brush GetFactoryRotate()
        {
            var lan =
                "M242 842l60-60c48 36 106 60 168 68v86c-86-8-164-42-228-94zM554 850c62-8 120-32 166-68l62 60c-64 52-142 86-228 94v-86zM782 722c36-48 60-104 68-166h86c-8 86-42 162-94 226zM640 512c0 70-58 128-128 128s-128-58-128-128 58-128 128-128 128 58 128 128zM174 554c8 62 32 120 68 166l-60 62c-52-64-86-142-94-228h86zM242 302c-36 48-60 106-68 168h-86c8-86 42-164 94-228zM850 470c-8-62-32-120-68-168l60-60c52 64 86 142 94 228h-86zM782 182l-60 60c-48-36-106-60-168-68v-86c86 8 164 42 228 94zM470 174c-62 8-120 32-168 68l-60-60c64-52 142-86 228-94v86z";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var geometry = (Geometry)converter.ConvertFrom(lan);
            TileBrush bsh =
                new DrawingBrush(new GeometryDrawing(ControlsHelper.Brush, new Pen(Brushes.Transparent, 2), geometry));
            bsh.Stretch = Stretch.Fill;
            return bsh;
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (designerItem != null && canvas != null)
            {
                var currentPoint = Mouse.GetPosition(canvas);
                var deltaVector = Point.Subtract(currentPoint, centerPoint);

                var angle = Vector.AngleBetween(startVector, deltaVector);

                var rotateTransform = designerItem.RenderTransform as RotateTransform;
                rotateTransform.Angle = initialAngle + Math.Round(angle, 0);
                designerItem.InvalidateMeasure();
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var thumb = sender as Thumb;
            designerItem = AdornedElement as FrameworkElement;
            canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
            if (canvas != null)
            {
                centerPoint = designerItem.TranslatePoint(
                    new Point(designerItem.Width * designerItem.RenderTransformOrigin.X,
                        designerItem.Height * designerItem.RenderTransformOrigin.Y),
                    canvas);

                var startPoint = Mouse.GetPosition(canvas);
                startVector = Point.Subtract(startPoint, centerPoint);

                rotateTransform = designerItem.RenderTransform as RotateTransform;
                if (rotateTransform == null)
                {
                    designerItem.RenderTransform = new RotateTransform(0);
                    initialAngle = 0;
                }
                else
                {
                    initialAngle = rotateTransform.Angle;
                }
            }
        }
    }
}