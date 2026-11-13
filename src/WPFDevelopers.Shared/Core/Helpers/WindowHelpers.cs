using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
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
            var mask = AddMask(outputWindow);
            try { outputWindow.Show(); }
            catch
            {
                mask.Dispose();
                throw;
            }
        }

        public static bool? MaskShowDialog(this Window outputWindow)
        {
            using (AddMask(outputWindow))
                return outputWindow.ShowDialog();
        }

        public static void CreateMask(this Window outputWindow)
        {
            AddMask(outputWindow);
        }

        private static MaskSession AddMask(Window outputWindow)
        {
            if (outputWindow == null)
                throw new ArgumentNullException(nameof(outputWindow));
            var owner = outputWindow.Owner ?? ControlsHelper.GetDefaultWindow();
            if (owner == null || owner == outputWindow || !owner.IsVisible)
                throw new InvalidOperationException("A mask requires a visible owner Window.");
            var source = owner.Content as FrameworkElement;
            var layer = source == null ? null : AdornerLayer.GetAdornerLayer(source);
            if (layer == null)
                throw new InvalidOperationException("The owner content requires an AdornerLayer.");
            var adorner = new MaskAdorner(source) { Child = new MaskControl(owner) };
            var session = new MaskSession(outputWindow, owner, layer, adorner);
            try { layer.Add(adorner); }
            catch
            {
                session.Dispose();
                throw;
            }
            return session;
        }

        private sealed class MaskAdorner : AdornerContainer
        {
            internal MaskAdorner(UIElement source) : base(source) { }

            protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
            {
                var size = AdornedElement.RenderSize;
                Child.Measure(size);
                return size;
            }
        }

        private sealed class MaskSession : IDisposable
        {
            private readonly Window _window;
            private readonly Window _owner;
            private readonly AdornerLayer _layer;
            private readonly MaskAdorner _adorner;
            private bool _disposed;

            internal MaskSession(Window window, Window owner, AdornerLayer layer, MaskAdorner adorner)
            {
                _window = window;
                _owner = owner;
                _layer = layer;
                _adorner = adorner;
                window.Closed += OnClosed;
                owner.Closed += OnClosed;
            }

            private void OnClosed(object sender, EventArgs e)
            {
                Dispose();
            }

            public void Dispose()
            {
                if (_disposed)
                    return;
                _disposed = true;
                _window.Closed -= OnClosed;
                _owner.Closed -= OnClosed;
                _layer.Remove(_adorner);
                _adorner.Child = null;
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
                    if (msg == WindowsMessageCodes.WM_DWMSENDICONICLIVEPREVIEWBITMAP)
                    {
                        try
                        {
                            using (var bmp = new Bitmap(imagePath))
                            {
                                IntPtr hBitmap = bmp.GetHbitmap();
                                Win32.DwmSetIconicLivePreviewBitmap(hwnd2, hBitmap, IntPtr.Zero, 0);
                                Win32.DeleteObject(hBitmap);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"LivePreview error: {ex.Message}");
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
