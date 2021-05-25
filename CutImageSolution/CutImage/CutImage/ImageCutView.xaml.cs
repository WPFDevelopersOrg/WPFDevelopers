using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace CutImage
{
    /// <summary>
    /// ImageCutView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageCutView : Window
    {
        public ImageCutView()
        {
            InitializeComponent();
            Loaded += ImageCutView_Loaded;
            this.dragDropItem.UpdateImageEvent += DragDropItem_UpdateImageEvent;
            this.btnImport.Click += OnImportClickHandler;
            this.btnSave.Click += BtnSave_Click;
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
            image2.ImageSource = crop;
        }

        private void ImageCutView_Loaded(object sender, RoutedEventArgs e)
        {
            var width = DrawCanvas.ActualWidth;
            var height = DrawCanvas.ActualHeight;
            Canvas.SetLeft(dragDropItem,width / 2 - dragDropItem.ActualWidth);
            Canvas.SetTop(dragDropItem, height / 2 - dragDropItem.ActualHeight);
        }
        private void OnImportClickHandler(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "图像文件(*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                imgCut.ImageSource = new BitmapImage(new Uri(openFileDialog.FileName));
                SetAreaInfo();
            }
        }
        private void SetAreaInfo()
        {
            if (this.dragDropItem != null)
            {
                this.dragDropItem.Visibility = Visibility.Visible;
                this.dragDropItem.ParentMaxHeight = this.DrawCanvas.ActualHeight;
                this.dragDropItem.ParentMaxWidth = this.DrawCanvas.ActualWidth;
            }
            
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName =$"{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "image file|*.jpg";

            if (dlg.ShowDialog() == true)
            {
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)image2.ImageSource));
                using (var fs = System.IO.File.OpenWrite(dlg.FileName))
                {
                    pngEncoder.Save(fs);
                    fs.Dispose();
                    fs.Close();
                }
            }

        }
    }

}
