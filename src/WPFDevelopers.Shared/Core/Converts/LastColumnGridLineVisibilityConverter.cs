using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class LastColumnGridLineVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return Visibility.Collapsed;

            if (values[0] is int displayIndex &&
                values[1] is int columnCount &&
                values[2] is DataGridGridLinesVisibility gridLines)
            {
                if (displayIndex == columnCount)
                    return Visibility.Collapsed;

                return gridLines ==DataGridGridLinesVisibility.Vertical
                       ||
                       gridLines == DataGridGridLinesVisibility.All
                       ? Visibility.Visible
                       : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
