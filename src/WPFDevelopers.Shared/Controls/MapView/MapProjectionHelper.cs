using System;

namespace WPFDevelopers.Controls
{
    public static class MapProjectionHelper
    {
        private const double WebMercatorMax = 20037508.3427892;

        public static Tuple<double, double> Wgs84ToWebMercator(double lon, double lat)
        {
            var x = lon * WebMercatorMax / 180.0;
            var y = Math.Log(Math.Tan((90.0 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            y = y * WebMercatorMax / 180.0;
            return new Tuple<double, double>(x, y);
        }

        public static Tuple<double, double> WebMercatorToWgs84(double x, double y)
        {
            var lon = (x / WebMercatorMax) * 180.0;
            var lat = (y / WebMercatorMax) * 180.0;

            lat = 180.0 / Math.PI * (2.0 * Math.Atan(Math.Exp(lat * Math.PI / 180.0)) - Math.PI / 2.0);
            return new Tuple<double, double>(lon, lat);
        }

        public static Tuple<int, int> LonLatToTileXY(double lon, double lat, int zoom)
        {
            var x = (int)((lon + 180.0) / 360.0 * (1 << zoom));
            var sinLat = Math.Sin(lat * Math.PI / 180.0);
            var y = (int)((0.5 - Math.Log((1.0 + sinLat) / (1.0 - sinLat)) / (4.0 * Math.PI)) * (1 << zoom));
            return new Tuple<int, int>(x, y);
        }

        public static Tuple<double, double> TileXYToLonLat(int x, int y, int zoom)
        {
            var n = 1 << zoom;
            var lon = x / (double)n * 360.0 - 180.0;
            var latRad = Math.Atan(Math.Sinh(Math.PI * (1 - 2.0 * y / n)));
            var lat = latRad * 180.0 / Math.PI;
            return new Tuple<double, double>(lon, lat);
        }
    }
}
