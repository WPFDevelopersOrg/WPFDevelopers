namespace WPFDevelopers.Controls
{
    public sealed class MapGeoPoint
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public MapCoordinateType CoordinateType { get; set; }

        public MapGeoPoint()
        {
        }

        public MapGeoPoint(double longitude, double latitude, MapCoordinateType coordinateType)
        {
            Longitude = longitude;
            Latitude = latitude;
            CoordinateType = coordinateType;
        }

        public override string ToString()
        {
            return $"({Longitude}, {Latitude}) [{CoordinateType}]";
        }
    }
}
