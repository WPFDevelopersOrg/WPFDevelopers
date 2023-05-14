using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class EdgeLight : ContentControl
    {
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(EdgeLight), new PropertyMetadata(false));

        public static readonly DependencyProperty LineSizeProperty =
            DependencyProperty.Register("LineSize", typeof(double), typeof(EdgeLight), new PropertyMetadata(1.0d));
        static EdgeLight()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EdgeLight),
                new FrameworkPropertyMetadata(typeof(EdgeLight)));
        }
        public bool IsAnimation
        {
            get => (bool)GetValue(IsAnimationProperty);
            set => SetValue(IsAnimationProperty, value);
        }
        public double LineSize
        {
            get => (double)GetValue(LineSizeProperty);
            set => SetValue(LineSizeProperty, value);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}