using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using WPFDevelopers.Controls;
using WPFDevelopers.Utilities;
using Size = System.Drawing.Size;

namespace WPFDevelopers.Helpers
{
    public static class WindowHelpers
    {
        public static void MaskShow(this Window outputWindow)
        {
            CreateMask(outputWindow);
            outputWindow.Show();
        }

        public static bool? MaskShowDialog(this Window outputWindow)
        {
            CreateMask(outputWindow);
            return outputWindow.ShowDialog();
        }

        public static void CreateMask(this Window outputWindow)
        {
            Visual parent = ControlsHelper.GetDefaultWindow();
            var _layer = ControlsHelper.GetAdornerLayer(parent);
            if (_layer == null) return;
            var _adornerContainer = new AdornerContainer(_layer)
            {
                Child = new MaskControl(parent)
            };
            _layer.Add(_adornerContainer);
            if (outputWindow != null)
            {
                outputWindow.Closed += delegate
                {
                    if (parent != null)
                        _layer.Remove(_adornerContainer);
                };
            }
        }

        public static void SetIconicThumbnail(this Window window, string imagePath)
        {
            if (window == null || string.IsNullOrWhiteSpace(imagePath)) return;
            if (!File.Exists(imagePath)) return;
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            int size = Marshal.SizeOf(typeof(int));
            IntPtr pBool = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.WriteInt32(pBool, 1);
                Win32.DwmSetWindowAttribute(hwnd, DwmWindowAttributes.FORCE_ICONIC_REPRESENTATION, pBool, size);
                Win32.DwmSetWindowAttribute(hwnd, DwmWindowAttributes.HAS_ICONIC_BITMAP, pBool, size);
            }
            finally
            {
                Marshal.FreeHGlobal(pBool);
            }
            var source = HwndSource.FromHwnd(hwnd);
            if (source != null)
            {
                source.AddHook(new HwndSourceHook((IntPtr hwnd2, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                {
                    if (msg == WindowsMessageCodes.WM_DWMSENDICONICTHUMBNAIL)
                    {
                        int width = ((int)((((long)lParam) >> 16) & 0xFFFF));
                        int height = ((int)((long)lParam & 0xFFFF));
                        try
                        {
                            using (var bmp = new Bitmap(imagePath))
                            using (var resized = new Bitmap(bmp, new Size(width, height)))
                            {
                                IntPtr hBitmap = resized.GetHbitmap();
                                Win32.DwmSetIconicThumbnail(hwnd2, hBitmap, (int)DwmWindowAttributes.None);
                                Win32.DeleteObject(hBitmap);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"DwmSetIconicThumbnail error :{ex.Message}!");
                        }

                        handled = true;
                    }

                    return IntPtr.Zero;
                }));
            }
            Win32.DwmInvalidateIconicBitmaps(hwnd);
            window.ShowInTaskbar = true;
        }

        public static Rect GetWindowBounds(IntPtr hWnd)
        {
            Win32.RECT rect;
            Win32.GetWindowRect(hWnd, out rect);
            int dpi = Win32.GetDpiForWindow(hWnd);
            double scale = dpi / 96.0;
            return new Rect(
                rect.Left / scale,
                rect.Top / scale,
                (rect.Right - rect.Left) / scale,
                (rect.Bottom - rect.Top) / scale);
        }
    }
}
