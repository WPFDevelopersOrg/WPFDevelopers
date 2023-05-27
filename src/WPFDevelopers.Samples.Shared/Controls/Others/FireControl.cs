using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.Controls
{
    public class FireControl : Control
    {
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(FireControl),
                new PropertyMetadata(default(bool)));

        static FireControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FireControl),
                new FrameworkPropertyMetadata(typeof(FireControl)));
        }

        public bool IsStart
        {
            get => (bool)GetValue(IsStartProperty);
            set => SetValue(IsStartProperty, value);
        }
    }
}