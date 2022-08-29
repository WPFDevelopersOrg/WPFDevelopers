using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class SongWords : TextBlock
    {
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan),
                typeof(SongWords), new PropertyMetadata(TimeSpan.FromSeconds(1)));

        public static readonly DependencyProperty StartDurationProperty =
            DependencyProperty.Register("StartDuration", typeof(TimeSpan), typeof(SongWords),
                new PropertyMetadata(TimeSpan.FromSeconds(1)));


        public SongWords()
        {
            NameScope.SetNameScope(this, new NameScope());
            var gradientBrush = new LinearGradientBrush();
            gradientBrush.EndPoint = new Point(1, 0.5);
            gradientBrush.StartPoint = new Point(0, 0.5);
            var stop1 = new GradientStop(Colors.White, 0);
            var stop2 = new GradientStop(Colors.White, 1);
            var stop3 = new GradientStop(Colors.Gray, 1);
            RegisterName("GradientStop1", stop1);
            RegisterName("GradientStop2", stop2);
            RegisterName("GradientStop3", stop3);
            gradientBrush.GradientStops.Add(stop1);
            gradientBrush.GradientStops.Add(stop2);
            gradientBrush.GradientStops.Add(stop3);
            Foreground = gradientBrush;
            Loaded += (s, e) => { Animate(); };
        }

        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public TimeSpan StartDuration
        {
            get => (TimeSpan)GetValue(StartDurationProperty);
            set => SetValue(StartDurationProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void Animate()
        {
            var storyboard = new Storyboard();
            var animation1 = new DoubleAnimation();
            animation1.From = 0;
            animation1.To = 1;
            animation1.Duration = Duration;
            animation1.BeginTime = StartDuration;
            Storyboard.SetTargetName(animation1, "GradientStop2");
            Storyboard.SetTargetProperty(animation1,
                new PropertyPath(GradientStop.OffsetProperty));

            var animation2 = new DoubleAnimation();
            animation2.From = 0;
            animation2.To = 1;
            animation2.Duration = Duration;
            animation2.BeginTime = StartDuration;
            Storyboard.SetTargetName(animation2, "GradientStop3");
            Storyboard.SetTargetProperty(animation2,
                new PropertyPath(GradientStop.OffsetProperty));

            storyboard.Children.Add(animation1);
            storyboard.Children.Add(animation2);
            storyboard.Begin(this);
        }
    }
}