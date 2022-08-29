using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = UniformGridTemplateName, Type = typeof(UniformGrid))]
    public class CropControl : Control
    {
        private const string UniformGridTemplateName = "PART_UniformGrid";

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CropControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty RowColumnProperty =
            DependencyProperty.Register("RowColumn", typeof(int), typeof(CropControl), new PropertyMetadata(3));

        private UniformGrid _uniformGrid;

        static CropControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CropControl),
                new FrameworkPropertyMetadata(typeof(CropControl)));
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public int RowColumn
        {
            get => (int)GetValue(RowColumnProperty);
            set => SetValue(RowColumnProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _uniformGrid = GetTemplateChild(UniformGridTemplateName) as UniformGrid;
            if (ImageSource == null || _uniformGrid == null) return;

            var imgSource = (BitmapSource)ImageSource;
            int w = 0, h = 0;
            if (!imgSource.PixelWidth.Equals(0)
                &&
                !imgSource.PixelHeight.Equals(0))
            {
                w = imgSource.PixelWidth / RowColumn;
                h = imgSource.PixelHeight / RowColumn;
                _uniformGrid.Width = imgSource.PixelWidth;
                _uniformGrid.Height = imgSource.PixelHeight;
            }

            for (var i = 0; i < RowColumn; i++)
            for (var j = 0; j < RowColumn; j++)
            {
                var rect = new Rectangle
                {
                    Fill = new ImageBrush
                        { ImageSource = new CroppedBitmap(imgSource, new Int32Rect(j * w, i * h, w, h)) },
                    StrokeThickness = .5,
                    Stroke = Brushes.White,
                    Cursor = Cursors.Hand
                };
                rect.RenderTransformOrigin = new Point(.5, .5);
                rect.RenderTransform = new ScaleTransform();
                rect.MouseMove += (sender, ex) =>
                {
                    var rect1 = sender as Rectangle;
                    Panel.SetZIndex(rect1, 1);
                    var doubleAnimation = new DoubleAnimation
                    {
                        To = 2,
                        Duration = TimeSpan.FromMilliseconds(100)
                    };
                    var scaleTransform = rect1.RenderTransform as ScaleTransform;
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
                };
                rect.MouseLeave += (sender, ex) =>
                {
                    var rect1 = sender as Rectangle;
                    Panel.SetZIndex(rect1, 0);
                    var scaleTransform = rect1.RenderTransform as ScaleTransform;
                    var doubleAnimation = new DoubleAnimation
                    {
                        To = 1,
                        Duration = TimeSpan.FromMilliseconds(100)
                    };
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
                };
                _uniformGrid.Children.Add(rect);
            }
        }
    }
}