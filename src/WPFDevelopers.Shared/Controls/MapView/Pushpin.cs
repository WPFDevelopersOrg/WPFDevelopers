using System;
using System.Globalization;
using System.Windows;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class Pushpin : ObservableObject
    {
        private string _location;
        private double _latitude;
        private double _longitude;
        private Point _anchorPoint = new Point(0.5, 0.5);
        private string _title;
        private object _tag;
        private DataTemplate _template;
        private bool _isSelected;
        private bool _isActivated;

        public double Latitude
        {
            get { return _latitude; }
            set
            {
                if (SetProperty(ref _latitude, value, nameof(Latitude)))
                {
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public double Longitude
        {
            get { return _longitude; }
            set
            {
                if (SetProperty(ref _longitude, value, nameof(Longitude)))
                {
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, nameof(Title)); }
        }

        public object Tag
        {
            get { return _tag; }
            set { SetProperty(ref _tag, value, nameof(Tag)); }
        }

        public DataTemplate Template
        {
            get { return _template; }
            set { SetProperty(ref _template, value, nameof(Template)); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value, nameof(IsSelected)); }
        }

        public bool IsActivated
        {
            get { return _isActivated; }
            set { SetProperty(ref _isActivated, value, nameof(IsActivated)); }
        }

        public Point AnchorPoint
        {
            get { return _anchorPoint; }
            set
            {
                var normalized = new Point(
                    Clamp01(value.X),
                    Clamp01(value.Y));
                SetProperty(ref _anchorPoint, normalized, nameof(AnchorPoint));
            }
        }

        public string Location
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_location)
                    ? _location
                    : string.Format(CultureInfo.InvariantCulture, "{0},{1}", Latitude, Longitude);
            }
            set
            {
                _location = value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                var parts = value.Split(',');
                if (parts.Length != 2)
                {
                    return;
                }

                if (TryParseCoordinate(parts[0], out var latitude))
                {
                    Latitude = latitude;
                }

                if (TryParseCoordinate(parts[1], out var longitude))
                {
                    Longitude = longitude;
                }

                OnPropertyChanged(nameof(Location));
            }
        }

        private static bool TryParseCoordinate(string rawValue, out double result)
        {
            return double.TryParse(rawValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)
                || double.TryParse(rawValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out result);
        }

        private static double Clamp01(double value)
        {
            return value < 0 ? 0 : (value > 1 ? 1 : value);
        }
    }
}
