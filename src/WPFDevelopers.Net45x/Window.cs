using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;
using WPFDevelopers.Controls;
using WPFDevelopers.Core.Helpers;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Net45x
{
    [TemplatePart(Name = TitleBarIcon, Type = typeof(Button))]
    [TemplatePart(Name = TitleBarMaximizeButton, Type = typeof(Button))]
    [TemplatePart(Name = TitleBarRestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = TitleBarThemeSwitchButton, Type = typeof(Button))]
    [TemplatePart(Name = ThemePopup, Type = typeof(Popup))]
    [TemplatePart(Name = ThemeItems, Type = typeof(StackPanel))]
    public class Window : System.Windows.Window
    {
        private const string TitleBarIcon = "PART_TitleBarIcon";
        private const string TitleBarMaximizeButton = "PART_TitleBarMaximizeButton";
        private const string TitleBarRestoreButton = "PART_TitleBarRestoreButton";
        private const string TitleBarThemeSwitchButton = "PART_TitleBarThemeSwitchButton";
        private const string ThemePopup = "PART_ThemePopup";
        private const string ThemeItems = "PART_ThemeItems";
        private Button _titleBarThemeSwitchButton;
        private WindowStyle _windowStyle;
        private Button _titleBarIcon;
        private Button _titleBarMaximizeButton;
        private Button _titleBarRestoreButton;
        private IntPtr hWnd;
        private System.Windows.Shell.WindowChrome _windowChrome;
        private Popup _themePopup;
        private UIElement _themePopupChild;
        private StackPanel _themeItems;
        private DispatcherTimer _themeCloseTimer;
        private Resources _themeResources;

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

        public static readonly DependencyProperty ShowThemeProperty =
            DependencyProperty.Register("ShowTheme", typeof(bool), typeof(Window), new PropertyMetadata(false));

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
            var isDark = currentTheme == ThemeType.Dark;
            var source = (HwndSource)PresentationSource.FromVisual(this);
            Win32.EnableDarkModeForWindow(source, isDark);
        }

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            DetachThemeSwitchButtons();
            base.OnTemplateChanged(oldTemplate, newTemplate);
        }

        public override void OnApplyTemplate()
        {
            DetachThemeSwitchButtons();
            base.OnApplyTemplate();
            _titleBarThemeSwitchButton = GetTemplateChild(TitleBarThemeSwitchButton) as Button;
            if (_titleBarThemeSwitchButton != null)
            {
                _titleBarThemeSwitchButton.Click += ThemeSwitchButton_Click;
                _themePopup = GetTemplateChild(ThemePopup) as Popup;
                _themeItems = GetTemplateChild(ThemeItems) as StackPanel;
                if (_themePopup == null || _themeItems == null)
                    throw new InvalidOperationException("The window template requires PART_ThemePopup and PART_ThemeItems.");

                _themePopup.PlacementTarget = _titleBarThemeSwitchButton;
                _themePopupChild = _themePopup.Child as UIElement;
                _themeCloseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
                _themeCloseTimer.Tick += ThemeCloseTimer_Tick;
                _titleBarThemeSwitchButton.MouseEnter += ThemeMenu_MouseEnter;
                _titleBarThemeSwitchButton.MouseLeave += ThemeMenu_MouseLeave;
                _titleBarThemeSwitchButton.IsVisibleChanged += ThemeSwitchButton_VisibilityChanged;
                _titleBarThemeSwitchButton.IsEnabledChanged += ThemeSwitchButton_VisibilityChanged;
                if (_themePopupChild != null)
                {
                    _themePopupChild.MouseEnter += ThemeMenu_MouseEnter;
                    _themePopupChild.MouseLeave += ThemeMenu_MouseLeave;
                }
                _themeItems.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ThemeItem_Click));
                _themeResources = ThemeManager.Instance.Resources;
                _themeResources.ThemeModeChanged += ThemeMode_Changed;
                Deactivated += ThemeWindow_Deactivated;
                ThemeMode_Changed(_themeResources.ThemeMode);
            }
            _windowStyle = WindowStyle;
            _titleBarIcon = GetTemplateChild(TitleBarIcon) as Button;
            if (_titleBarIcon != null)
            {
                _titleBarIcon.MouseDoubleClick -= Icon_MouseDoubleClick;
                _titleBarIcon.MouseDoubleClick += Icon_MouseDoubleClick;
            }
            _titleBarMaximizeButton = GetTemplateChild(TitleBarMaximizeButton) as Button;
            _titleBarRestoreButton = GetTemplateChild(TitleBarRestoreButton) as Button;
            SetTitleHeight();
        }

        private void DetachThemeSwitchButtons()
        {
            if (_themeCloseTimer != null)
            {
                _themeCloseTimer.Stop();
                _themeCloseTimer.Tick -= ThemeCloseTimer_Tick;
                _themeCloseTimer = null;
            }
            CloseThemePopup();
            if (_themeItems != null)
                _themeItems.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ThemeItem_Click));
            if (_themePopupChild != null)
            {
                _themePopupChild.MouseEnter -= ThemeMenu_MouseEnter;
                _themePopupChild.MouseLeave -= ThemeMenu_MouseLeave;
                _themePopupChild = null;
            }
            if (_titleBarThemeSwitchButton != null)
            {
                _titleBarThemeSwitchButton.Click -= ThemeSwitchButton_Click;
                _titleBarThemeSwitchButton.MouseEnter -= ThemeMenu_MouseEnter;
                _titleBarThemeSwitchButton.MouseLeave -= ThemeMenu_MouseLeave;
                _titleBarThemeSwitchButton.IsVisibleChanged -= ThemeSwitchButton_VisibilityChanged;
                _titleBarThemeSwitchButton.IsEnabledChanged -= ThemeSwitchButton_VisibilityChanged;
            }
            if (_themeResources != null)
            {
                _themeResources.ThemeModeChanged -= ThemeMode_Changed;
                _themeResources = null;
            }
            Deactivated -= ThemeWindow_Deactivated;
            _themePopup = null;
            _themeItems = null;
        }

        private void ThemeSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_themePopup != null)
                _themePopup.IsOpen = true;
        }

        private void ThemeMenu_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _themeCloseTimer?.Stop();
            if (sender == _titleBarThemeSwitchButton && _themePopup != null)
                _themePopup.IsOpen = true;
        }

        private void ThemeMenu_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _themeCloseTimer?.Start();
        }

        private void ThemeSwitchButton_VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_titleBarThemeSwitchButton.IsVisible || !_titleBarThemeSwitchButton.IsEnabled)
                CloseThemePopup();
        }

        private void ThemeWindow_Deactivated(object sender, EventArgs e)
        {
            CloseThemePopup();
        }

        private void ThemeCloseTimer_Tick(object sender, EventArgs e)
        {
            _themeCloseTimer.Stop();
            if (_titleBarThemeSwitchButton.IsMouseOver || (_themePopupChild != null && _themePopupChild.IsMouseOver))
                return;
            _themePopup.IsOpen = false;
        }

        private void CloseThemePopup()
        {
            _themeCloseTimer?.Stop();
            if (_themePopup != null)
                _themePopup.IsOpen = false;
        }

        private void ThemeItem_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as ToggleButton;
            if (item == null)
                return;

            CloseThemePopup();
            ThemeManager.Instance.SwitchThemeMode(_titleBarThemeSwitchButton, (ThemeMode)item.Tag);
            e.Handled = true;
        }

        private void ThemeMode_Changed(ThemeMode mode)
        {
            _titleBarThemeSwitchButton.SetCurrentValue(FrameworkElement.TagProperty, mode);
            foreach (ToggleButton item in _themeItems.Children)
                item.IsChecked = (ThemeMode)item.Tag == mode;
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
            if (TitleBarMode == TitleBarMode.Normal && e.ChangedButton == MouseButton.Left)
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

        public bool ShowTheme
        {
            get => (bool)GetValue(ShowThemeProperty);
            set => SetValue(ShowThemeProperty, value);
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
            var resources = ThemeManager.Instance.Resources;
            if (resources != null)
                Win32.EnableDarkModeForWindow(source, resources.IsDarkTheme);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
                InvalidateMeasure();
        }

        protected override void OnClosed(EventArgs e)
        {
            DetachThemeSwitchButtons();
            WPFDevelopers.Resources.ThemeChanged -= Resources_ThemeChanged;
            base.OnClosed(e);
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
                        var button = WindowState != WindowState.Maximized ? _titleBarMaximizeButton : _titleBarRestoreButton;
                        if (button != null)
                        {
                            bool isOverButton = IsPointOverButton(button, lParam);
                            if (isOverButton)
                            {
                                button.SetCurrentValue(
                                    Button.BackgroundProperty,
                                    (Brush)button.FindResource("WD.WindowCaptionButtonHoverBrush"));
                                handled = true;
                                return new IntPtr(OSVersionHelper.HTMAXBUTTON);
                            }

                            button.ClearValue(Button.BackgroundProperty);
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
                        Button button = WindowState != WindowState.Maximized ? _titleBarMaximizeButton : _titleBarRestoreButton;
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