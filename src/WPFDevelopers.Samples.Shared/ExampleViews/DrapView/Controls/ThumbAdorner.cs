using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFDevelopers.Core;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class ThumbAdorner : Adorner
    {
        private const double THUMB_SIZE = 5;
        private const double MINIMAL_SIZE = 20;
        private const double RotateThumbSize = 20;
        private readonly Thumb lc;
        private readonly Thumb tl;
        private readonly Thumb tc;
        private readonly Thumb tr;
        private readonly Thumb rc;
        private readonly Thumb br;
        private readonly Thumb bc;
        private readonly Thumb bl;
        private readonly Thumb tMove;
        private readonly Thumb tRotate;
        private readonly VisualCollection visCollec;
        private Canvas canvas;
        private readonly FrameworkElement child;
        private Point centerPoint;
        private FrameworkElement designerItem;
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;
        private TransformThumb parentElemt;

        public ThumbAdorner(UIElement adorned) : base(adorned)
        {
            canvas = FindParent(adorned) as Canvas;
            child = FindVisualChild<Rectangle>(adorned);
            parentElemt = adorned as TransformThumb;
            visCollec = new VisualCollection(this);
            // 创建移动
            visCollec.Add(tMove = CreateMoveThumb());
            visCollec.Add(lc = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center));
            visCollec.Add(tl = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top));
            visCollec.Add(tc = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Top));
            visCollec.Add(tr = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
            visCollec.Add(rc = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center));
            visCollec.Add(br = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));
            visCollec.Add(bc = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Bottom));
            visCollec.Add(bl = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));
            visCollec.Add(tRotate = CreateRotateThumb());
        }

        protected override int VisualChildrenCount => visCollec.Count;

        protected override void OnRender(DrawingContext drawingContext)
        {
            var offset = THUMB_SIZE / 2;
            var sz = new Size(THUMB_SIZE, THUMB_SIZE);
            var renderPen = new Pen(ThemeManager.Instance.PrimaryBrush, 2.0);
            var startPoint = new Point(AdornedElement.RenderSize.Width / 2,
                AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height);
            var endPoint = new Point(AdornedElement.RenderSize.Width / 2,
                AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height - 16);
            drawingContext.DrawLine(renderPen, startPoint, endPoint);
            tMove.Arrange(new Rect(new Point(0, 0), new Size(RenderSize.Width, RenderSize.Height)));

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

            tRotate.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - 10, -(RotateThumbSize - 5)), new Size(20, 20)));

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

        private void SetPositon(FrameworkElement element, double left, double top)
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

        private Brush GetMoveEllipse()
        {
            return new DrawingBrush(new GeometryDrawing(Brushes.Transparent, null, null));
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <returns></returns>
        private Thumb CreateRotateThumb()
        {
            Thumb thumb = new Thumb()
            {
                Cursor = Cursors.ScrollAll,
                Width = RotateThumbSize,
                Height = RotateThumbSize,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, -RotateThumbSize, 0, 0),
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetEllipseFactory(GetFactoryRotate()),
                },
            };
            thumb.DragDelta += Thumb_DragDelta;
            thumb.DragStarted += Thumb_DragStarted;
            return thumb;
        }

        private FrameworkElementFactory GetEllipseFactory(Brush back)
        {
            var elementFactory = new FrameworkElementFactory(typeof(Ellipse));
            elementFactory.SetValue(Shape.FillProperty, back);
            return elementFactory;
        }

        private Brush GetFactoryRotate()
        {
            var lan =
                "M242 842l60-60c48 36 106 60 168 68v86c-86-8-164-42-228-94zM554 850c62-8 120-32 166-68l62 60c-64 52-142 86-228 94v-86zM782 722c36-48 60-104 68-166h86c-8 86-42 162-94 226zM640 512c0 70-58 128-128 128s-128-58-128-128 58-128 128-128 128 58 128 128zM174 554c8 62 32 120 68 166l-60 62c-52-64-86-142-94-228h86zM242 302c-36 48-60 106-68 168h-86c8-86 42-164 94-228zM850 470c-8-62-32-120-68-168l60-60c52 64 86 142 94 228h-86zM782 182l-60 60c-48-36-106-60-168-68v-86c86 8 164 42 228 94zM470 174c-62 8-120 32-168 68l-60-60c64-52 142-86 228-94v86z";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var geometry = (Geometry)converter.ConvertFrom(lan);
            TileBrush bsh =
                new DrawingBrush(new GeometryDrawing(ThemeManager.Instance.PrimaryBrush, new Pen(Brushes.Transparent, 2), geometry));
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

        private static UIElement FindParent(UIElement element)
        {
            DependencyObject obj = element;
            obj = VisualTreeHelper.GetParent(obj);
            return obj as UIElement;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    ContentPresenter element = child as ContentPresenter;
                    if (element != null && element.Content is childItem)
                    {
                        return (childItem)element.Content;
                    }
                }
            }
            return null;
        }
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
            child.Width = frameworkElement.RenderSize.Width;
            child.Height = frameworkElement.RenderSize.Height;
        }

        private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment hor, VerticalAlignment ver)
        {
            // 设置拖拽控件实例
            var thumb = new Thumb
            {
                Width = THUMB_SIZE,
                Height = THUMB_SIZE,
                HorizontalAlignment = hor,
                VerticalAlignment = ver,
                Cursor = cur,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(ThemeManager.Instance.PrimaryBrush)
                }
            };

            // 最大宽度
            var maxWidth = double.IsNaN(canvas.Width) ? canvas.ActualWidth : canvas.Width;
            // 最大高度
            var maxHeight = double.IsNaN(canvas.Height) ? canvas.ActualHeight : canvas.Height;
            // 订阅拖拽开始事件
            thumb.DragStarted += (s, e) => {
                Debug.WriteLine($"HorizontalOffset:{e.HorizontalOffset}");
                Debug.WriteLine($"VerticalOffset:{e.VerticalOffset}");
            };
            // 订阅拖拽事件
            thumb.DragDelta += (s, e) =>
            {
                // 转换遮罩目标节点元素为基础元素节点
                var element = AdornedElement as FrameworkElement;
                if (element == null)
                    return;
                // 刷新节点大小
                Resize(element);
                // 判定拖动控件所属垂直位置
                switch (thumb.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        if (element.Height + e.VerticalChange > MINIMAL_SIZE)
                        {
                            var newHeight = element.Height + e.VerticalChange;
                            var top = Canvas.GetTop(element) + newHeight;
                            if (newHeight > 0 && top <= canvas.ActualHeight)
                                element.Height = newHeight;
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
                            }
                        }

                        break;
                }
                // 判定拖动控件所属水平位置
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
                            }
                        }

                        break;
                    case HorizontalAlignment.Right:
                        if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
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
            thumb.DragCompleted += (s, e) => {
                Debug.WriteLine($"HorizontalChange:{e.HorizontalChange}");
                Debug.WriteLine($"VerticalChange:{e.VerticalChange}");
            };
            return thumb;
        }

        private FrameworkElementFactory GetFactory(Brush back)
        {
            var fef = new FrameworkElementFactory(typeof(Rectangle));
            fef.SetValue(Shape.FillProperty, back);
            fef.SetValue(Shape.StrokeProperty, ThemeManager.Instance.PrimaryBrush);
            fef.SetValue(Shape.StrokeThicknessProperty, (double)1);
            return fef;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visCollec[index];
        }
    }
}
