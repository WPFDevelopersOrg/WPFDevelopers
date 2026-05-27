using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Converts
{
    public class FilterVisibilityMultiConverter : IMultiValueConverter
    {
        private const int VALUES_LENGTH_MIN = 3;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null)
                throw new ArgumentException(
                    $"{nameof(FilterVisibilityMultiConverter)}.{nameof(Convert)} expects values to be not null.");
            if (values.Length < VALUES_LENGTH_MIN)
                throw new ArgumentException($"{nameof(FilterVisibilityMultiConverter)}.{nameof(Convert)} expects values to have at least {VALUES_LENGTH_MIN} values but got {values.Length}.");

            // values.Count >= 4 means FilterVersion is included (values[3]) - used for binding refresh
            // The actual version value is not used in logic, but its change triggers re-evaluation

            var column = values[0] as DataGridColumn;
            if (column == null || !(values[1] is IFilterEngine filterEngine))
            {
                return Visibility.Collapsed;
            }

            bool isValidColumn = !string.IsNullOrEmpty(column.Header?.ToString())
                                 || column is DataGridBoundColumn
                                 || (column is DataGridTemplateColumn templateColumn && templateColumn.CellTemplate != null);
            if (!isValidColumn)
            {
                return Visibility.Collapsed;
            }

            if (values[2] is bool mouseOver && mouseOver)
            {
                return Visibility.Visible;
            }

            string headerLiteral = column.Header.ToString();
            if (filterEngine.GetFilterValues(headerLiteral) != null)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
