using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class PieSegmentModel : ViewModelBase
    {
        private Brush _color;
        private Brush _colorStroke = Brushes.White;
        private double _endAngle;

        private string _name;


        private double _startAngle;
        private double _value;

        public Brush ColorStroke
        {
            get => _colorStroke;
            set
            {
                _colorStroke = value;
                NotifyPropertyChange("ColorStroke");
            }
        }

        public Brush Color
        {
            get => _color;
            set
            {
                _color = value;
                NotifyPropertyChange("Color");
            }
        }

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyPropertyChange("Value");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChange("Name");
            }
        }

        public double StartAngle
        {
            get => _startAngle;
            set
            {
                _startAngle = value;
                NotifyPropertyChange("StartAngle");
            }
        }

        public double EndAngle
        {
            get => _endAngle;
            set
            {
                _endAngle = value;
                NotifyPropertyChange("EndAngle");
            }
        }
    }
}