using Standard;
using System;
using System.Runtime.InteropServices;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Core.Helpers
{
    public static class MonitorHelper
    {
        internal class WindowsMessageCodes
        {
            public const int SC_RESTORE = 0xF120;
            public const int SC_MINIMIZE = 0xF020;
            public const int WM_SYSCOMMAND = 0x0112;
            public const int WM_DEADCHAR = 0x0024;
        }

        [DllImport(Win32.User32)]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport(Win32.User32)]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport(Win32.User32)]
        internal static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

    }
}
