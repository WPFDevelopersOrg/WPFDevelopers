using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    internal class AngleToPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var angle = (double)values[0];
            var size = (Size)values[1];
            var radius = (double)size.Height;
            var piang = angle * Math.PI / 180;

            var px = Math.Sin(piang) * radius + radius;
            var py = -Math.Cos(piang) * radius + radius;
            return new Point(px, py);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}