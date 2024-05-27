using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Controls
{
    public class ScreenCaptureExt : Window
    {
        const int WM_USER = 0x03FC;
        const int MY_MESSAGE = WM_USER + 1;
        /// <summary>
        /// 截图完成委托
        /// </summary>
        public delegate void ScreenShotDone(BitmapSource bitmap);
        /// <summary>
        /// 截图完成事件
        /// </summary>
        public event ScreenShotDone SnapCompleted;
        /// <summary>
        /// 截图取消委托
        /// </summary>
        public delegate void ScreenShotCanceled();
        /// <summary>
        /// 截图取消事件
        /// </summary>
        public event ScreenShotCanceled SnapCanceled;

        public ScreenCaptureExt()
        {
            Width = 0;
            Height = 0;
            Left = int.MinValue;
            Top = int.MinValue;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            ShowActivated = false;
            Title = "ScreenShot";
            Show();
            ShowScreenShot();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null) hwndSource.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_USER:
                    GetClipboard();
                    Close();
                    break;
                case MY_MESSAGE:
                    if (SnapCanceled != null) 
                        SnapCanceled();
                    Close();
                    break;
            }
            return IntPtr.Zero;
        }

        void GetClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                var dataObject = Clipboard.GetDataObject();
                if (dataObject == null) return;

                using (var memoryStream = new MemoryStream())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)dataObject.GetData(DataFormats.Bitmap)));
                    encoder.Save(memoryStream);
                    memoryStream.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    if (SnapCompleted != null)
                        SnapCompleted(bitmapImage);
                }
            }
        }

        void ShowScreenShot()
        {
            if (Helper.GetTempPathVersionExt != null && File.Exists(Helper.GetTempPathVersionExt))
            {
                var process = new Process();
                process.StartInfo.FileName = Helper.GetTempPathVersionExt;
                process.StartInfo.Arguments = Title;
                process.Start();
            }
        }
    }
}
