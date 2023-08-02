using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class BreadCrumbBarConvertr : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var item = values[0] as ListBoxItem;
            var listBox = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            if (listBox == null) return Visibility.Collapsed;
            var arrayIndex = listBox.ItemContainerGenerator.IndexFromContainer(item);
            if (arrayIndex == 0)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
