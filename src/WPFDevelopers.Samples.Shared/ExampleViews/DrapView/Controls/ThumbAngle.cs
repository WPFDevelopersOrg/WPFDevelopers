using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class ThumbAngle : Thumb
    {
        private Canvas canvas;
        private Point centerPoint;
        private Border designerItem;
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;

        public ThumbAngle()
        {
            Cursor = Cursors.Hand;
            Width = 16;
            Height = 16;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(0, -28, 0, 0);
            Template = new ControlTemplate(typeof(Thumb))
            {
                VisualTree = GetFactory(GetMoveEllipseBack())
            };
            ToolTip = "旋转";
            DragDelta += Thumb_DragDelta;
            DragStarted += Thumb_DragStarted;
        }

        private Brush GetMoveEllipseBack()
        {
            var lan =
                "M242 842l60-60c48 36 106 60 168 68v86c-86-8-164-42-228-94zM554 850c62-8 120-32 166-68l62 60c-64 52-142 86-228 94v-86zM782 722c36-48 60-104 68-166h86c-8 86-42 162-94 226zM640 512c0 70-58 128-128 128s-128-58-128-128 58-128 128-128 128 58 128 128zM174 554c8 62 32 120 68 166l-60 62c-52-64-86-142-94-228h86zM242 302c-36 48-60 106-68 168h-86c8-86 42-164 94-228zM850 470c-8-62-32-120-68-168l60-60c52 64 86 142 94 228h-86zM782 182l-60 60c-48-36-106-60-168-68v-86c86 8 164 42 228 94zM470 174c-62 8-120 32-168 68l-60-60c64-52 142-86 228-94v86z";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var geometry = (Geometry)converter.ConvertFrom(lan);
            TileBrush bsh =
                new DrawingBrush(new GeometryDrawing(Brushes.White, new Pen(Brushes.Transparent, 2), geometry));
            bsh.Stretch = Stretch.Fill;
            return bsh;
        }

        private FrameworkElementFactory GetFactory(Brush back)
        {
            var elementFactory = new FrameworkElementFactory(typeof(Ellipse));
            elementFactory.SetValue(Shape.FillProperty, back);
            return elementFactory;
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
            designerItem = thumb.DataContext as Border;
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
