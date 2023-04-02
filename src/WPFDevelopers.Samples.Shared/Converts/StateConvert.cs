using System;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFDevelopers.Sample.Converts
{
    public class StateConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            var color = Brushes.Green;
            if (value != null)
            {
                var state = int.Parse(value.ToString());
                switch (state)
                {
                    case 0:
                        color = Brushes.Green;
                        break;
                    case 1:
                        color = Brushes.Orange;
                        break;
                    case 2:
                        color = Brushes.Red;
                        break;
                }
            }

            return color;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
