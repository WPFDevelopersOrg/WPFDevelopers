using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class CropImageExample : UserControl
    {
        public CropImageExample()
        {
            InitializeComponent();
        }
        double ConvertBytesToMB(long bytes)
        {
            return (double)bytes / (1024 * 1024);
        }
        private void OnImportClickHandler(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件(*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                var fileSize = fileInfo.Length;
                var mb = ConvertBytesToMB(fileSize);
                if (mb > 1)
                {
                    WPFDevelopers.Controls.MessageBox.Show("图片不能大于 1M ", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                bitmap.EndInit();

                if (bitmap.PixelWidth > 500 || bitmap.PixelHeight > 500)
                {
                    var width = (int)(bitmap.PixelWidth * 0.5);
                    var height = (int)(bitmap.PixelHeight * 0.5);
                    var croppedBitmap = new CroppedBitmap(bitmap, new Int32Rect(0, 0, width, height));
                    var bitmapNew = new BitmapImage();
                    bitmapNew.BeginInit();
                    bitmapNew.DecodePixelWidth = width;
                    bitmapNew.DecodePixelHeight = height;
                    var memoryStream = new MemoryStream();
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap.Source));
                    encoder.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    bitmapNew.StreamSource = memoryStream;
                    bitmapNew.EndInit();
                    MyCropImage.Source = bitmapNew;
                }
                else
                {
                    MyCropImage.Source = bitmap;
                }
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = $"WPFDevelopers_CropImage_{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "image file|*.jpg";
            if (dlg.ShowDialog() == true)
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)MyCropImage.CurrentAreaBitmap));
                using (var fs = File.OpenWrite(dlg.FileName))
                {
                    pngEncoder.Save(fs);
                    fs.Dispose();
                    fs.Close();
                }
            }

        }
    }
}
