using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class CircularProgressBar : ProgressBar
    {
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularProgressBar),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBar),
                new PropertyMetadata(10.0));

        public static readonly DependencyProperty BrushStrokeThicknessProperty =
            DependencyProperty.Register("BrushStrokeThickness", typeof(double), typeof(CircularProgressBar),
                new PropertyMetadata(1.0));

        public CircularProgressBar()
        {
            ValueChanged += CircularProgressBar_ValueChanged;
        }

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public double BrushStrokeThickness
        {
            get => (double)GetValue(BrushStrokeThicknessProperty);
            set => SetValue(BrushStrokeThicknessProperty, value);
        }

        private void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var bar = sender as CircularProgressBar;
            var currentAngle = bar.Angle;
            var targetAngle = e.NewValue / bar.Maximum * 359.999;
            var anim = new DoubleAnimation(currentAngle, targetAngle, TimeSpan.FromMilliseconds(500));
            bar.BeginAnimation(AngleProperty, anim, HandoffBehavior.SnapshotAndReplace);
        }
    }
}