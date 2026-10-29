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

        internal void Push(string message, ToastImage type = ToastImage.Info,
                  bool center = false, IntPtr intPtr = default)
        {
            Rect targetArea = GetTargetArea(intPtr);
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

        private Rect GetTargetArea(IntPtr windowHandle)
        {
            if (windowHandle != IntPtr.Zero)
            {
                IsPosition = false;
                return WindowHelpers.GetWindowBounds(windowHandle);
            }

            IsPosition = false;
            var mainWindow = Application.Current?.MainWindow;
            if (mainWindow != null && mainWindow.IsLoaded)
            {
                var mainWindowHandle = new WindowInteropHelper(mainWindow).Handle;
                if (mainWindowHandle != IntPtr.Zero)
                {
                    var screen = System.Windows.Forms.Screen.FromHandle(mainWindowHandle);
                    var dpi = Win32.GetDpiForWindow(mainWindowHandle);
                    var scale = dpi > 0 ? dpi / 96.0 : 1.0;
                    var workArea = screen.WorkingArea;
                    return new Rect(
                        workArea.Left / scale,
                        workArea.Top / scale,
                        workArea.Width / scale,
                        workArea.Height / scale);
                }
            }

            return SystemParameters.WorkArea;
        }

        private void CalculateAndSetPosition(Rect targetArea, ToastListBoxItem item)
        {
            double x = 0;
            double y = 0;

            switch (Position)
            {
                case Position.Left:
                    x = targetArea.Left + item.Margin.Left;
                    y = targetArea.Top;
                    break;

                case Position.Top:
                    x = targetArea.Left + (targetArea.Width - item.Width) / 2;
                    y = targetArea.Top;
                    break;

                case Position.Right:
                    x = targetArea.Right - (item.Width + item.Margin.Right + item.Padding.Right);
                    y = targetArea.Top;
                    break;

                case Position.Bottom:
                    x = targetArea.Left + (targetArea.Width - item.Width) / 2;
                    _listBox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    y = targetArea.Bottom - _listBox.DesiredSize.Height;
                    break;
            }

            Height = targetArea.Height;
            Left = x;
            Top = y;
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
