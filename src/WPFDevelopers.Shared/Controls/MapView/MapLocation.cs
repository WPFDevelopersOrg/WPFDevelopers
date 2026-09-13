using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class MapLocation : ObservableObject
    {
        private double _latitude;
        private double _longitude;

        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value, nameof(Latitude)); }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value, nameof(Longitude)); }
        }
    }
}
