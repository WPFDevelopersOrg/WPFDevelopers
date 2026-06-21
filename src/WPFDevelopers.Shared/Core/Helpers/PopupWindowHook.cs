using System;
using System.Windows;
using System.Windows.Interop;

namespace WPFDevelopers.Helpers
{
    public sealed class PopupWindowHook : IDisposable
    {
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_ACTIVATEAPP = 0x001C;

        private readonly Action _onClose;
        private readonly bool _handleActivateApp;
        private readonly Window _window;
        private HwndSource _hwndSource;

        public PopupWindowHook(Window window, Action onClose, bool handleActivateApp = true)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _onClose = onClose ?? throw new ArgumentNullException(nameof(onClose));
            _handleActivateApp = handleActivateApp;

            if (_window.IsInitialized)
                Hook();
            else
                _window.SourceInitialized += OnSourceInitialized;
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            _window.SourceInitialized -= OnSourceInitialized;
            Hook();
        }

        private void Hook()
        {
            _hwndSource = PresentationSource.FromVisual(_window) as HwndSource;
            if (_hwndSource != null)
                _hwndSource.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCLBUTTONDOWN)
                _onClose();
            else if (_handleActivateApp && msg == WM_ACTIVATEAPP && wParam == IntPtr.Zero)
                _onClose();
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(WndProc);
                _hwndSource = null;
            }
            _window.SourceInitialized -= OnSourceInitialized;
        }
    }
}
