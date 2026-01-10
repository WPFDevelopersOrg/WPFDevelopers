using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ScreenCutExample.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenCutExample : UserControl
    {
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ScreenCutExample), new PropertyMetadata(false));


        public ScreenCutExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsChecked)
            {
                App.CurrentMainWindow.WindowState = WindowState.Minimized;
            }
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(350); 
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ScreenCapture screenCapturer = new ScreenCapture();
                    screenCapturer.SnapCompleted += ScreenCapturer_SnapCompleted;
                    screenCapturer.SnapCanceled += ScreenCapturer_SnapCanceled;
                    screenCapturer.SnapSaveFullPath += ScreenCapturer_SnapSaveFullPath;
                    screenCapturer.Capture();
                }));
            });
        }

        private void ScreenCapturer_SnapSaveFullPath(string text)
        {
            WPFDevelopers.Controls.MessageBox.Show($"截图路径：{text}","Info",MessageBoxImage.Information);
        }

        private void ScreenCapturer_SnapCanceled()
        {
            App.CurrentMainWindow.WindowState = WindowState.Normal;
            Message.Push($"{DateTime.Now} 取消截图", MessageBoxImage.Information);
        }

        private void ScreenCapturer_SnapCompleted(System.Windows.Media.Imaging.CroppedBitmap bitmap)
        {
            myImage.Source = bitmap;
            App.CurrentMainWindow.WindowState = WindowState.Normal;
        }
        private void ButtonExt_Click(object sender, RoutedEventArgs e)
        {
            if (IsChecked)
            {
                App.CurrentMainWindow.WindowState = WindowState.Minimized;
            }

            var screenCaptureExt = new ScreenCaptureExt();
            screenCaptureExt.SnapCanceled += ScreenCaptureExt_SnapCanceled;
            screenCaptureExt.SnapCompleted += ScreenCaptureExt_SnapCompleted;
            screenCaptureExt.SnapSaveFullPath += ScreenCaptureExt_SnapSaveFullPath;
        }

        private void ScreenCaptureExt_SnapSaveFullPath(string text)
        {
            Message.Push($"截图路径：{text}", MessageBoxImage.Information);
        }

        private void ScreenCaptureExt_SnapCompleted(System.Windows.Media.Imaging.BitmapSource bitmap)
        {
            myImage.Source = bitmap;
            App.CurrentMainWindow.WindowState = WindowState.Normal;
        }

        private void ScreenCaptureExt_SnapCanceled()
        {
            try
            {
                if (App.CurrentMainWindow.WindowState == WindowState.Minimized)
                    App.CurrentMainWindow.WindowState = WindowState.Normal;
                Message.Push($"{DateTime.Now} 取消截图", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                WPFDevelopers.Controls.MessageBox.Show(ex.Message);
            }
        }
    }
}