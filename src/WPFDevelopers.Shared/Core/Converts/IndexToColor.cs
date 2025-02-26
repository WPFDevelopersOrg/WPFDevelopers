using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFDevelopers.Converts
{
    public class IndexToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = System.Convert.ToInt32(value);
            var brush = (SolidColorBrush)Application.Current?.TryFindResource("WD.CircleMenuBrush");
            if (num % 2 == 1)
                brush = (SolidColorBrush)Application.Current?.TryFindResource("WD.CircleMenuDualBrush");
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}