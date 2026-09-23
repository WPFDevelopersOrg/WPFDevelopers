using System;
using System.Windows.Media;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class MapCircle : ObservableObject
    {
        private bool _isVisible;
        private double _centerLatitude;
        private double _centerLongitude;
        private double _radiusMeters;
        private int _segmentCount;
        private Brush _fill;
        private Brush _stroke;
        private double _strokeThickness;
        private double _opacity;
        private int _shapeZIndex;

        public MapCircle()
        {
            _isVisible = true;
            _radiusMeters = 300d;
            _segmentCount = 48;
            _fill = new SolidColorBrush(Color.FromArgb(0x55, 0x59, 0xA4, 0xF6));
            _stroke = new SolidColorBrush(Color.FromRgb(0x25, 0x63, 0xEB));
            _strokeThickness = 2d;
            _opacity = 0.8d;
            _shapeZIndex = 0;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value, nameof(IsVisible)); }
        }

        public double CenterLatitude
        {
            get { return _centerLatitude; }
            set
            {
                var lat = value < -85 ? -85 : (value > 85 ? 85 : value);
                SetProperty(ref _centerLatitude, lat, nameof(CenterLatitude));
            }
        }

        public double CenterLongitude
        {
            get { return _centerLongitude; }
            set
            {
                var lon = value;
                while (lon < -180)
                {
                    lon += 360;
                }

                while (lon > 180)
                {
                    lon -= 360;
                }

                SetProperty(ref _centerLongitude, lon, nameof(CenterLongitude));
            }
        }

        public double RadiusMeters
        {
            get { return _radiusMeters; }
            set { SetProperty(ref _radiusMeters, value < 0 ? 0 : value, nameof(RadiusMeters)); }
        }

        public int SegmentCount
        {
            get { return _segmentCount; }
            set
            {
                var segments = value < 12 ? 12 : (value > 180 ? 180 : value);
                SetProperty(ref _segmentCount, segments, nameof(SegmentCount));
            }
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
    }
}
