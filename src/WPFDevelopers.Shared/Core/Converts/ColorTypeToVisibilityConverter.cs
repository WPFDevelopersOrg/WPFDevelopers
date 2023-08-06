using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class ColorTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue && parameter is Enum targetEnumValue)
            {
                if (enumValue.Equals(targetEnumValue))
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
