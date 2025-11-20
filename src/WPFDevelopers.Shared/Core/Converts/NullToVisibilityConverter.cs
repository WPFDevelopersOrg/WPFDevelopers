using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            bool paramInverse = false;
            if (parameter != null)
            {
                if (parameter is bool pb)
                {
                    paramInverse = pb;
                }
                else
                {
                    var s = parameter.ToString();
                    if (string.Equals(s, "Inverse", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(s, "True", StringComparison.OrdinalIgnoreCase))
                    {
                        paramInverse = true;
                    }
                }
            }

            bool final = isNull; 
            if (Inverse) final = !final;
            if (paramInverse) final = !final;

            return final ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}