using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CutImage
{
    /// <summary>
    /// ImageCutCustoms.xaml 的交互逻辑
    /// </summary>
    public partial class ImageCutCustoms : UserControl
    {


        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageCutCustoms), new PropertyMetadata(ImageSourcePropertyChangedCallback));




        public ImageSource SaveImageSource
        {
            get { return (ImageSource)GetValue(SaveImageSourceProperty); }
            set { SetValue(SaveImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveImageSourceProperty =
            DependencyProperty.Register("SaveImageSource", typeof(ImageSource), typeof(ImageCutCustoms), new PropertyMetadata());



        public Rect CutRect
        {
            get { return (Rect)GetValue(CutRectProperty); }
            set { SetValue(CutRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CutRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CutRectProperty =
            DependencyProperty.Register("CutRect", typeof(Rect), typeof(ImageCutCustoms), new PropertyMetadata());

        private Point startPoint, endPoint;

        public ImageCutCustoms()
        {
            InitializeComponent();
            this.dragDropItem.UpdateImageEvent += DragDropItem_UpdateImageEvent;
        }
        private void DragDropItem_UpdateImageEvent()
        {
            var x = Canvas.GetLeft(dragDropItem);
            var y = Canvas.GetTop(dragDropItem);
            var w = dragDropItem.Width;
            var h = dragDropItem.Height;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rectImage.RenderSize.Width,
    (int)rectImage.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(rectImage);

            var crop = new CroppedBitmap(rtb, new Int32Rect((int)x, (int)y, (int)w, (int)h));
            SaveImageSource = crop;
            startPoint = new Point(x, y);
            endPoint = new Point(x+w, y+h);
            CutRect = new Rect(startPoint, endPoint);
        }
        private static void ImageSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cutCustoms = d as ImageCutCustoms;
            var x = cutCustoms.ActualWidth / 3;
            var y = cutCustoms.ActualHeight / 3;
            cutCustoms.startPoint = new Point(x, y);
            cutCustoms.endPoint = new Point(x + 120, y + 120);
            cutCustoms.CutRect = new Rect(cutCustoms.startPoint,cutCustoms.endPoint);
            //var layer = AdornerLayer.GetAdornerLayer(cutCustoms.RecDragDrop);
            //layer.Add(new ImageAdorner(cutCustoms.RecDragDrop));
            
        }
       
    }
}
