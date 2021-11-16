using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Controls
{
    public class IsArrangedToScaleConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return object.Equals(true, value) ? 1 : 0;
        }
    }
}
