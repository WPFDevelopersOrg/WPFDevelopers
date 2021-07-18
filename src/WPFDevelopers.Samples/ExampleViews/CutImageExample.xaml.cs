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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CutImageExample.xaml 的交互逻辑
    /// </summary>
    public partial class CutImageExample : UserControl
    {
        public CutImageExample()
        {
            InitializeComponent();
            this.btnImport.Click += OnImportClickHandler;
            this.btnSave.Click += BtnSave_Click;
        }
        private void OnImportClickHandler(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "图像文件(*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                cutCustoms.ImageSource = new BitmapImage(new Uri(openFileDialog.FileName));
                btnSave.Visibility = Visibility.Visible;
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
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
