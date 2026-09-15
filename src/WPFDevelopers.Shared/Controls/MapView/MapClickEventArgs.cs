using System.Windows;

namespace WPFDevelopers.Controls
{
    public class MapClickEventArgs : RoutedEventArgs
    {
        public MapClickEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double ScreenX { get; set; }

        public double ScreenY { get; set; }

        public bool IsEmptyAreaClick { get; set; }

        public Pushpin ClickedPushpin { get; set; }

        public Point ScreenPosition => new Point(ScreenX, ScreenY);
    }
}
