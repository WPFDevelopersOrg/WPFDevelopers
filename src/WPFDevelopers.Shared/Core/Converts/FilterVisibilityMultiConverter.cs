using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class FilterVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Visibility.Collapsed;

            var column = values[0] as DataGridColumn;
            if (column == null) return Visibility.Collapsed;
            var filterEngine = values[1];
            bool isValidColumn = !string.IsNullOrEmpty(column.Header?.ToString())
                                 || column is DataGridBoundColumn
                                 || (column is DataGridTemplateColumn templateColumn && templateColumn.CellTemplate != null);

            bool hasFilterEngine = filterEngine != null;
            if (isValidColumn && hasFilterEngine)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
