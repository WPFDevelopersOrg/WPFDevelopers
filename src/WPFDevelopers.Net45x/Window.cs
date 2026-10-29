using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using WPFDevelopers.Controls;
using WPFDevelopers.Core.Helpers;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Net45x
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
        private System.Windows.Shell.WindowChrome _windowChrome;

        public static readonly DependencyProperty TitleHeightProperty =
            DependencyProperty.Register("TitleHeight", typeof(double), typeof(Window), new PropertyMetadata(50d, OnTitleHeightChanged));

        public static readonly DependencyProperty TitleBarProperty =
            DependencyProperty.Register("TitleBar", typeof(object), typeof(Window), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleBackgroundProperty =
          DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(Window), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleBarModeProperty =
           DependencyProperty.Register("TitleBarMode", typeof(TitleBarMode), typeof(Window), new PropertyMetadata(TitleBarMode.Normal, OnTitleBarModeChanged));

        public static readonly DependencyProperty TitleBarLayoutProperty =
            DependencyProperty.Register("TitleBarLayout", typeof(object), typeof(Window), new PropertyMetadata(null));

        static Window()
        {
            StyleProperty.OverrideMetadata(typeof(Window),
                new FrameworkPropertyMetadata(GetResourceKey<Style>("WPFDevelopersWindow")));
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
            _windowChrome = new System.Windows.Shell.WindowChrome
            {
                CaptionHeight = TitleHeight,
                GlassFrameThickness = new Thickness(0, 0, 0, 0.1),
                UseAeroCaptionButtons = false
            };
            System.Windows.Shell.WindowChrome.SetWindowChrome(this, _windowChrome);
            StateChanged += OnWindow_StateChanged;
        }

        private void OnWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowChrome.SetWindowChrome(this, new WindowChrome
                {
                    CaptionHeight = 40,
                    GlassFrameThickness = new Thickness(0),
                    NonClientFrameEdges = NonClientFrameEdges.Right
                });
            }
            else
            {
                WindowChrome.SetWindowChrome(this, _windowChrome);
            }
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
            SetTitleHeight();
        }
        private static void OnTitleBarModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Window;
            if (ctrl != null)
                ctrl.SetTitleHeight();
        }
        private static void OnTitleHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Window;
            if (ctrl != null)
                ctrl._windowChrome.CaptionHeight = ctrl.TitleHeight;
        }
        void SetTitleHeight()
        {
            if (TitleBarMode == TitleBarMode.Normal)
                TitleHeight = SystemParameters.WindowNonClientFrameThickness.Top + SystemParameters.WindowResizeBorderThickness.Top;
            else if (TitleBarMode == TitleBarMode.High)
                TitleHeight = 50d;
            else
                _windowChrome.CaptionHeight = TitleHeight;
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

        public object TitleBarLayout
        {
            get => GetValue(TitleBarLayoutProperty);
            set => SetValue(TitleBarLayoutProperty, value);
        }

        private static T GetResourceKey<T>(string key)
        {
            var t = ThemeManager.Instance.Resources.TryFindResource<T>(key);
            if (t != null && t is T resource)
                return resource;
            return default;
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
                WindowStartupHelper.CenterOnCursorMonitor(this);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            hWnd = new WindowInteropHelper(this).Handle;
            var source = HwndSource.FromHwnd(hWnd);
            source.AddHook(WindowProc);
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
            SystemCommands.MaximizeWindow(this);
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
                #region SnapLayouts
                case WindowsMessageCodes.WM_NCHITTEST:
                    if (OSVersionHelper.IsSnapLayoutSupported() && ResizeMode != ResizeMode.NoResize && ResizeMode != ResizeMode.CanMinimize)
                    {
                        var button = TitleBarMode == TitleBarMode.Normal
                            ? (WindowState != WindowState.Maximized ? _titleBarMaximizeButton : _titleBarRestoreButton)
                            : (WindowState != WindowState.Maximized ? _highTitleMaximizeButton : _highTitleRestoreButton);
                        if (button != null)
                        {
                            var contentPresenter = button.Template.FindName("PART_ContentPresenter", button) as ContentPresenter;
                            if (contentPresenter != null)
                            {
                                bool isOverButton = IsPointOverButton(button, lParam);
                                contentPresenter.Opacity = isOverButton ? 1 : 0.7;
                                if (isOverButton)
                                {
                                    handled = true;
                                    return new IntPtr(OSVersionHelper.HTMAXBUTTON);
                                }
                            }
                        }
                    }
                    break;
                case WindowsMessageCodes.WM_NCLBUTTONDOWN:
                    if (OSVersionHelper.IsSnapLayoutSupported()
                        &&
                        (ResizeMode != ResizeMode.NoResize
                        &&
                        ResizeMode != ResizeMode.CanMinimize))
                    {
                        Button button = TitleBarMode == TitleBarMode.Normal
                            ? (WindowState != WindowState.Maximized ? _titleBarMaximizeButton : _titleBarRestoreButton)
                            : (WindowState != WindowState.Maximized ? _highTitleMaximizeButton : _highTitleRestoreButton);
                        if (button != null)
                        {
                            if (IsPointOverButton(button, lParam))
                            {
                                handled = true;
                                IInvokeProvider invokeProv = new ButtonAutomationPeer(button).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                                invokeProv?.Invoke();
                            }
                        }
                    }
                    break;
                #endregion
                default:
                    handled = false;
                    break;
            }
            return IntPtr.Zero;
        }



        private static bool IsPointOverButton(Button button, IntPtr lParam)
        {
            long coordinates = lParam.ToInt64();
            var screenPoint = new Point(
                unchecked((short)(coordinates & 0xffff)),
                unchecked((short)((coordinates >> 16) & 0xffff)));
            return new Rect(button.RenderSize).Contains(button.PointFromScreen(screenPoint));
        }

        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == null)
                return;

            var screenPoint = element.PointToScreen(new Point(0, element.ActualHeight));
            var source = HwndSource.FromHwnd(hWnd);
            var point = source.CompositionTarget.TransformFromDevice.Transform(screenPoint);
            SystemCommands.ShowSystemMenu(this, point);
        }

        #endregion
    }

}