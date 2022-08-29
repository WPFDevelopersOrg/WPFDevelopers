using System.Windows;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class RadarModel : ViewModelBase
    {
        private Point _pointValue;

        private int _valueMax;
        public string Text { get; set; }

        public int ValueMax
        {
            get => _valueMax;
            set
            {
                _valueMax = value;
                NotifyPropertyChange("ValueMax");
            }
        }

        public Point PointValue
        {
            get => _pointValue;
            set
            {
                _pointValue = value;
                NotifyPropertyChange("PointValue");
            }
        }
    }
}