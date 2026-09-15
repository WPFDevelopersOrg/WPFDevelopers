using System.Collections.ObjectModel;
using System.Windows.Media;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class MapPolyline : ObservableObject
    {
        private bool _isVisible;
        private ObservableCollection<MapLocation> _points;
        private Brush _stroke;
        private double _strokeThickness;
        private double _opacity;
        private bool _isClosed;

        public MapPolyline()
        {
            _isVisible = true;
            _points = new ObservableCollection<MapLocation>();
            _stroke = new SolidColorBrush(Color.FromRgb(0x22, 0xC5, 0x5E));
            _strokeThickness = 3d;
            _opacity = 0.9d;
            _isClosed = false;
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

        public bool IsClosed
        {
            get { return _isClosed; }
            set { SetProperty(ref _isClosed, value, nameof(IsClosed)); }
        }
    }
}
