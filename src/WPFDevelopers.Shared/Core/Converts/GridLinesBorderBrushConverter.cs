using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace WPFDevelopers.Converts
{
    public class GridLinesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataGridGridLinesVisibility visibility && parameter is string lineType)
            {
                string lineTypeLower = lineType.ToLower();
                bool shouldShow = visibility == DataGridGridLinesVisibility.All ||
                                 (visibility == DataGridGridLinesVisibility.Horizontal && lineTypeLower == "horizontal") ||
                                 (visibility == DataGridGridLinesVisibility.Vertical && lineTypeLower == "vertical");

                return shouldShow ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}