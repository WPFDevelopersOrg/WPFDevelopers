using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPFDevelopers.Helpers
{
    internal sealed class WindowThemeMenu
    {
        private static readonly DependencyProperty ControllerProperty =
            DependencyProperty.RegisterAttached("Controller", typeof(WindowThemeMenu), typeof(WindowThemeMenu));

        private readonly Button _button;
        private readonly Popup _popup;
        private readonly UIElement _popupChild;
        private readonly StackPanel _items;
        private readonly DispatcherTimer _closeTimer;
        private readonly Resources _resources;
        private readonly Window _window;

        private WindowThemeMenu(Button button)
        {
            _button = button;
            _resources = ThemeManager.Instance.Resources;
            _window = Window.GetWindow(button);
            button.ApplyTemplate();
            _popup = button.Template.FindName("PART_ThemePopup", button) as Popup;
            _items = button.Template.FindName("PART_ThemeItems", button) as StackPanel;
            if (_popup == null || _items == null)
                throw new InvalidOperationException("The window theme button template requires PART_ThemePopup and PART_ThemeItems.");
            _popupChild = _popup.Child;
            _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            _closeTimer.Tick += OnCloseTimer;
            button.MouseEnter += OnMouseEnter;
            button.MouseLeave += OnMouseLeave;
            button.IsVisibleChanged += OnIsVisibleChanged;
            button.IsEnabledChanged += OnIsVisibleChanged;
            _popupChild.MouseEnter += OnMouseEnter;
            _popupChild.MouseLeave += OnMouseLeave;
            _popupChild.PreviewKeyDown += OnKeyDown;
            _items.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnItemClick));
            _resources.ThemeModeChanged += Synchronize;
            if (_window != null) _window.Deactivated += OnDeactivated;
            Synchronize(_resources.ThemeMode);
        }

        internal static void Attach(Button button)
        {
            if (button == null) return;
            Detach(button);
            button.SetValue(ControllerProperty, new WindowThemeMenu(button));
        }

        internal static void Detach(Button button)
        {
            if (button == null) return;
            var controller = (WindowThemeMenu)button.GetValue(ControllerProperty);
            if (controller == null) return;
            controller.Close();
            controller._closeTimer.Tick -= controller.OnCloseTimer;
            controller._popupChild.MouseEnter -= controller.OnMouseEnter;
            controller._popupChild.MouseLeave -= controller.OnMouseLeave;
            controller._popupChild.PreviewKeyDown -= controller.OnKeyDown;
            controller._items.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(controller.OnItemClick));
            button.MouseEnter -= controller.OnMouseEnter;
            button.MouseLeave -= controller.OnMouseLeave;
            button.IsVisibleChanged -= controller.OnIsVisibleChanged;
            button.IsEnabledChanged -= controller.OnIsVisibleChanged;
            controller._resources.ThemeModeChanged -= controller.Synchronize;
            if (controller._window != null) controller._window.Deactivated -= controller.OnDeactivated;
            button.ClearValue(ControllerProperty);
        }

        internal static void Open(Button button)
        {
            var controller = (WindowThemeMenu)button.GetValue(ControllerProperty);
            if (controller == null) return;
            controller._closeTimer.Stop();
            controller._popup.IsOpen = true;
            if (!button.IsMouseOver)
                button.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    if (controller._popup.IsOpen)
                        controller._items.Children[(int)controller._resources.ThemeMode].Focus();
                }));
        }

        private void OnItemClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as ToggleButton;
            if (item == null) return;
            Close();
            ThemeManager.Instance.SwitchThemeMode(_button, (ThemeMode)item.Tag);
            Synchronize(_resources.ThemeMode);
            e.Handled = true;
        }

        private void Synchronize(ThemeMode mode)
        {
            _button.SetCurrentValue(FrameworkElement.TagProperty, mode);
            foreach (ToggleButton item in _items.Children)
                item.IsChecked = (ThemeMode)item.Tag == mode;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            _closeTimer.Stop();
            if (sender == _button && _button.IsEnabled) _popup.IsOpen = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _closeTimer.Start();
        }

        private void OnCloseTimer(object sender, EventArgs e)
        {
            _closeTimer.Stop();
            if (!_button.IsMouseOver && !_popupChild.IsMouseOver) Close();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_button.IsVisible || !_button.IsEnabled) Close();
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                _button.Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Down || e.Key == Key.Up)
            {
                var index = _items.Children.IndexOf(Keyboard.FocusedElement as UIElement);
                index = (index + (e.Key == Key.Down ? 1 : 2)) % 3;
                _items.Children[index].Focus();
                e.Handled = true;
            }
        }

        private void Close()
        {
            _closeTimer.Stop();
            _popup.IsOpen = false;
        }
    }
}
