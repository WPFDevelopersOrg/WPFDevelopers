using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class Toast
    {
        private static readonly ConcurrentDictionary<Window, ToastAdorner> _windowAdorners = new ConcurrentDictionary<Window, ToastAdorner>();
        private static ToastExt _toastExt;
        private static Position _position = Position.Top;
        private static bool _isExitHandlerRegistered = false;

        static Toast()
        {
            RegisterAppExitHandler();
        }

        private static void RegisterAppExitHandler()
        {
            if (_isExitHandlerRegistered)
                return;

            try
            {
                if (Application.Current != null)
                {
                    Application.Current.Exit += (s, e) => CleanupAll();
                    _isExitHandlerRegistered = true;
                }
                else if (System.Windows.Forms.Application.MessageLoop)
                {
                    System.Windows.Forms.Application.ApplicationExit += (s, e) => CleanupAll();
                    _isExitHandlerRegistered = true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"RegisterAppExitHandler failed: {ex.Message}");
#endif
            }
        }

        static void CreateToastAdorner(Window owner = null, string message = null, ToastImage type = ToastImage.Info, bool center = false)
        {
            try
            {
                if (owner == null)
                    owner = ControlsHelper.GetDefaultWindow();
                ToastAdorner ToastAdorner = null;
                if (!_windowAdorners.TryGetValue(owner, out ToastAdorner))
                {
                    var layer = ControlsHelper.GetAdornerLayer(owner);
                    if (layer == null)
                        throw new Exception("AdornerLayer is not found, it is recommended to use PushDesktop");
                    ToastAdorner = new ToastAdorner(layer);
                    layer.Add(ToastAdorner);
                    owner.Closed -= OnOwner_Closed;
                    owner.Closed += OnOwner_Closed;
                    _windowAdorners[owner] = ToastAdorner;
                }
                if (ToastAdorner.Position != _position)
                    ToastAdorner.Position = _position;

                if (!string.IsNullOrWhiteSpace(message))
                    ToastAdorner.Push(message, type, center);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"CreateToastAdorner failed: {ex.Message}");
#endif
            }
        }

        private static void OnOwner_Closed(object sender, EventArgs e)
        {
            var owner = sender as Window;
            if (owner != null)
            {
                ToastAdorner adorner;
                if (_windowAdorners.TryRemove(owner, out adorner))
                {
                    adorner.Clear();
                }
            }
            var normalWindowCount = Application.Current.Windows.Cast<Window>().Count(w => w != null && !(w is ToastExt));
            if (normalWindowCount <= 1)
            {
                CleanupAll();
            }
        }

        public static void Push(this Window owner, string message, ToastImage type = ToastImage.Info, bool center = false)
        {
            CreateToastAdorner(owner, message, type, center);
        }

        public static void Push(string message, ToastImage type = ToastImage.Info, bool center = false)
        {
            CreateToastAdorner(message: message, type: type, center: center);
        }

        public static void Push(IntPtr intPtr, string message, ToastImage type = ToastImage.Info, bool center = false)
        {
            PushDesktop(message, type, center, intPtr);
        }

        public static void PushDesktop(string message, ToastImage type = ToastImage.Info, bool center = false, IntPtr intPtr = default)
        {
            var targetArea = ToastExt.GetTargetArea(intPtr);
            if (_toastExt == null)
            {
                _toastExt = new ToastExt();
                _toastExt.Closed += delegate { _toastExt = null; };
            }
            if (!_toastExt.IsVisible)
                _toastExt.Show();
            if (_toastExt.Position != _position)
            {
                _toastExt.Position = _position;
            }
            _toastExt.IsPosition = false;
            _toastExt.Push(message, type, center, targetArea);
        }

        public static void SetPosition(Position position = Position.Top)
        {
            if (_position != position)
                _position = position;
        }

        public static void Clear(Window owner = null)
        {
            if (owner == null)
                owner = ControlsHelper.GetDefaultWindow();

            if (owner != null && _windowAdorners.ContainsKey(owner))
                _windowAdorners[owner].Clear();
        }

        public static void ClearDesktop()
        {
            if (_toastExt != null)
                _toastExt.Clear();
        }

        public static void ClearAll()
        {
            CleanupAll();
        }

        private static void CleanupAll()
        {
            try
            {
                foreach (var kvp in _windowAdorners)
                {
                    kvp.Value?.Clear();
                }
                _windowAdorners.Clear();
                if (_toastExt != null)
                {
                    _toastExt.Clear();
                    _toastExt = null;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"CleanupAll failed: {ex.Message}");
#endif
            }
        }
    }
    public enum ToastImage
    {
        Info,
        Success,
        Warning,
        Error
    }
}
