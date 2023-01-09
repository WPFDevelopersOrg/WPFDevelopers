using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Controls.ScreenCapturer
{
    public class ScreenCapture

    {
        /// <summary>
        /// 截图完成委托
        /// </summary>
        public delegate void ScreenShotDone(CroppedBitmap bitmap);
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
        /// 是否将截图结果复制
        /// 默认复制
        /// </summary>
        private bool copyToClipboard;
        List<ScreenCut> ScreenCuts = new List<ScreenCut>();
        public ScreenCapture(bool copyToClipboard = true)
        {
            this.copyToClipboard = copyToClipboard;
            for (var i = 0; i < Screen.AllScreens.Length; i++)
            {
                ScreenCuts.Add(CaptureScreen(i));
            }
        }
        private ScreenCut CaptureScreen(int index)
        {
            ScreenCut screenCut = new ScreenCut(index);
            screenCut.CutCompleted += ScreenCut_CutCompleted;
            screenCut.CutCanceled += ScreenCut_CutCanceled;
            screenCut.Closed += ScreenCut_Closed;
            return screenCut;
        }

        private void ScreenCut_CutCanceled()
        {
            if (SnapCanceled != null) SnapCanceled();
        }

        public void Capture()
        {
            foreach (var screenCut in ScreenCuts)
            {
                screenCut.Show();
                screenCut.Activate();
            }
        }

        private void ScreenCut_Closed(object sender, System.EventArgs e)
        {
            if (ScreenCuts.Contains((ScreenCut)sender))
            {
                ScreenCuts.Remove((ScreenCut)sender);
            }
            CloseCutters();
            ScreenCut.ClearCaptureScreenID();
        }
        private void CloseCutters()
        {
            if (ScreenCuts.Count == 0) return;
            while (ScreenCuts.Count > 0)
            {
                ScreenCuts[0].Close();
            }
            ScreenCuts.Clear();
        }
        private void ScreenCut_CutCompleted(CroppedBitmap bitmap)
        {
            if (SnapCompleted != null)
                SnapCompleted(bitmap);
            if (copyToClipboard)
                System.Windows.Clipboard.SetImage(bitmap);
        }
    }
}