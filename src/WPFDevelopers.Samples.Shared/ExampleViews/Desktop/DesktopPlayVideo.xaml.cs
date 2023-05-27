using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFDevelopers.Helpers;


namespace WPFDevelopers.Samples.ExampleViews.Desktop
{
    /// <summary>
    /// DesktopPlayVideo.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopPlayVideo : Window
    {
        private IntPtr programHandle;
        public DesktopPlayVideo()
        {
            InitializeComponent();
            this.Loaded += DesktopPlayVideo_Loaded;
        }

        private void DesktopPlayVideo_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".mp4";
            openFileDialog.Filter = "视频文件(.MP4)|*.mp4;";
            if (openFileDialog.ShowDialog() == true)
            {
                SendMsgToProgman();
                Width = SystemParameters.PrimaryScreenWidth; Height = SystemParameters.PrimaryScreenHeight; Left = 0; Top = 0;

                //PART_MediaElement.Source = new Uri(openFileDialog.FileName);
                //PART_MediaElement.MediaEnded += (s1, e1) => 
                //{
                //    PART_MediaElement.Position = new TimeSpan(0, 0, 1);
                //    PART_MediaElement.Play();
                //};

                var storyboard = new Storyboard();
                storyboard.RepeatBehavior = RepeatBehavior.Forever;
                var mediaTimeline = new MediaTimeline
                {
                    Source = new Uri(openFileDialog.FileName),
                };
                Storyboard.SetTargetName(mediaTimeline, PART_MediaElement.Name);
                storyboard.Children.Add(mediaTimeline);

                // 设置当前窗口为 Program Manager的子窗口
                Win32.SetParent(new WindowInteropHelper(this).Handle, programHandle);
                PART_MediaElement.Loaded += (s1, e1) =>
                {
                    storyboard.Begin(PART_MediaElement);
                };
                App.CurrentMainWindow.WindowState = WindowState.Minimized;
            }
        }

        /// <summary>
        /// 向桌面发送消息
        /// </summary>
        void SendMsgToProgman()
        {
            // 桌面窗口句柄，在外部定义，用于后面将我们自己的窗口作为子窗口放入
            programHandle = Win32.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;
            // 向 Program Manager 窗口发送消息 0x52c 的一个消息，超时设置为2秒
            Win32.SendMessageTimeout(programHandle, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 2, result);

            // 遍历顶级窗口
            Win32.EnumWindows((hwnd, lParam) =>
            {
                // 找到第一个 WorkerW 窗口，此窗口中有子窗口 SHELLDLL_DefView，所以先找子窗口
                if (Win32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                {
                    // 找到当前第一个 WorkerW 窗口的，后一个窗口，及第二个 WorkerW 窗口。
                    IntPtr tempHwnd = Win32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

                    // 隐藏第二个 WorkerW 窗口
                    Win32.ShowWindow(tempHwnd, 0);
                }
                return true;
            }, IntPtr.Zero);
        }
    }
}
