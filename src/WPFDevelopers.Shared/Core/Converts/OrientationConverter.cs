using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class OrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Orientation orientation)
                return (orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
