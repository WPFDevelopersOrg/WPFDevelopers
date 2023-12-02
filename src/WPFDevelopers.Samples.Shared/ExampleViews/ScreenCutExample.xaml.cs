using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;
using WPFDevelopers.Controls.ScreenCapturer;

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

            Dispatcher.Invoke(new Action(delegate
            {
                ScreenCapture screenCapturer;
                if (IsChecked)
                {
                    App.CurrentMainWindow.WindowState = WindowState.Minimized;
                    //Thread.Sleep(1000);
                }

                #region 无App.xaml情况，需引入资源文件
                //如没有App.xaml文件需要使用截图控件需要按照以下方式传入资源文件，具体查看
                //https://github.com/WPFDevelopersOrg/WPFDevelopers/issues/68
                //ResourceDictionary resources = new ResourceDictionary();
                //resources.Source = new Uri("pack://application:,,,/MyResources.xaml");
                //screenCapturer = new ScreenCapture(resources: resources);

                #endregion
                screenCapturer = new ScreenCapture();
                screenCapturer.SnapCompleted += ScreenCapturer_SnapCompleted;
                screenCapturer.SnapCanceled += ScreenCapturer_SnapCanceled;
                screenCapturer.Capture();
            }));

        }

        private void ScreenCapturer_SnapCanceled()
        {
            App.CurrentMainWindow.WindowState = WindowState.Normal;
        }

        private void ScreenCapturer_SnapCompleted(System.Windows.Media.Imaging.CroppedBitmap bitmap)
        {
            App.CurrentMainWindow.WindowState = WindowState.Normal;
        }
    }
}