using System;
using System.Windows.Media;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class MapRectangle : ObservableObject
    {
        private bool _isVisible;
        private double _minLatitude;
        private double _minLongitude;
        private double _maxLatitude;
        private double _maxLongitude;
        private Brush _fill;
        private Brush _stroke;
        private double _strokeThickness;
        private double _opacity;
        private int _shapeZIndex;

        public MapRectangle()
        {
            _isVisible = true;
            _fill = new SolidColorBrush(Color.FromArgb(0x44, 0x16, 0xA3, 0x4A));
            _stroke = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));
            _strokeThickness = 2d;
            _opacity = 0.85d;
            _shapeZIndex = 0;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value, nameof(IsVisible)); }
        }

        public double MinLatitude
        {
            get { return _minLatitude; }
            set { SetProperty(ref _minLatitude, ClampLatitude(value), nameof(MinLatitude)); }
        }

        public double MinLongitude
        {
            get { return _minLongitude; }
            set { SetProperty(ref _minLongitude, NormalizeLongitude(value), nameof(MinLongitude)); }
        }

        public double MaxLatitude
        {
            get { return _maxLatitude; }
            set { SetProperty(ref _maxLatitude, ClampLatitude(value), nameof(MaxLatitude)); }
        }

        public double MaxLongitude
        {
            get { return _maxLongitude; }
            set { SetProperty(ref _maxLongitude, NormalizeLongitude(value), nameof(MaxLongitude)); }
        }

        public Brush Fill
        {
            get { return _fill; }
            set { SetProperty(ref _fill, value, nameof(Fill)); }
        }

        public Brush Stroke
        {
            get { return _stroke; }
            set { SetProperty(ref _stroke, value, nameof(Stroke)); }
        }

        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set { SetProperty(ref _strokeThickness, value < 0 ? 0 : value, nameof(StrokeThickness)); }
        }

        public double Opacity
        {
            get { return _opacity; }
            set
            {
                var bounded = value < 0 ? 0 : (value > 1 ? 1 : value);
                SetProperty(ref _opacity, bounded, nameof(Opacity));
            }
        }

        public int ShapeZIndex
        {
            get { return _shapeZIndex; }
            set { SetProperty(ref _shapeZIndex, value, nameof(ShapeZIndex)); }
        }

        private static double ClampLatitude(double latitude)
        {
            return latitude < -85 ? -85 : (latitude > 85 ? 85 : latitude);
        }

        private static double NormalizeLongitude(double longitude)
        {
            var lon = longitude;
            while (lon < -180)
            {
                lon += 360;
            }

            while (lon > 180)
            {
                lon -= 360;
            }

            return lon;
        }
    }
}
