using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace WPFDevelopers.Helpers
{
    internal static class WindowStartupHelper
    {
        private const uint MonitorDefaultToNearest = 2;
        private const uint SwpNoSize = 0x0001;
        private const uint SwpNoZOrder = 0x0004;
        private const uint SwpNoActivate = 0x0010;
        private const int DwmwaCloak = 13;

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonitorInfo
        {
            public int Size;
            public NativeRect Monitor;
            public NativeRect WorkArea;
            public uint Flags;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetCursorPos(out NativePoint point);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(NativePoint point, uint flags);

        [DllImport("user32.dll", EntryPoint = "GetMonitorInfo", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr window, out NativeRect rect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr window, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmSetWindowAttribute(IntPtr window, int attribute, ref int value, int size);

        internal static void CenterOnCursorMonitor(Window window)
        {
            NativePoint cursorPosition;
            if (!GetCursorPos(out cursorPosition))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var monitor = MonitorFromPoint(cursorPosition, MonitorDefaultToNearest);
            var monitorInfo = new MonitorInfo { Size = Marshal.SizeOf(typeof(MonitorInfo)) };
            if (monitor == IntPtr.Zero || !GetMonitorInfo(monitor, ref monitorInfo))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var originalOpacity = window.Opacity;
            bool useCloak = Environment.OSVersion.Version >= new Version(6, 2);
            EventHandler sourceInitialized = null;
            if (useCloak)
            {
                sourceInitialized = delegate
                {
                    window.SourceInitialized -= sourceInitialized;
                    int cloak = 1;
                    DwmSetWindowAttribute(new WindowInteropHelper(window).Handle, DwmwaCloak, ref cloak, sizeof(int));
                };
                window.SourceInitialized += sourceInitialized;
            }
            else
            {
                window.Opacity = 0;
            }
            bool completed = false;
            EventHandler contentRendered = null;
            RoutedEventHandler loaded = null;
            Action completeStartup = delegate
            {
                if (completed)
                    return;
                completed = true;
                window.ContentRendered -= contentRendered;
                window.Loaded -= loaded;
                try
                {
                    var handle = new WindowInteropHelper(window).Handle;
                    NativeRect windowRect;
                    if (!GetWindowRect(handle, out windowRect))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    var width = windowRect.Right - windowRect.Left;
                    var height = windowRect.Bottom - windowRect.Top;
                    if (width <= 0 || height <= 0)
                        throw new InvalidOperationException("The window has no size after its content was rendered.");

                    var x = monitorInfo.WorkArea.Left + ((monitorInfo.WorkArea.Right - monitorInfo.WorkArea.Left) - width) / 2;
                    var y = monitorInfo.WorkArea.Top + ((monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top) - height) / 2;
                    if (!SetWindowPos(handle, IntPtr.Zero, x, y, 0, 0,
                            SwpNoSize | SwpNoZOrder | SwpNoActivate))
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                finally
                {
                    if (useCloak)
                    {
                        int cloak = 0;
                        DwmSetWindowAttribute(new WindowInteropHelper(window).Handle, DwmwaCloak, ref cloak, sizeof(int));
                    }
                    else
                    {
                        window.Opacity = originalOpacity;
                    }
                }
            };
            contentRendered = delegate { completeStartup(); };
            loaded = delegate
            {
                if (window.Content == null)
                    window.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, completeStartup);
            };
            window.ContentRendered += contentRendered;
            window.Loaded += loaded;
        }
    }
}
