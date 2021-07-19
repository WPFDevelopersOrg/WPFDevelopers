using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    public class ThumbAngle:Thumb
    {
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;
        private Point centerPoint;
        private Border designerItem;
        private Canvas canvas;
        public ThumbAngle()
        {
            Cursor = Cursors.Hand;
            Width = 16;
            Height = 16;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(0,-28,0,0);
            Template = new ControlTemplate(typeof(Thumb))
            {
                VisualTree = GetFactory(GetMoveEllipseBack())
            };
            ToolTip = "旋转";
            DragDelta += new DragDeltaEventHandler(this.Thumb_DragDelta);
            DragStarted += new DragStartedEventHandler(this.Thumb_DragStarted);
        }
        Brush GetMoveEllipseBack()
        {

            string lan = "M242 842l60-60c48 36 106 60 168 68v86c-86-8-164-42-228-94zM554 850c62-8 120-32 166-68l62 60c-64 52-142 86-228 94v-86zM782 722c36-48 60-104 68-166h86c-8 86-42 162-94 226zM640 512c0 70-58 128-128 128s-128-58-128-128 58-128 128-128 128 58 128 128zM174 554c8 62 32 120 68 166l-60 62c-52-64-86-142-94-228h86zM242 302c-36 48-60 106-68 168h-86c8-86 42-164 94-228zM850 470c-8-62-32-120-68-168l60-60c52 64 86 142 94 228h-86zM782 182l-60 60c-48-36-106-60-168-68v-86c86 8 164 42 228 94zM470 174c-62 8-120 32-168 68l-60-60c64-52 142-86 228-94v86z";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var geometry = (Geometry)converter.ConvertFrom(lan);
            TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.White, new Pen(Brushes.Transparent,2), geometry));
            bsh.Stretch = Stretch.Fill;
            return bsh;

        }
        private FrameworkElementFactory GetFactory(Brush back)
        {
            var elementFactory = new FrameworkElementFactory(typeof(Ellipse));
            elementFactory.SetValue(Ellipse.FillProperty, back);
            return elementFactory;
        }
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.designerItem != null && this.canvas != null)
            {
                Point currentPoint = Mouse.GetPosition(this.canvas);
                Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);

                double angle = Vector.AngleBetween(this.startVector, deltaVector);

                RotateTransform rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                rotateTransform.Angle = this.initialAngle + Math.Round(angle, 0);
                designerItem.InvalidateMeasure();
            }
        }
      
        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            var thumb = sender as Thumb;
            designerItem = thumb.DataContext as Border;
            canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
            if (this.canvas != null)
            {
                this.centerPoint = this.designerItem.TranslatePoint(
                    new Point(this.designerItem.Width * this.designerItem.RenderTransformOrigin.X,
                              this.designerItem.Height * this.designerItem.RenderTransformOrigin.Y),
                              this.canvas);

                Point startPoint = Mouse.GetPosition(this.canvas);
                this.startVector = Point.Subtract(startPoint, this.centerPoint);

                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                if (this.rotateTransform == null)
                {
                    this.designerItem.RenderTransform = new RotateTransform(0);
                    this.initialAngle = 0;
                }
                else
                {
                    this.initialAngle = this.rotateTransform.Angle;
                }
            }
        }
    }
}
