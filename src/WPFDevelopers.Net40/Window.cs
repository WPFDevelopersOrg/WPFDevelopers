using Microsoft.Windows.Shell;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using WPFDevelopers.Controls;
using WPFDevelopers.Core.Helpers;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Net40
{
    [TemplatePart(Name = TitleBarIcon, Type = typeof(Button))]
    [TemplatePart(Name = HighTitleMaximizeButton, Type = typeof(Button))]
    [TemplatePart(Name = HighTitleRestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = TitleBarMaximizeButton, Type = typeof(Button))]
    [TemplatePart(Name = TitleBarRestoreButton, Type = typeof(Button))]
    public class Window : System.Windows.Window
    {
        private const string TitleBarIcon = "PART_TitleBarIcon";
        private const string HighTitleMaximizeButton = "PART_MaximizeButton";
        private const string HighTitleRestoreButton = "PART_RestoreButton";
        private const string TitleBarMaximizeButton = "PART_TitleBarMaximizeButton";
        private const string TitleBarRestoreButton = "PART_TitleBarRestoreButton";
        private WindowStyle _windowStyle;
        private Button _titleBarIcon;
        private Button _highTitleMaximizeButton;
        private Button _highTitleRestoreButton;
        private Button _titleBarMaximizeButton;
        private Button _titleBarRestoreButton;
        private IntPtr hWnd;

        public static readonly DependencyProperty TitleHeightProperty =
            DependencyProperty.Register("TitleHeight", typeof(double), typeof(Window), new PropertyMetadata(50d));

        public static readonly DependencyProperty NoChromeProperty =
            DependencyProperty.Register("NoChrome", typeof(bool), typeof(Window), new PropertyMetadata(false));

        public static readonly DependencyProperty TitleBarProperty =
            DependencyProperty.Register("TitleBar", typeof(object), typeof(Window), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleBackgroundProperty =
           DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(Window), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleBarModeProperty =
            DependencyProperty.Register("TitleBarMode", typeof(TitleBarMode), typeof(Window), new PropertyMetadata(TitleBarMode.Normal));

        static Window()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        }

        public Window()
        {
            WPFDevelopers.Resources.ThemeChanged += Resources_ThemeChanged;
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow,
                CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow,
                CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow,
                CanResizeWindow));
        }

        private void Resources_ThemeChanged(ThemeType currentTheme)
        {
            var isDark = currentTheme == ThemeType.Dark ? true : false;
            var source = (HwndSource)PresentationSource.FromVisual(this);
            Win32.EnableDarkModeForWindow(source, isDark);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _windowStyle = WindowStyle;
            _titleBarIcon = GetTemplateChild(TitleBarIcon) as Button;
            if (_titleBarIcon != null)
            {
                _titleBarIcon.MouseDoubleClick -= Icon_MouseDoubleClick;
                _titleBarIcon.MouseDoubleClick += Icon_MouseDoubleClick;
            }
            _highTitleMaximizeButton = GetTemplateChild(HighTitleMaximizeButton) as Button;
            _highTitleRestoreButton = GetTemplateChild(HighTitleRestoreButton) as Button;
            _titleBarMaximizeButton = GetTemplateChild(TitleBarMaximizeButton) as Button;
            _titleBarRestoreButton = GetTemplateChild(TitleBarRestoreButton) as Button;
        }

        private void Icon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Close();
        }

        public double TitleHeight
        {
            get => (double)GetValue(TitleHeightProperty);
            set => SetValue(TitleHeightProperty, value);
        }

        public bool NoChrome
        {
            get => (bool)GetValue(NoChromeProperty);
            set => SetValue(NoChromeProperty, value);
        }

        public object TitleBar
        {
            get => (object)GetValue(TitleBarProperty);
            set => SetValue(TitleBarProperty, value);
        }

        public Brush TitleBackground
        {
            get => (Brush)GetValue(TitleBackgroundProperty);
            set => SetValue(TitleBackgroundProperty, value);
        }

        public TitleBarMode TitleBarMode
        {
            get => (TitleBarMode)GetValue(TitleBarModeProperty);
            set => SetValue(TitleBarModeProperty, value);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd).AddHook(WindowProc);
            if (TitleBarMode == TitleBarMode.Normal)
                TitleHeight = SystemParameters2.Current.WindowNonClientFrameThickness.Top;
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
            SystemCommands.CloseWindow(this);
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Win32.SendMessage(hWnd, WindowsMessageCodes.WM_SYSCOMMAND, new IntPtr(WindowsMessageCodes.SC_MINIMIZE), IntPtr.Zero);
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }


        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WindowsMessageCodes.WM_SYSCOMMAND:
                    if (wParam.ToInt32() == WindowsMessageCodes.SC_MINIMIZE)
                    {
                        _windowStyle = WindowStyle;
                        if (WindowStyle != WindowStyle.SingleBorderWindow)
                            WindowStyle = WindowStyle.SingleBorderWindow;
                        WindowState = WindowState.Minimized;
                        handled = true;
                    }
                    else if (wParam.ToInt32() == WindowsMessageCodes.SC_RESTORE)
                    {
                        WindowState = WindowState.Normal;
                        WindowStyle = WindowStyle.None;
                        if (WindowStyle.None != _windowStyle)
                            WindowStyle = _windowStyle;
                        handled = true;
                    }
                    break;
                case WindowsMessageCodes.WM_NCHITTEST:
                case WindowsMessageCodes.WM_NCLBUTTONDOWN:
                    try
                    {
                        if (!OSVersionHelper.IsSnapLayoutSupported()
                            ||
                            ResizeMode == ResizeMode.NoResize 
                            || 
                            ResizeMode == ResizeMode.CanMinimize)
                            break;
                        IntPtr result = IntPtr.Zero;
                        if (HandleSnapLayoutMessage(msg, lParam, ref result))
                        {
                            handled = true;
                            return result;
                        }
                    }
                    catch (OverflowException)
                    {
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private bool HandleSnapLayoutMessage(int msg, IntPtr lParam, ref IntPtr result)
        {
            Button button = TitleBarMode == TitleBarMode.Normal
                ? (WindowState != WindowState.Maximized ? _titleBarMaximizeButton : _titleBarRestoreButton)
                : (WindowState != WindowState.Maximized ? _highTitleMaximizeButton : _highTitleRestoreButton);

            if (button == null || button.ActualWidth <= 0 || button.ActualHeight <= 0)
                return false;
            var contentPresenter = button.Template.FindName("PART_ContentPresenter", button) as ContentPresenter;
            if (contentPresenter == null) return false;
            var x = lParam.ToInt32() & 0xffff;
            var y = lParam.ToInt32() >> 16;
            var dpiX = OSVersionHelper.DeviceUnitsScalingFactorX;
            var rect = new Rect(button.PointToScreen(new Point()), new Size(button.ActualWidth * dpiX, button.ActualHeight * dpiX));
            var point = new Point(x, y);

            if (msg == WindowsMessageCodes.WM_NCHITTEST && contentPresenter != null)
            {
                if (!rect.Contains(point))
                {
                    if(contentPresenter.Opacity != 0.7)
                        contentPresenter.Opacity = 0.7;
                    return false;
                }
                contentPresenter.Opacity = 1;
                result = new IntPtr(OSVersionHelper.HTMAXBUTTON);
            }
            else if (msg == WindowsMessageCodes.WM_NCLBUTTONDOWN)
            {
                IInvokeProvider invokeProv = new ButtonAutomationPeer(button).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                invokeProv?.Invoke();
            }

            return true;
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