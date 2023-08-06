using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Converts
{
    public class ColorToRedConverter : IValueConverter
    {
        private Color? _curColor = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _curColor = (Color)value;

            return _curColor.Value.R;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.FromArgb(_curColor.Value.A, (byte)(double.Parse(value.ToString())), _curColor.Value.G, _curColor.Value.B);
        }
    }

    public class ColorToGreenConverter : IValueConverter
    {
        private Color? _curColor = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _curColor = (Color)value;

            return _curColor.Value.G;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.FromArgb(_curColor.Value.A, _curColor.Value.R, (byte)(double.Parse(value.ToString())), _curColor.Value.B);
        }
    }

    public class ColorToBlueConverter : IValueConverter
    {
        private Color? _curColor = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _curColor = (Color)value;

            return _curColor.Value.B;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.FromArgb(_curColor.Value.A, _curColor.Value.R, _curColor.Value.G, (byte)(double.Parse(value.ToString())));
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorToStringConverter : IValueConverter
    {
        private Color? _curColor = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _curColor = (Color)value;

            return _curColor.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorStr = (string)value;

            if (!string.IsNullOrWhiteSpace(colorStr) && Regex.IsMatch(colorStr, @"^#[\da-fA-F]{6,8}$"))
               return ColorConverter.ConvertFromString(colorStr);

            return _curColor.Value;
        }

    }

    public class HToColorConverter : IValueConverter
    {
        private Color? _curColor = null;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _curColor = (Color)value;
            return $"{ColorUtil.ColorFromH(_curColor.Value)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorStr = (string)value;
            if (!string.IsNullOrWhiteSpace(colorStr) && double.TryParse(colorStr, out double hValue))
                _curColor = ColorUtil.ConvertHSLToColor(_curColor.Value, hValue: hValue % 360);
            return _curColor;
        }
    }

    public class SToColorConverter : IValueConverter
    {
        private Color? _curColor = null;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _curColor = (Color)value;
            return $"{ColorUtil.ColorFromS(_curColor.Value)}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorStr = (string)value;
            if (!string.IsNullOrWhiteSpace(colorStr) && double.TryParse(colorStr, out double sValue))
                _curColor = ColorUtil.ConvertHSLToColor(_curColor.Value, sValue: sValue / 100);
            return _curColor;
        }
    }

    public class LToColorConverter : IValueConverter
    {
        private Color? _curColor = null;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _curColor = (Color)value;
            return $"{ColorUtil.ColorFromL(_curColor.Value)}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorStr = (string)value;
            if (!string.IsNullOrWhiteSpace(colorStr) && double.TryParse(colorStr, out double lValue))
                _curColor = ColorUtil.ConvertHSLToColor(_curColor.Value, lValue: lValue / 100);
            return _curColor;
        }
    }

}
