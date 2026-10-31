using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.Controls
{
    public class Flame : Control
    {
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(Flame),
                new PropertyMetadata(default(bool)));

        static Flame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flame),
                new FrameworkPropertyMetadata(typeof(Flame)));
        }

        public bool IsStart
        {
            get => (bool)GetValue(IsStartProperty);
            set => SetValue(IsStartProperty, value);
        }
    }
}