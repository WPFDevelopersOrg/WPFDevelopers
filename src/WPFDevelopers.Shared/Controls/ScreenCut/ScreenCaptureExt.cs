using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class ScreenCaptureExt : Window
    {
        private ThemeType? _theme;
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
        /// <summary>
        /// 获取保存的图片全路径
        /// </summary>
        public event Action<string> SnapSaveFullPath;

        public ScreenCaptureExt(ThemeType? themeType = null)
        {
            if(themeType == null)
            {
                var existingResourceDictionary =
               (Resources)Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x is Resources);
                if (existingResourceDictionary != null)
                    themeType = existingResourceDictionary.Theme;
                else
                    themeType = ThemeType.Dark;
            }
            _theme = themeType;
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
                case Helper.WM_USER:
                    Close();
                    GetClipboard();
                    break;
                case Helper.MY_MESSAGE:
                    Close();
                    if (SnapCanceled != null) 
                        SnapCanceled();
                    break;
                case Helper.MY_MESSAGEFULLPATH:
                    Close();
                    GetClipboard();
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
            else if (Clipboard.ContainsText()) 
            {
                var clipboardText = Clipboard.GetText();
                if (SnapSaveFullPath != null)
                    SnapSaveFullPath(clipboardText);
            }
        }

        void ShowScreenShot()
        {
            string[] args = { Title, _theme.ToString() };//1.窗体tilte 2.Light或Dark
            if (Helper.GetTempPathVersionExt != null && File.Exists(Helper.GetTempPathVersionExt))
            {
                var process = new Process();
                process.StartInfo.FileName = Helper.GetTempPathVersionExt;
                process.StartInfo.Arguments = string.Join(" ", args);
                process.Start();
            }
        }
    }
}
