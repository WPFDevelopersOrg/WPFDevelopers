using System;
using System.Windows;
using System.Windows.Data;

namespace WpfCircularProgressBar.Converters
{
    class AngleToPointConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double angle = (double)value;
            double radius = 50;
            double piang = angle * Math.PI / 180;

            double px = Math.Sin(piang) * radius + radius;
            double py = -Math.Cos(piang) * radius + radius;
            return new Point(px, py);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
