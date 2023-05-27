using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Net45x
{
    public class Window : System.Windows.Window
    {
        private WindowStyle _windowStyle;
        public static readonly DependencyProperty TitleHeightProperty =
            DependencyProperty.Register("TitleHeight", typeof(double), typeof(Window), new PropertyMetadata(50d));

        static Window()
        {
            StyleProperty.OverrideMetadata(typeof(Window),
                new FrameworkPropertyMetadata(GetResourceKey<Style>("WPFDevelopersWindow")));
        }

        public Window()
        {
            Loaded += Window_Loaded;
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow,
                CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow,
                CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow,
                CanResizeWindow));
            //CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _windowStyle = WindowStyle;
        }
        public double TitleHeight
        {
            get => (double)GetValue(TitleHeightProperty);
            set => SetValue(TitleHeightProperty, value);
        }

        private static T GetResourceKey<T>(string key)
        {
            if (Application.Current.TryFindResource(key) is T resource) return resource;

            return default;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd).AddHook(WindowProc);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
                InvalidateMeasure();
        }

        #region Window Commands

        private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            //Close();
            SystemCommands.CloseWindow(this);
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            //SystemCommands.MinimizeWindow(this);
            SendMessage(hWnd, ApiCodes.WM_SYSCOMMAND, new IntPtr(ApiCodes.SC_MINIMIZE), IntPtr.Zero);
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        internal class ApiCodes
        {
            public const int SC_RESTORE = 0xF120;
            public const int SC_MINIMIZE = 0xF020;
            public const int WM_SYSCOMMAND = 0x0112;
        }

        private IntPtr hWnd;

        [DllImport(Win32.User32)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == ApiCodes.WM_SYSCOMMAND)
            {
                if (wParam.ToInt32() == ApiCodes.SC_MINIMIZE)
                {
                    _windowStyle = WindowStyle;
                    if (WindowStyle != WindowStyle.SingleBorderWindow)
                        WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Minimized;
                    handled = true;
                }
                else if (wParam.ToInt32() == ApiCodes.SC_RESTORE)
                {
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;
                    if (WindowStyle.None != _windowStyle)
                        WindowStyle = _windowStyle;
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == null)
                return;

            var point = WindowState == WindowState.Maximized
                ? new Point(0, element.ActualHeight)
                : new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
            point = element.TransformToAncestor(this).Transform(point);
            SystemCommands.ShowSystemMenu(this, point);
        }

        #endregion
    }
}