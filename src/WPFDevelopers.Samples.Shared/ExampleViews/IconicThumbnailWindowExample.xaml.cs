using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Sample.ExampleViews
{
    /// <summary>
    /// IconicThumbnailWindowExample.xaml 的交互逻辑
    /// </summary>
    public partial class IconicThumbnailWindowExample
    {
        private List<string> fileList = new List<string>(); 
        private int currentFileIndex = -1;
        public IconicThumbnailWindowExample()
        {
            InitializeComponent();
            Loaded += IconicThumbnailWindowExample_Loaded;
        }

        private void IconicThumbnailWindowExample_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.Clear();
            currentFileIndex = -1;
            var directorys = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IconicThumbnail");
            if (!Directory.Exists(directorys)) return;
            string[] files = Directory.GetFiles(directorys);
            fileList.AddRange(files);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (fileList.Count == 0) return;
            currentFileIndex = (currentFileIndex + 1) % fileList.Count;
            var img = fileList[currentFileIndex];
            ImagePreview.Source = new BitmapImage(new Uri(img));
            this.SetIconicThumbnail(img);
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (fileList.Count == 0) return;
            currentFileIndex = (currentFileIndex - 1 + fileList.Count) % fileList.Count;
            var img = fileList[currentFileIndex];
            ImagePreview.Source = new BitmapImage(new Uri(img));
            this.SetIconicThumbnail(img);
        }
    }
}
