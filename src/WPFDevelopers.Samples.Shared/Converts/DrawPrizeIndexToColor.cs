using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFDevelopers.Samples.Converts
{
    public class DrawPrizeIndexToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = System.Convert.ToInt32(value);
            var brush = (SolidColorBrush)Application.Current?.TryFindResource("WD.DrawPrizeSingularSolidColorBrush");
            if (num % 2 == 1)
                brush = (SolidColorBrush)Application.Current?.TryFindResource("WD.DrawPrizeDualSolidColorBrush");
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
