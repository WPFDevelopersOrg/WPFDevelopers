using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class WDScrollViewer : ScrollViewer
    {
        private double _animationTarget;
        private bool _isAnimating;
        private int _animationVersion;

        public bool IsScrollAnimation
        {
            get { return (bool)GetValue(IsScrollAnimationProperty); }
            set { SetValue(IsScrollAnimationProperty, value); }
        }

        public static readonly DependencyProperty IsScrollAnimationProperty =
            DependencyProperty.Register("IsScrollAnimation", typeof(bool), typeof(WDScrollViewer), new PropertyMetadata(false, OnIsScrollAnimationChanged));

        private static void OnIsScrollAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as WDScrollViewer;
            if (ctrl == null) return;
            if (!ctrl.IsScrollAnimation)
                ctrl.StopScrollAnimation();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!IsScrollAnimation)
            {
                base.OnMouseWheel(e);
                return;
            }
            if (e.Handled)
                return;
            var change = -e.Delta * 2.0;
            var start = _isAnimating && (_animationTarget - VerticalOffset) * change > 0
                ? _animationTarget
                : VerticalOffset;
            var newOffset = Math.Max(0, Math.Min(start + change, ScrollableHeight));
            AnimateScroll(newOffset);
            e.Handled = true;
        }

        public void AnimateScroll(double toValue, Action onCompleted = null)
        {
            if (double.IsNaN(toValue))
                throw new ArgumentOutOfRangeException("toValue");

            var from = VerticalOffset;
            _animationTarget = Math.Max(0, Math.Min(toValue, ScrollableHeight));
            _isAnimating = true;
            var version = ++_animationVersion;
            var animation = new DoubleAnimation
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                From = from,
                To = _animationTarget,
                Duration = TimeSpan.FromMilliseconds(250)
            };
            animation.Completed += (s, e) =>
            {
                if (version != _animationVersion)
                    return;
                var target = _animationTarget;
                StopScrollAnimation();
                ScrollToVerticalOffset(target);
                onCompleted?.Invoke();
            };
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, animation,
                HandoffBehavior.SnapshotAndReplace);
        }

        private void StopScrollAnimation()
        {
            ++_animationVersion;
            _isAnimating = false;
            SetValue(ScrollViewerBehavior.VerticalOffsetProperty, VerticalOffset);
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, null);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (_isAnimating)
                StopScrollAnimation();
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (_isAnimating && (e.Key == Key.Up || e.Key == Key.Down ||
                e.Key == Key.PageUp || e.Key == Key.PageDown ||
                e.Key == Key.Home || e.Key == Key.End))
                StopScrollAnimation();
            base.OnPreviewKeyDown(e);
        }
    }
}
