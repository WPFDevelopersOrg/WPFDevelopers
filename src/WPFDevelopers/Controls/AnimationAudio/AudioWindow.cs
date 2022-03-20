using System;
using System.Windows;
using System.Windows.Interop;

namespace WPFDevelopers.Controls
{
    public class AudioWindow:Window
    {
        const int MM_MCINOTIFY = 0x3B9;
        public delegate void StopDelegate();
        public event StopDelegate StopDelegateEvent;
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(this.WndProc));
            }
        }
        
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case MM_MCINOTIFY:
                    StopDelegateEvent?.Invoke();
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
