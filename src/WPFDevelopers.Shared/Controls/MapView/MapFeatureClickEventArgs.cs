using System.Windows;

namespace WPFDevelopers.Controls
{
    public enum MapFeatureType
    {
        None = 0,
        Pushpin = 1,
        Polyline = 2,
        Polygon = 3,
        Circle = 4,
        Rectangle = 5
    }

    public class MapFeatureClickEventArgs : RoutedEventArgs
    {
        public MapFeatureClickEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        public MapFeatureType FeatureType { get; set; }

        public Pushpin ClickedPushpin { get; set; }

        public MapPolyline ClickedPolyline { get; set; }

        public MapPolygon ClickedPolygon { get; set; }

        public MapCircle ClickedCircle { get; set; }

        public MapRectangle ClickedRectangle { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double ScreenX { get; set; }

        public double ScreenY { get; set; }

        public Point ScreenPosition
        {
            get { return new Point(ScreenX, ScreenY); }
        }
    }
}
