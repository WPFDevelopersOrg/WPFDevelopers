using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class SelectedIndexToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is int altIndex && values[1] is int selectedIndex)
                return altIndex == selectedIndex;
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
