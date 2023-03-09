using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class NormalLoading : Control
    {
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(NormalLoading), new PropertyMetadata(true));

        public static readonly DependencyProperty StrokeValueProperty =
            DependencyProperty.Register("StrokeValue", typeof(double), typeof(NormalLoading)
                , new PropertyMetadata(0.0, OnStrokeValueChanged));

        public static readonly DependencyProperty StrokeArrayProperty =
            DependencyProperty.Register("StrokeArray", typeof(DoubleCollection), typeof(NormalLoading)
                , new PropertyMetadata(new DoubleCollection { 10, 100 }));

        private Storyboard _storyboard;


        public NormalLoading()
        {
            Loaded += LoadingNew_Loaded;
            Unloaded += LoadingNew_Unloaded;
        }

        /// <summary>
        ///     true :动画开始
        ///     false：动画停止
        /// </summary>
        public bool IsAnimation
        {
            get => (bool)GetValue(IsAnimationProperty);
            set => SetValue(IsAnimationProperty, value);
        }

        public double StrokeValue
        {
            get => (double)GetValue(StrokeValueProperty);
            set => SetValue(StrokeValueProperty, value);
        }


        public DoubleCollection StrokeArray
        {
            get => (DoubleCollection)GetValue(StrokeArrayProperty);
            set => SetValue(StrokeArrayProperty, value);
        }

        private static void OnStrokeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalLoading).StrokeArray = new DoubleCollection { (double)e.NewValue, 100 };
        }

        private void LoadingNew_Loaded(object sender, RoutedEventArgs e)
        {
            _storyboard = new Storyboard();
            _storyboard.RepeatBehavior = RepeatBehavior.Forever;
            var animation = new DoubleAnimation(0, Width + 20, new Duration(TimeSpan.FromSeconds(1.0)));
            //animation.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
            //animation.AutoReverse = true;
            _storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(StrokeValueProperty));
            _storyboard.Begin();
        }

        private void LoadingNew_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_storyboard != null)
                _storyboard.Stop();
            IsAnimation = false;
        }
    }
}