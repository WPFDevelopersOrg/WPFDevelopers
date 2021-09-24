using System.Windows;

namespace WPFDevelopers.Helpers
{
    public class ControlsHelper: DependencyObject
    {
        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlsHelper), new PropertyMetadata(new CornerRadius(4)));
    }
}
