using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public sealed class ToastExt : Window
    {
        private ToastListBox _listBox;
        internal bool IsPosition;
        internal Position Position;

        public ToastExt()
        {
            Resources = ThemeManager.Instance.Resources;
            SizeToContent = SizeToContent.Width;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            ShowActivated = false;
            Background = Brushes.Transparent;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Topmost = true;
            if (_listBox == null)
            {
                _listBox = new ToastListBox();
                _listBox.SizeChanged -= ListBox_SizeChanged;
                _listBox.SizeChanged += ListBox_SizeChanged;
                Content = _listBox;
            }
           
        }

        internal void Push(string message, ToastImage type, bool center, Rect targetArea)
        {
            var messageItem = new ToastListBoxItem
            {
                Content = message,
                ToastType = type,
                IsCenter = center
            };
            _listBox.Items.Insert(0, messageItem);
            if (!IsPosition || Position == Position.Bottom)
            {
                CalculateAndSetPosition(targetArea, messageItem);
                IsPosition = true;
            }
        }

        internal static Rect GetTargetArea(IntPtr windowHandle)
        {
            if (windowHandle != IntPtr.Zero)
            {
                Win32.RECT rect;
                if (!Win32.GetWindowRect(windowHandle, out rect))
                    throw new InvalidOperationException("Unable to get the target window bounds.");
                return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }

            var application = Application.Current;
            var mainWindow = application?.MainWindow;
            if (mainWindow is ToastExt || mainWindow == null)
            {
                if (application != null)
                {
                    foreach (Window window in application.Windows)
                    {
                        if (window != null && !(window is ToastExt))
                        {
                            mainWindow = window;
                            break;
                        }
                    }
                }
            }

            if (mainWindow != null)
            {
                var interopHelper = new WindowInteropHelper(mainWindow);
                var mainWindowHandle = interopHelper.Handle;
                if (mainWindowHandle == IntPtr.Zero)
                    mainWindowHandle = interopHelper.EnsureHandle();

                if (mainWindowHandle != IntPtr.Zero)
                {
                    var screen = System.Windows.Forms.Screen.FromHandle(mainWindowHandle);
                    var workArea = screen.WorkingArea;
                    return new Rect(workArea.Left, workArea.Top, workArea.Width, workArea.Height);
                }
            }

            var primaryScreen = System.Windows.Forms.Screen.PrimaryScreen;
            if (primaryScreen == null)
                throw new InvalidOperationException("Unable to find the primary screen.");
            var primaryWorkArea = primaryScreen.WorkingArea;
            return new Rect(primaryWorkArea.Left, primaryWorkArea.Top, primaryWorkArea.Width, primaryWorkArea.Height);
        }

        private void CalculateAndSetPosition(Rect targetArea, ToastListBoxItem item)
        {
            // Select the target monitor before converting WPF sizes to device pixels.
            WindowStartupHelper.SetPosition(this, (int)targetArea.Left, (int)targetArea.Top);
            var source = PresentationSource.FromVisual(this);
            if (source == null || source.CompositionTarget == null)
                throw new InvalidOperationException("Unable to get the Toast window's DPI transform.");
            var transform = source.CompositionTarget.TransformToDevice;
            var scaleX = transform.M11;
            var scaleY = transform.M22;
            double x = 0;
            double y = 0;

            switch (Position)
            {
                case Position.Left:
                    x = targetArea.Left + item.Margin.Left * scaleX;
                    y = targetArea.Top;
                    break;

                case Position.Top:
                    x = targetArea.Left + (targetArea.Width - item.Width * scaleX) / 2;
                    y = targetArea.Top;
                    break;

                case Position.Right:
                    x = targetArea.Right - (item.Width + item.Margin.Right + item.Padding.Right) * scaleX;
                    y = targetArea.Top;
                    break;

                case Position.Bottom:
                    x = targetArea.Left + (targetArea.Width - item.Width * scaleX) / 2;
                    _listBox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    y = targetArea.Bottom - _listBox.DesiredSize.Height * scaleY;
                    break;
            }

            Height = targetArea.Height / scaleY;
            WindowStartupHelper.SetPosition(this, (int)Math.Round(x), (int)Math.Round(y));
        }

        internal void Clear() 
        {
            if(_listBox!=null)
                _listBox.Items.Clear();
            Close();
        }

        private void ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(_listBox.ActualHeight <= 10)
                Close();
        }
    }
}
