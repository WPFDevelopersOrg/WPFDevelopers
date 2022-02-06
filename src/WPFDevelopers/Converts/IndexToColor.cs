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
            var brush = Application.Current.Resources["CircularSingularSolidColorBrush"] as SolidColorBrush; 
            if ((num % 2) == 1)
                brush = Application.Current.Resources["CircularDualSolidColorBrush"] as SolidColorBrush;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DrawPrizeIndexToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = System.Convert.ToInt32(value);
            var brush = Application.Current.Resources["DrawPrizeSingularSolidColorBrush"] as SolidColorBrush;
            if ((num % 2) == 1)
                brush = Application.Current.Resources["DrawPrizeDualSolidColorBrush"] as SolidColorBrush;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
