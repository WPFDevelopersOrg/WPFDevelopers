using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFDevelopers.Core;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.Controls
{
    public class ElementAdorner : Adorner
    {
        public event RoutedPropertyChangedEventHandler<double> AngleChanged;

        protected virtual void OnAngleChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            AngleChanged?.Invoke(this, args);
        }

        private double angle;
        public double Angle
        {
            get { return angle; }
            set
            {
                if (angle != value)
                {
                    double oldValue = angle;
                    angle = value;
                    OnAngleChanged(oldValue, angle);
                }
            }
        }

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
            canvas = FindParent(adornedElement) as Canvas;
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
            var renderPen = new Pen(ThemeManager.Instance.BackgroundBrush, 2.0);
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
            var pen = new Pen(ThemeManager.Instance.BackgroundBrush, 2);
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

        UIElement FindParent(UIElement element)
        {
            DependencyObject obj = element;
            obj = VisualTreeHelper.GetParent(obj);
            return obj as UIElement;
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
                    VisualTree = GetFactory(ThemeManager.Instance.BackgroundBrush)
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
                            var newHeight = element.Height + e.VerticalChange;
                            var top = Canvas.GetTop(element) + newHeight;
                            if (newHeight > 0 && top <= canvas.ActualHeight)
                                element.Height = newHeight;
                        }
                        break;
                    case VerticalAlignment.Top:
                        if (element.Height - e.VerticalChange > ElementMiniSize)
                        {
                            var newHeight = element.Height - e.VerticalChange;
                            var top = Canvas.GetTop(element) + e.VerticalChange;
                            if (newHeight > 0 && top >= 0)
                            {
                                element.Height = newHeight;
                                Canvas.SetTop(element, top);
                            }
                        }

                        break;
                }

                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (element.Width - e.HorizontalChange > ElementMiniSize)
                        {
                            var newWidth = element.Width - e.HorizontalChange;
                            var left = Canvas.GetLeft(element) + e.HorizontalChange;
                            if (newWidth > 0 && left >= 0)
                            {
                                element.Width = newWidth;
                                Canvas.SetLeft(element, left);
                            }
                        }

                        break;
                    case HorizontalAlignment.Right:
                        if (element.Width + e.HorizontalChange > ElementMiniSize)
                        {
                            var newWidth = element.Width + e.HorizontalChange;
                            var left = Canvas.GetLeft(element) + newWidth;
                            if (newWidth > 0 && left <= canvas.ActualWidth)
                                element.Width = newWidth;
                        }
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
                HorizontalAlignment = HorizontalAlignment.Center,
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
                "M35.072 512h121.446v-10.496c5.53-227.021 190.669-409.395 418.253-409.395 231.168 0 418.509 188.006 418.509 419.891s-187.341 419.789-418.509 419.789c-97.178 0-186.624-33.28-257.69-88.986l71.987-77.005c52.019 38.298 116.224 61.082 185.702 61.082 173.363 0 313.907-141.005 313.907-314.88s-140.544-314.88-313.907-314.88c-169.83 0-308.122 135.322-313.6 304.384v10.496h136.806l-178.893 199.373-184.013-199.373z";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var geometry = (Geometry)converter.ConvertFrom(lan);
            var bsh =
                new DrawingBrush(new GeometryDrawing(ThemeManager.Instance.BackgroundBrush, new Pen(Brushes.Transparent, 2), geometry));
            bsh.Stretch = Stretch.Uniform;
            return bsh;
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (designerItem != null && canvas != null)
            {
                var currentPoint = Mouse.GetPosition(canvas);
                var deltaVector = Point.Subtract(currentPoint, centerPoint);
                var angle = Vector.AngleBetween(startVector, deltaVector);
                Angle = angle;
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
                    initialAngle = rotateTransform.Angle;
            }
        }
    }
}