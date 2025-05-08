using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    internal class WidthHeightEqualValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var width = (double) values[0];
            var height = (double) values[1];
            if (width == 0 && height == 0)
                return 0;
            var max = Math.Max(width, height);
            return max;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}