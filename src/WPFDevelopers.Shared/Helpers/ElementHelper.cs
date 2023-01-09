using System.Windows;

namespace WPFDevelopers.Helpers
{
    public class ElementHelper : DependencyObject
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ElementHelper),
                new PropertyMetadata(new CornerRadius(3)));

        public static readonly DependencyProperty IsWatermarkProperty =
            DependencyProperty.RegisterAttached("IsWatermark", typeof(bool), typeof(ElementHelper),
                new PropertyMetadata(false));
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ElementHelper),
                new PropertyMetadata("Please input"));

        public static readonly DependencyProperty IsStripeProperty =
           DependencyProperty.RegisterAttached("IsStripe", typeof(bool), typeof(ElementHelper),
               new PropertyMetadata(false));

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
        public static bool GetIsWatermark(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWatermarkProperty);
        }

        public static void SetIsWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWatermarkProperty, value);
        }

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }
        public static bool GetIsStripe(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsStripeProperty);
        }

        public static void SetIsStripe(DependencyObject obj, bool value)
        {
            obj.SetValue(IsStripeProperty, value);
        }
    }
}