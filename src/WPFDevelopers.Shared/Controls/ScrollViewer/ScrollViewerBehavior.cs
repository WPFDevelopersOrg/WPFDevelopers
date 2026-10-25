using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerBehavior),
                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        public static readonly DependencyProperty IsBackToTopEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsBackToTopEnabled",
                typeof(bool),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(false, OnIsBackToTopEnabledChanged));

        private static readonly DependencyProperty BackToTopStateProperty =
            DependencyProperty.RegisterAttached(
                "BackToTopState",
                typeof(BackToTopState),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(null));

        public static void SetIsBackToTopEnabled(DependencyObject target, bool value)
        {
            target.SetValue(IsBackToTopEnabledProperty, value);
        }

        public static bool GetIsBackToTopEnabled(DependencyObject target)
        {
            return (bool)target.GetValue(IsBackToTopEnabledProperty);
        }

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            (target as WDScrollViewer)?.ScrollToVerticalOffset((double)e.NewValue);
        }

        private static void OnIsBackToTopEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as FrameworkElement;
            if (target == null)
                return;

            var state = (BackToTopState)target.GetValue(BackToTopStateProperty);
            if ((bool)e.NewValue)
            {
                if (state != null)
                    return;

                state = new BackToTopState(target);
                target.SetValue(BackToTopStateProperty, state);
                state.Start();
            }
            else if (state != null)
            {
                state.Stop();
                target.ClearValue(BackToTopStateProperty);
            }
        }

        private static ScrollViewer FindScrollViewer(DependencyObject root)
        {
            var scrollViewer = root as ScrollViewer;
            if (scrollViewer != null)
                return scrollViewer;

            return FindScrollViewer(root, true) ?? FindScrollViewer(root, false);
        }

        private static ScrollViewer FindScrollViewer(DependencyObject root, bool requireTemplatePartName)
        {
            var count = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                var scrollViewer = child as ScrollViewer;
                if (scrollViewer != null &&
                    (!requireTemplatePartName || scrollViewer.Name == "PART_ScrollViewer"))
                {
                    return scrollViewer;
                }

                scrollViewer = FindScrollViewer(child, requireTemplatePartName);
                if (scrollViewer != null)
                    return scrollViewer;
            }

            return null;
        }

        private sealed class BackToTopState
        {
            private readonly FrameworkElement _target;
            private ScrollViewer _scrollViewer;
            private Popup _popup;
            private Button _button;
            private Window _ownerWindow;

            public BackToTopState(FrameworkElement target)
            {
                _target = target;
            }

            public void Start()
            {
                _target.Loaded += OnTargetLoaded;
                _target.Unloaded += OnTargetUnloaded;
                _target.IsVisibleChanged += OnTargetStateChanged;
                _target.IsEnabledChanged += OnTargetStateChanged;
                _target.LayoutUpdated += OnTargetLayoutUpdated;
                _target.SizeChanged += OnTargetSizeChanged;

                if (_target.IsLoaded)
                    Attach();
            }

            public void Stop()
            {
                _target.Loaded -= OnTargetLoaded;
                _target.Unloaded -= OnTargetUnloaded;
                _target.IsVisibleChanged -= OnTargetStateChanged;
                _target.IsEnabledChanged -= OnTargetStateChanged;
                _target.LayoutUpdated -= OnTargetLayoutUpdated;
                _target.SizeChanged -= OnTargetSizeChanged;
                Detach();
            }

            private void OnTargetLoaded(object sender, RoutedEventArgs e)
            {
                Attach();
            }

            private void OnTargetUnloaded(object sender, RoutedEventArgs e)
            {
                Detach();
            }

            private void OnTargetStateChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                UpdateButton();
            }

            private void OnTargetLayoutUpdated(object sender, EventArgs e)
            {
                if (_scrollViewer == null)
                    Attach();
                else
                    UpdateButton();
            }

            private void OnTargetSizeChanged(object sender, SizeChangedEventArgs e)
            {
                UpdateButton();
            }

            private void Attach()
            {
                Detach();

                var control = _target as Control;
                if (control != null)
                    control.ApplyTemplate();

                _scrollViewer = FindScrollViewer(_target);
                if (_scrollViewer == null)
                    return;

                _button = CreateBackToTopButton();
                _popup = new Popup
                {
                    AllowsTransparency = true,
                    Placement = PlacementMode.Relative,
                    PlacementTarget = _target,
                    StaysOpen = true,
                    Child = _button
                };
                AttachOwnerWindow();
                _scrollViewer.ScrollChanged += OnScrollChanged;
                LanguageManager.Instance.PropertyChanged += OnLanguagePropertyChanged;
                UpdateButton();
            }

            private void Detach()
            {
                if (_scrollViewer != null)
                    _scrollViewer.ScrollChanged -= OnScrollChanged;

                LanguageManager.Instance.PropertyChanged -= OnLanguagePropertyChanged;

                if (_popup != null)
                    _popup.IsOpen = false;

                DetachOwnerWindow();
                _scrollViewer = null;
                _popup = null;
                _button = null;
            }

            private void AttachOwnerWindow()
            {
                _ownerWindow = Window.GetWindow(_target);
                if (_ownerWindow == null)
                    return;

                _ownerWindow.StateChanged += OnOwnerWindowStateChanged;
                _ownerWindow.IsVisibleChanged += OnOwnerWindowIsVisibleChanged;
                _ownerWindow.Closed += OnOwnerWindowClosed;
            }

            private void DetachOwnerWindow()
            {
                if (_ownerWindow == null)
                    return;

                _ownerWindow.StateChanged -= OnOwnerWindowStateChanged;
                _ownerWindow.IsVisibleChanged -= OnOwnerWindowIsVisibleChanged;
                _ownerWindow.Closed -= OnOwnerWindowClosed;
                _ownerWindow = null;
            }

            private void OnOwnerWindowStateChanged(object sender, EventArgs e)
            {
                UpdateButton();
            }

            private void OnOwnerWindowIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                UpdateButton();
            }

            private void OnOwnerWindowClosed(object sender, EventArgs e)
            {
                if (_popup != null)
                    _popup.IsOpen = false;
            }

            private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                UpdateButton();
            }

            private void OnLanguagePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (_button == null)
                    return;

                UpdateLocalizedText();
            }

            private void UpdateButton()
            {
                if (_popup == null || _button == null || _scrollViewer == null)
                    return;

                _button.IsEnabled = _target.IsEnabled;
                _popup.HorizontalOffset = Math.Max(0, _target.ActualWidth - 52);
                _popup.VerticalOffset = Math.Max(0, _target.ActualHeight - 52);
                var isOwnerWindowVisible = _ownerWindow == null ||
                                           (_ownerWindow.IsVisible &&
                                            _ownerWindow.WindowState != WindowState.Minimized);
                _popup.IsOpen = _target.IsVisible &&
                                isOwnerWindowVisible &&
                                _scrollViewer.VerticalOffset > 0;
            }

            private void ScrollToTop()
            {
                var animatedScrollViewer = _scrollViewer as WDScrollViewer;
                if (animatedScrollViewer != null && animatedScrollViewer.IsScrollAnimation)
                    animatedScrollViewer.AnimateScroll(0);
                else
                    _scrollViewer.ScrollToTop();
            }

            private Button CreateBackToTopButton()
            {
                var text = LanguageManager.Instance["BackToTop"];
                var button = new Button
                {
                    Width = 36,
                    Height = 36,
                    ToolTip = text,
                    Content = new PathIcon
                    {
                        Width = 18,
                        Height = 18,
                        Kind = PackIconKind.UpArrow,
                    }
                };
                ElementHelper.SetIsRound(button, true);
                button.SetResourceReference(FrameworkElement.StyleProperty, "WD.DefaultButton");
                AutomationProperties.SetName(button, text);
                button.Click += (sender, e) => ScrollToTop();
                return button;
            }

            private void UpdateLocalizedText()
            {
                var text = LanguageManager.Instance["BackToTop"];
                _button.ToolTip = text;
                AutomationProperties.SetName(_button, text);
            }
        }
    }
}
