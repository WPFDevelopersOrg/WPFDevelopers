using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class FirstLastMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is ItemsControl itemsControl && values[1] is int index)
            {
                var count = itemsControl.Items.Count;
                var left = index == 0 ? 0 : 4;
                var right = index == count - 1 ? 0 : 4;
                return new Thickness(left, 0, right, 0);
            }
            return new Thickness(4, 0, 4, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
