using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    internal class AngleToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var angle = (double)value;
            double radius = 50;
            var piang = angle * Math.PI / 180;

            var px = Math.Sin(piang) * radius + radius;
            var py = -Math.Cos(piang) * radius + radius;
            return new Point(px, py);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}