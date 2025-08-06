using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace WPFDevelopers.Helpers
{
    public static class Win32
    {
        public const string
            User32 = "user32.dll",
            Gdi32 = "gdi32.dll",
            GdiPlus = "gdiplus.dll",
            Kernel32 = "kernel32.dll",
            Shell32 = "shell32.dll",
            MsImg = "msimg32.dll",
            NTdll = "ntdll.dll",
            DwmApi = "dwmapi.dll",
            Winmm = "winmm.dll",
            Shcore = "shcore.dll";
        //查找窗口的委托 查找逻辑
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport(User32)]
        public static extern IntPtr FindWindow(string className, string winName);

        [DllImport(User32)]
        public static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam,
            uint fuFlage, uint timeout, IntPtr result);

        [DllImport(User32)]
        internal static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32)]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport(User32)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className,
            string winName);

        [DllImport(User32)]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport(User32)]
        public static extern IntPtr SetParent(IntPtr hwnd, IntPtr parentHwnd);

        [DllImport(User32)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(User32)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(User32)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(Winmm)]
        public static extern long mciSendString(string strCommand, StringBuilder strReturn,
            int iReturnLength, IntPtr hwndCallback);

        #region WINAPI DLL Imports

        [DllImport(Gdi32, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport(Gdi32)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(Gdi32)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(Gdi32)]
        public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel,
            IntPtr lpvBits);

        [DllImport(User32)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(Gdi32)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport(User32)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);


        [DllImport(Gdi32, EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDc);


        public const int SM_CXSCREEN = 0;

        public const int SM_CYSCREEN = 1;

        [DllImport(User32, EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(User32, EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        [DllImport(User32, EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(int ptr);

        public struct DeskTopSize
        {
            public int cx;
            public int cy;
        }

        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,

            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,

            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,

            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,

            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,

            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,

            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,

            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,

            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,

            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,

            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,

            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,

            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,

            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,

            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062
        }

        [DllImport(Gdi32)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
            int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        #endregion

        /// <summary>   
        /// 设置鼠标的坐标   
        /// </summary>   
        /// <param name="x">横坐标</param>   
        /// <param name="y">纵坐标</param>   
        [DllImport(User32)]
        public extern static void SetCursorPos(int x, int y);

        [DllImport(DwmApi)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttributes attr, ref int attrValue, int attrSize);

        public static bool EnableDarkModeForWindow(HwndSource source, bool enable)
        {
            int darkMode = enable ? 1 : 0;
            int hr = DwmSetWindowAttribute(source.Handle, DwmWindowAttributes.UseImmersiveDarkMode, ref darkMode, sizeof(int));
            return hr >= 0;
        }

        [DllImport(DwmApi)]
        public static extern int DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbmp, int dwSITFlags);

        [DllImport(DwmApi)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttributes dwAttribute, IntPtr pvAttribute, int cbAttribute);

        [DllImport(DwmApi)]
        public static extern int DwmInvalidateIconicBitmaps(IntPtr hwnd);

        [DllImport(User32)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(User32)]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }

    internal class WindowsMessageCodes
    {
        public const int SC_RESTORE = 0xF120;
        public const int SC_MINIMIZE = 0xF020;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_DEADCHAR = 0x0024;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCHITTEST = 0x0084;

        public const int WM_DWMSENDICONICTHUMBNAIL = 0x0323;
    }

    [Flags]
    public enum DwmWindowAttributes : uint
    {
        None = 0, 
        DISPLAYFRAME = 1,
        FORCE_ICONIC_REPRESENTATION = 7,
        HAS_ICONIC_BITMAP = 10,
        UseImmersiveDarkMode = 20
    }

}