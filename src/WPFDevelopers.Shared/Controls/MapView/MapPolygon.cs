using System.Collections.ObjectModel;
using System.Windows.Media;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class MapPolygon : ObservableObject
    {
        private bool _isVisible;
        private ObservableCollection<MapLocation> _points;
        private Brush _fill;
        private Brush _stroke;
        private double _strokeThickness;
        private double _opacity;

        public MapPolygon()
        {
            _isVisible = true;
            _points = new ObservableCollection<MapLocation>();
            _fill = new SolidColorBrush(Color.FromArgb(0x55, 0x22, 0xC5, 0x5E));
            _stroke = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));
            _strokeThickness = 2d;
            _opacity = 0.8d;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value, nameof(IsVisible)); }
        }

        public ObservableCollection<MapLocation> Points
        {
            get { return _points; }
            set
            {
                var next = value ?? new ObservableCollection<MapLocation>();
                SetProperty(ref _points, next, nameof(Points));
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
    }
}
