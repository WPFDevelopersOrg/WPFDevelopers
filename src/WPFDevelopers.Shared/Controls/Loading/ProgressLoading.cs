using System.Windows;

namespace WPFDevelopers.Controls
{
    public class ProgressLoading : DefaultLoading
    {
        public static ProgressLoading Progress = new ProgressLoading();
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ProgressLoading),
                new PropertyMetadata(0d));
    }
}
