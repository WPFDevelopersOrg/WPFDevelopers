using System;
using System.Windows;
using System.Windows.Interop;

namespace WPFDevelopers.Controls
{
    public class AudioWindow : Window
    {
        public delegate void StopDelegate();

        private const int MM_MCINOTIFY = 0x3B9;
        public event StopDelegate StopDelegateEvent;

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
                case MM_MCINOTIFY:
                    StopDelegateEvent?.Invoke();
                    break;
            }

            return IntPtr.Zero;
        }
    }
}