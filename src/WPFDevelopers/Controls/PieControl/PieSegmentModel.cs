using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class PieSegmentModel:ViewModelBase
    {
        private Brush _colorStroke = Brushes.White;

        public Brush ColorStroke
        {
            get { return _colorStroke; }
            set
            {
                _colorStroke = value;
                NotifyPropertyChange("ColorStroke");
            }
        }
        private Brush _color;

        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                NotifyPropertyChange("Color");
            }
        }
        private double _value;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                NotifyPropertyChange( "Value");
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChange("Name");
            }
        }


        private double _startAngle;
        public double StartAngle
        {
            get
            {
                return _startAngle;
            }
            set
            {
                _startAngle = value;
                NotifyPropertyChange("StartAngle");
            }
        }
        private double _endAngle;
        public double EndAngle
        {
            get
            {
                return _endAngle;
            }
            set
            {
                _endAngle = value;
                NotifyPropertyChange("EndAngle");
            }
        }
    }
}
