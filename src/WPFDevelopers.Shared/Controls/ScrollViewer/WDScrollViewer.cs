using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class WDScrollViewer : ScrollViewer
    {
        private static double _lastLocation;

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
            if (ctrl.IsScrollAnimation)
            {
                ctrl.ScrollChanged -= OnScrollChanged;
                ctrl.ScrollChanged += OnScrollChanged;
            }
            else
                ctrl.ScrollChanged -= OnScrollChanged;
        }
       
        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var ctrl = sender as WDScrollViewer;
            if (ctrl == null) return;
            if (e.VerticalChange != 0)
                _lastLocation = ctrl.VerticalOffset;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!IsScrollAnimation)
            {
                base.OnMouseWheel(e);
                return;
            }
            var WheelChange = e.Delta;
            var newOffset = _lastLocation - WheelChange * 2;
            ScrollToVerticalOffset(_lastLocation);
            if (newOffset < 0)
                newOffset = 0;
            if (newOffset > ScrollableHeight)
                newOffset = ScrollableHeight;
            AnimateScroll(newOffset);
            _lastLocation = newOffset;
            e.Handled = true;
        }

        public void AnimateScroll(double toValue, Action onCompleted = null)
        {
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, null);
            var animation = new DoubleAnimation
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                From = VerticalOffset,
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(800)
            };
            Timeline.SetDesiredFrameRate(animation, 40);
            animation.Completed += (s, e) => onCompleted?.Invoke();
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, animation);
        }
    }
}