using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class FireControl : Control
    {
        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
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