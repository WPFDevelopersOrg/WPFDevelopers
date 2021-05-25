using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageSizeInfo imageSizeInfo = new ImageSizeInfo();
        private double moveMinY;
        private Bitmap bitmap = null;
        public MainWindow()
        {
            InitializeComponent();
            imageSizeInfo.Width = 120;
            imageSizeInfo.Height = 120;
            this.dragDropItem.UpdateImageEvent += DragDropItem_UpdateImageEvent;
            this.btnImport.Click += OnImportClickHandler;
            this.btnSave.Click += BtnSave_Click;    
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "123.jpg"; 
            dlg.DefaultExt = ".jpg"; 
            dlg.Filter = "image file|*.jpg"; 

            if (dlg.ShowDialog() == true)
            {
                bitmap.Save(dlg.FileName);
            }
           
        }

        private void DragDropItem_UpdateImageEvent()
        {
            Bitmap sourceBitmap = null;
            Bitmap targetBitmap = null;
            
            sourceBitmap = this.BitmapImageToBitmap(this.imageItem.Source as BitmapImage);
            if (sourceBitmap != null)
            {
                System.Drawing.Rectangle rectangle = this.dragDropItem.GetCutRectangle();

                targetBitmap = new Bitmap((int)this.imageItem.ActualWidth, (int)this.imageItem.ActualHeight);
                using (Graphics graphics = Graphics.FromImage(targetBitmap))
                {
                    graphics.DrawImage(sourceBitmap, new System.Drawing.Rectangle(0, (int)Canvas.GetTop(this.imageItem), targetBitmap.Width, targetBitmap.Height));
                }

                bitmap = new Bitmap(rectangle.Width, rectangle.Height);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(targetBitmap, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, System.Drawing.GraphicsUnit.Pixel);
                }
                image2.ImageSource = GetBitmapSource(bitmap);
            }
        }

        private void OnImportClickHandler(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "图像文件(*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                this.LoadImage(openFileDialog.FileName);
            }
        }
        private void OnImageCanvasMouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            if (this.imageItem.Source == null) return;

            double canvasWidth = this.imageCanvas.Width + e.Delta;
            double canvasHeight = this.imageCanvas.Height + e.Delta;
            if (canvasWidth == 0) return;


            if (canvasWidth <= 0) canvasWidth = 0;
            if (canvasWidth > this.containerPanel.Width)
            {
                this.containerPanel.Width = canvasWidth;
            }

            if (canvasHeight <= 0) canvasHeight = 0;
            if (canvasHeight > this.containerPanel.Height) canvasHeight = this.containerPanel.Height;
            this.imageCanvas.Width = canvasWidth;
            this.imageCanvas.Height += canvasHeight;

            this.imageItem.Width = canvasWidth;

            // 加载完成之后
            this.imageItem.UpdateLayout();

            this.SetAreaInfo(false);
        }
        private void LoadImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            if (this.imageSizeInfo == null) return;
            this.imageItem.Width = this.imageSizeInfo.Width;
            this.imageItem.Source = this.GetBitmapImage(imagePath);

            // 加载完成之后
            this.imageItem.UpdateLayout();
            this.imageCanvas.Width = this.imageSizeInfo.Width;
            this.imageCanvas.Height = this.imageItem.ActualHeight;

            this.SetAreaInfo(true);
            DragDropItem_UpdateImageEvent();
        }

        #region 转换Bitmap到BitmapSource
        /// <summary>
        /// 转换Bitmap到BitmapSource
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapSource GetBitmapSource(System.Drawing.Bitmap bmp)
        {
            System.Windows.Media.Imaging.BitmapFrame bf = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bf = System.Windows.Media.Imaging.BitmapFrame.Create(ms, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);

            }
            return bf;
        }
        #endregion
        private BitmapImage GetBitmapImage(string imagePath)
        {
            BitmapImage bitmapImage = new BitmapImage();
            if (File.Exists(imagePath))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                using (Stream stream = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
            }
            return bitmapImage;
        }
        private Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        private void SetAreaInfo(bool initStatus)
        {
            double sub = (this.containerPanel.Height - this.imageCanvas.Height) * 0.5;
            if (sub < 0) sub = 0;

            this.imageCanvas.Margin = new Thickness(0, sub, 0, 0);

            if (this.dragDropItem != null)
            {
                double maxHeight = Math.Min(this.imageCanvas.Height, this.imageSizeInfo.Height);
                if (initStatus)
                {
                    this.dragDropItem.ParentMaxWidth = this.imageSizeInfo.Width;
                }
                else
                {
                    this.dragDropItem.ParentMaxWidth = this.imageCanvas.Width;
                }
                if (this.imageCanvas.Height <= this.containerPanel.Height)
                {
                    this.dragDropItem.ParentMaxHeight = this.imageCanvas.Height;
                }
                else
                {
                    this.dragDropItem.ParentMaxHeight = this.containerPanel.Height;
                }
                this.dragDropItem.Init(0, 0, this.imageSizeInfo.Width, (int)maxHeight);
            }
            this.moveMinY = this.dragDropItem.ParentMaxHeight - this.imageItem.ActualHeight;
            Canvas.SetTop(this.imageItem, 0);
        }
    }
}


