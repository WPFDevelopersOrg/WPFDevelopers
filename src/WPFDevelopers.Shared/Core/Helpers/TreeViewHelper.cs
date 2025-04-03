using System.Windows;

namespace WPFDevelopers.Helpers
{
    public class TreeViewHelper
    {
        public static readonly DependencyProperty IsScrollAnimationProperty =
            DependencyProperty.RegisterAttached("IsScrollAnimation", typeof(bool), typeof(TreeViewHelper),
                new FrameworkPropertyMetadata(false));

        public static object GetIsScrollAnimation(DependencyObject obj)
        {
            return obj.GetValue(IsScrollAnimationProperty);
        }

        public static void SetIsScrollAnimation(DependencyObject obj, object value)
        {
            obj.SetValue(IsScrollAnimationProperty, value);
        }
    }
}
