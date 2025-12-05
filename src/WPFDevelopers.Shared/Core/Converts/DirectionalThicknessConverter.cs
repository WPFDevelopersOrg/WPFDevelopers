using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class DirectionalThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                bool isVertical = true;
                if (parameter is string direction)
                    isVertical = direction.ToLower() != "horizontal";
                if (isVertical)
                    return new Thickness(0, thickness.Top, 0, thickness.Bottom);
                else
                    return new Thickness(thickness.Left, 0, thickness.Right, 0);
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
