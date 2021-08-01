using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public partial class CircularProgressBar : ProgressBar
    {
        public CircularProgressBar()
        {
            this.ValueChanged += CircularProgressBar_ValueChanged;
        }

        void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CircularProgressBar bar = sender as CircularProgressBar;
            double currentAngle = bar.Angle;
            double targetAngle = e.NewValue / bar.Maximum * 359.999;
            DoubleAnimation anim = new DoubleAnimation(currentAngle, targetAngle, TimeSpan.FromMilliseconds(500));
            bar.BeginAnimation(CircularProgressBar.AngleProperty, anim, HandoffBehavior.SnapshotAndReplace);
        }

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(0.0));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(10.0));

        public double BrushStrokeThickness
        {
            get { return (double)GetValue(BrushStrokeThicknessProperty); }
            set { SetValue(BrushStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty BrushStrokeThicknessProperty =
            DependencyProperty.Register("BrushStrokeThickness", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(1.0));
    }
}
