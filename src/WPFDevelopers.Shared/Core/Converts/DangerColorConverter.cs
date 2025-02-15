using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFDevelopers.Converts
{
    public class DangerColorConverter : IValueConverter
    {
        private Dictionary<Tuple<Color, double>, SolidColorBrush> _cacheBrush = new Dictionary<Tuple<Color, double>, SolidColorBrush>();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush dangerBrush)
            {
                double brightnessIncrement = 0.5;
                if (parameter != null && double.TryParse(parameter.ToString(), out double parsedValue))
                {
                    brightnessIncrement = parsedValue;
                }
                var cacheKey = Tuple.Create(dangerBrush.Color, brightnessIncrement);
                if (_cacheBrush.ContainsKey(cacheKey))
                {
                    return _cacheBrush[cacheKey];
                }
                else
                {
                    var newColor = dangerBrush.Color.AddBrightness(brightnessIncrement);
                    var newBrush = new SolidColorBrush(newColor);
                    newBrush.Freeze();
                    _cacheBrush[cacheKey] = newBrush;
                    return newBrush;
                }
                //var newColor = dangerBrush.Color.AddBrightness(brightnessIncrement);
                //var newBrush = new SolidColorBrush(newColor);
                //newBrush.Freeze();
                //return newBrush;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public static class ColorExtensions
    {
        public static Color AddBrightness(this Color color, double factor)
        {
            factor = Math.Max(0, Math.Min(factor, 1));
            var r = (byte)Math.Min(color.R + factor * 255, 255);
            var g = (byte)Math.Min(color.G + factor * 255, 255);
            var b = (byte)Math.Min(color.B + factor * 255, 255);
            return Color.FromArgb(color.A, r, g, b);
        }
    }
}
