using System;
using System.Windows;

namespace WPFDevelopers.Helpers
{
    public partial class ControlsHelper: DependencyObject
    {
        /// <summary>
        /// Get angle
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double CalculeAngle(Point start, Point end)
        {
            var radian = Math.Atan2((end.Y - start.Y), (end.X - start.X));
            var angle = (radian * (180 / Math.PI) + 360) % 360;
            return angle;
        }
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
