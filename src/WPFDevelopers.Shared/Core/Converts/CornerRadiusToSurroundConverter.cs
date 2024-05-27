using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Converts
{
    public class CornerRadiusToSurroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CornerRadius cornerRadius && parameter != null)
            {
                if (cornerRadius != new CornerRadius(0))
                {
                    CornerRadius _cornerRadius = cornerRadius;
                    RadiusOrientation _radiusOrientation;
                    if (Enum.TryParse(parameter.ToString(), out _radiusOrientation))
                    {
                        switch (_radiusOrientation)
                        {
                            case RadiusOrientation.Down:
                                _cornerRadius.BottomRight = 0;
                                _cornerRadius.BottomLeft = 0;
                                break;
                            case RadiusOrientation.Up:
                                _cornerRadius.TopLeft = 0;
                                _cornerRadius.TopRight = 0;
                                break;
                            case RadiusOrientation.Left:
                                _cornerRadius.TopLeft = 0;
                                _cornerRadius.BottomLeft = 0;
                                break;
                            case RadiusOrientation.Right:
                                _cornerRadius.TopRight = 0;
                                _cornerRadius.BottomRight = 0;
                                break;
                            case RadiusOrientation.TopLeft:
                                _cornerRadius.TopRight = 0;
                                _cornerRadius.BottomRight = 0;
                                _cornerRadius.BottomLeft = 0;
                                break;
                            case RadiusOrientation.TopRight:
                                _cornerRadius.TopLeft = 0;
                                _cornerRadius.BottomRight = 0;
                                _cornerRadius.BottomLeft = 0;
                                break;
                        }
                    }
                    return _cornerRadius;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
