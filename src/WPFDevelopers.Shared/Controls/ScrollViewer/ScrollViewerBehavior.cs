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

        public static readonly DependencyProperty IsBackToBottomEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsBackToBottomEnabled",
                typeof(bool),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(false, OnScrollNavigationChanged));

        public static void SetIsBackToBottomEnabled(DependencyObject target, bool value)
        {
            target.SetValue(IsBackToBottomEnabledProperty, value);
        }

        public static bool GetIsBackToBottomEnabled(DependencyObject target)
        {
            return (bool)target.GetValue(IsBackToBottomEnabledProperty);
        }

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            (target as WDScrollViewer)?.ScrollToVerticalOffset((double)e.NewValue);
        }

        private static void OnIsBackToTopEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnScrollNavigationChanged(d, e);
        }

        private static void OnScrollNavigationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as FrameworkElement;
            if (target == null)
                return;

            var state = (BackToTopState)target.GetValue(BackToTopStateProperty);
            if (GetIsBackToTopEnabled(target) || GetIsBackToBottomEnabled(target))
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
            private StackPanel _buttonPanel;
            private Button _backToTopButton;
            private Button _backToBottomButton;
            private Window _ownerWindow;
            private Point? _lastTargetScreenPosition;
            private int _stablePlacementFrameCount;
            private bool _isTrackingPlacement;

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
            }

            private void OnTargetSizeChanged(object sender, SizeChangedEventArgs e)
            {
                UpdateButton();
                StartPlacementTracking();
            }

            private void Attach()
            {
                Detach();

                var control = _target as Control;
                if (control != null)
                    control.ApplyTemplate();

                _scrollViewer = FindScrollViewer(_target);
                if (_scrollViewer == null)
                {
                    _target.LayoutUpdated -= OnTargetLayoutUpdated;
                    _target.LayoutUpdated += OnTargetLayoutUpdated;
                    return;
                }

                _target.LayoutUpdated -= OnTargetLayoutUpdated;

                _backToTopButton = GetIsBackToTopEnabled(_target)
                    ? CreateNavigationButton("BackToTop", PackIconKind.UpArrow, ScrollToTop)
                    : null;
                _backToBottomButton = GetIsBackToBottomEnabled(_target)
                    ? CreateNavigationButton("BackToBottom", PackIconKind.DownArrow, ScrollToBottom)
                    : null;
                _buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                if (_backToTopButton != null)
                    _buttonPanel.Children.Add(_backToTopButton);
                if (_backToBottomButton != null)
                {
                    if (_backToTopButton != null)
                        _backToBottomButton.Margin = new Thickness(6, 0, 0, 0);
                    _buttonPanel.Children.Add(_backToBottomButton);
                }

                _popup = new Popup
                {
                    AllowsTransparency = true,
                    Placement = PlacementMode.Relative,
                    PlacementTarget = _target,
                    StaysOpen = true,
                    Child = _buttonPanel
                };
                AttachOwnerWindow();
                _scrollViewer.ScrollChanged += OnScrollChanged;
                LanguageManager.Instance.PropertyChanged += OnLanguagePropertyChanged;
                UpdateButton();
            }

            private void Detach()
            {
                StopPlacementTracking();

                if (_scrollViewer != null)
                    _scrollViewer.ScrollChanged -= OnScrollChanged;

                LanguageManager.Instance.PropertyChanged -= OnLanguagePropertyChanged;

                if (_popup != null)
                    _popup.IsOpen = false;

                DetachOwnerWindow();
                _scrollViewer = null;
                _popup = null;
                _buttonPanel = null;
                _backToTopButton = null;
                _backToBottomButton = null;
            }

            private void AttachOwnerWindow()
            {
                _ownerWindow = Window.GetWindow(_target);
                if (_ownerWindow == null)
                    return;

                _ownerWindow.StateChanged += OnOwnerWindowStateChanged;
                _ownerWindow.IsVisibleChanged += OnOwnerWindowIsVisibleChanged;
                _ownerWindow.Activated += OnOwnerWindowActivated;
                _ownerWindow.Deactivated += OnOwnerWindowDeactivated;
                _ownerWindow.Closed += OnOwnerWindowClosed;
            }

            private void DetachOwnerWindow()
            {
                if (_ownerWindow == null)
                    return;

                _ownerWindow.StateChanged -= OnOwnerWindowStateChanged;
                _ownerWindow.IsVisibleChanged -= OnOwnerWindowIsVisibleChanged;
                _ownerWindow.Activated -= OnOwnerWindowActivated;
                _ownerWindow.Deactivated -= OnOwnerWindowDeactivated;
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

            private void OnOwnerWindowActivated(object sender, EventArgs e)
            {
                UpdateButton();
            }

            private void OnOwnerWindowDeactivated(object sender, EventArgs e)
            {
                StopPlacementTracking();
                if (_popup != null)
                    _popup.IsOpen = false;
            }

            private void OnOwnerWindowClosed(object sender, EventArgs e)
            {
                StopPlacementTracking();
                if (_popup != null)
                    _popup.IsOpen = false;
            }

            private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                UpdateButton();
            }

            private void OnLanguagePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (_buttonPanel == null)
                    return;

                UpdateLocalizedText();
            }

            private void UpdateButton()
            {
                if (_popup == null || _buttonPanel == null || _scrollViewer == null)
                    return;

                if (!_target.IsLoaded ||
                    _target.ActualWidth <= 0 ||
                    _target.ActualHeight <= 0 ||
                    PresentationSource.FromVisual(_target) == null)
                {
                    _popup.IsOpen = false;
                    return;
                }

                if (_backToTopButton != null)
                    _backToTopButton.Visibility = _scrollViewer.VerticalOffset > 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                if (_backToBottomButton != null)
                    _backToBottomButton.Visibility = _scrollViewer.VerticalOffset <
                                                     _scrollViewer.ScrollableHeight
                        ? Visibility.Visible
                        : Visibility.Collapsed;

                if (_backToTopButton != null)
                    _backToTopButton.IsEnabled = _target.IsEnabled;
                if (_backToBottomButton != null)
                    _backToBottomButton.IsEnabled = _target.IsEnabled;
                var visibleButtonCount = (_backToTopButton != null &&
                                          _backToTopButton.Visibility == Visibility.Visible ? 1 : 0) +
                                         (_backToBottomButton != null &&
                                          _backToBottomButton.Visibility == Visibility.Visible ? 1 : 0);
                var panelWidth = visibleButtonCount * 36 + Math.Max(0, visibleButtonCount - 1) * 6;
                _popup.HorizontalOffset = Math.Max(0, _target.ActualWidth - panelWidth - 16);
                _popup.VerticalOffset = Math.Max(0, _target.ActualHeight - 52);
                var isOwnerWindowVisible = _ownerWindow == null ||
                                           (_ownerWindow.IsVisible &&
                                            _ownerWindow.IsActive &&
                                            _ownerWindow.WindowState != WindowState.Minimized);
                var shouldOpen = _target.IsVisible &&
                                 isOwnerWindowVisible &&
                                 visibleButtonCount > 0;
                if (!shouldOpen)
                {
                    StopPlacementTracking();
                    _popup.IsOpen = false;
                    return;
                }

                if (!_popup.IsOpen)
                {
                    _popup.IsOpen = true;
                    StartPlacementTracking();
                }
            }

            private void StartPlacementTracking()
            {
                if (_popup == null || !_popup.IsOpen)
                    return;

                _lastTargetScreenPosition = null;
                _stablePlacementFrameCount = 0;
                if (_isTrackingPlacement)
                    return;

                CompositionTarget.Rendering += OnPlacementRendering;
                _isTrackingPlacement = true;
            }

            private void StopPlacementTracking()
            {
                if (!_isTrackingPlacement)
                    return;

                CompositionTarget.Rendering -= OnPlacementRendering;
                _isTrackingPlacement = false;
                _lastTargetScreenPosition = null;
                _stablePlacementFrameCount = 0;
            }

            private void OnPlacementRendering(object sender, EventArgs e)
            {
                if (_popup == null ||
                    !_popup.IsOpen ||
                    PresentationSource.FromVisual(_target) == null)
                {
                    StopPlacementTracking();
                    return;
                }

                var currentPosition = _target.PointToScreen(new Point());
                if (!_lastTargetScreenPosition.HasValue)
                {
                    _lastTargetScreenPosition = currentPosition;
                    return;
                }

                var previousPosition = _lastTargetScreenPosition.Value;
                if (Math.Abs(currentPosition.X - previousPosition.X) > 0.1 ||
                    Math.Abs(currentPosition.Y - previousPosition.Y) > 0.1)
                {
                    _lastTargetScreenPosition = currentPosition;
                    _stablePlacementFrameCount = 0;
                    ForcePopupReposition();
                    return;
                }

                _stablePlacementFrameCount++;
                if (_stablePlacementFrameCount >= 10)
                    StopPlacementTracking();
            }

            private void ForcePopupReposition()
            {
                var horizontalOffset = _popup.HorizontalOffset;
                _popup.HorizontalOffset = horizontalOffset + 0.1;
                _popup.HorizontalOffset = horizontalOffset;
            }

            private void ScrollToTop()
            {
                var animatedScrollViewer = _scrollViewer as WDScrollViewer;
                if (animatedScrollViewer != null && animatedScrollViewer.IsScrollAnimation)
                    animatedScrollViewer.AnimateScroll(0);
                else
                    _scrollViewer.ScrollToTop();
            }

            private Button CreateNavigationButton(string resourceKey, PackIconKind iconKind, Action clickHandler)
            {
                var text = LanguageManager.Instance[resourceKey];
                var button = new Button
                {
                    Width = 36,
                    Height = 36,
                    ToolTip = text,
                    Content = new PathIcon
                    {
                        Width = 18,
                        Height = 18,
                        Kind = iconKind,
                    }
                };
                ElementHelper.SetIsRound(button, true);
                button.SetResourceReference(FrameworkElement.StyleProperty, "WD.DefaultButton");
                AutomationProperties.SetName(button, text);
                button.Click += (sender, e) => clickHandler();
                return button;
            }

            private void UpdateLocalizedText()
            {
                UpdateButtonText(_backToTopButton, "BackToTop");
                UpdateButtonText(_backToBottomButton, "BackToBottom");
            }

            private static void UpdateButtonText(Button button, string resourceKey)
            {
                if (button == null)
                    return;

                var text = LanguageManager.Instance[resourceKey];
                button.ToolTip = text;
                AutomationProperties.SetName(button, text);
            }

            private void ScrollToBottom()
            {
                var animatedScrollViewer = _scrollViewer as WDScrollViewer;
                if (animatedScrollViewer != null && animatedScrollViewer.IsScrollAnimation)
                    animatedScrollViewer.AnimateScroll(_scrollViewer.ScrollableHeight);
                else
                    _scrollViewer.ScrollToEnd();
            }
        }
    }
}
