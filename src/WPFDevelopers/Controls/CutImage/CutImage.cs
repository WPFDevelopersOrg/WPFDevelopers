using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = DragDropTemplateName, Type = typeof(DragDrop))]
    [TemplatePart(Name = RectangleTemplateName, Type = typeof(Rectangle))]
    public class CutImage : Control
    {
        private const string DragDropTemplateName = "PART_DragDrop";
        private const string RectangleTemplateName = "PART_RectImage";

        private DragDrop _dragDrop;
        private Rectangle _rectangle;

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); 
                   DragDropItem_UpdateImageEvent();
            }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CutImage), new PropertyMetadata(ImageSourcePropertyChangedCallback));


        public ImageSource SaveImageSource
        {
            get { return (ImageSource)GetValue(SaveImageSourceProperty); }
            set { SetValue(SaveImageSourceProperty, value); }
        }

        public static readonly DependencyProperty SaveImageSourceProperty =
            DependencyProperty.Register("SaveImageSource", typeof(ImageSource), typeof(CutImage), new PropertyMetadata());



        public Rect CutRect
        {
            get { return (Rect)GetValue(CutRectProperty); }
            set { SetValue(CutRectProperty, value); }
        }

        public static readonly DependencyProperty CutRectProperty =
            DependencyProperty.Register("CutRect", typeof(Rect), typeof(CutImage), new PropertyMetadata());

        private Point startPoint, endPoint;
        static CutImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CutImage), new FrameworkPropertyMetadata(typeof(CutImage)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _dragDrop = GetTemplateChild(DragDropTemplateName) as DragDrop;
            _rectangle = GetTemplateChild(RectangleTemplateName) as Rectangle;
            _dragDrop.UpdateImageEvent += DragDropItem_UpdateImageEvent;
        }
        private void DragDropItem_UpdateImageEvent()
        {
            var x = Canvas.GetLeft(_dragDrop);
            var y = Canvas.GetTop(_dragDrop);
            var w = _dragDrop.Width;
            var h = _dragDrop.Height;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)_rectangle.RenderSize.Width,
                (int)_rectangle.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(_rectangle);

            var crop = new CroppedBitmap(rtb, new Int32Rect((int)x, (int)y, (int)w, (int)h));
            SaveImageSource = crop;
            startPoint = new Point(x, y);
            endPoint = new Point(x + w, y + h);
            CutRect = new Rect(startPoint, endPoint);
        }
        private static void ImageSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cutCustoms = d as CutImage;
            var x = cutCustoms.ActualWidth / 3;
            var y = cutCustoms.ActualHeight / 3;
            cutCustoms.startPoint = new Point(x, y);
            cutCustoms.endPoint = new Point(x + 120, y + 120);
            cutCustoms.CutRect = new Rect(cutCustoms.startPoint, cutCustoms.endPoint);
            cutCustoms.Width = cutCustoms.ActualWidth;
            cutCustoms.Height = cutCustoms.ActualHeight;
            cutCustoms._dragDrop.Visibility = Visibility.Visible;

        }
    }
}
