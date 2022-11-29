using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// NineGridViewExample.xaml 的交互逻辑
    /// </summary>
    public partial class NineGridViewExample : UserControl
    {
        public NineGridViewExample()
        {
            InitializeComponent();
        }

        private void MyMediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            var path = "E:\\DCLI6K5UIAEmH9R.mp4";
            if (File.Exists(path))
                MyMediaElement.Source = new Uri(path);
        }

        private void MyMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            MyMediaElement.Position = new TimeSpan(0);
        }
        
    }
}
