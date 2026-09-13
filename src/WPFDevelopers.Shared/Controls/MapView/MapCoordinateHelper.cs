using System;

namespace WPFDevelopers.Controls
{
    public static class MapCoordinateHelper
    {
        private const double EarthRadius = 6378245.0;
        private const double EE = 0.006693421622965943;

        public static MapGeoPoint Wgs84ToGcj02(double lon, double lat)
        {
            var point = TransformWgs84ToGcj02(lon, lat);
            return new MapGeoPoint(point.Item1, point.Item2, MapCoordinateType.Gcj02);
        }

        public static MapGeoPoint Gcj02ToWgs84(double lon, double lat)
        {
            var point = TransformGcj02ToWgs84(lon, lat);
            return new MapGeoPoint(point.Item1, point.Item2, MapCoordinateType.Wgs84);
        }

        public static MapGeoPoint Wgs84ToBd09(double lon, double lat)
        {
            var gcj = Wgs84ToGcj02(lon, lat);
            return Gcj02ToBd09(gcj.Longitude, gcj.Latitude);
        }

        public static MapGeoPoint Bd09ToWgs84(double lon, double lat)
        {
            var gcj = Bd09ToGcj02(lon, lat);
            return Gcj02ToWgs84(gcj.Longitude, gcj.Latitude);
        }

        public static MapGeoPoint Gcj02ToBd09(double lon, double lat)
        {
            var x = lon;
            var y = lat;
            var z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * Math.PI);
            var theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * Math.PI);
            var bdLon = z * Math.Cos(theta) + 0.0065;
            var bdLat = z * Math.Sin(theta) + 0.006;

            return new MapGeoPoint(bdLon, bdLat, MapCoordinateType.Bd09);
        }

        public static MapGeoPoint Bd09ToGcj02(double lon, double lat)
        {
            var x = lon - 0.0065;
            var y = lat - 0.006;
            var z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * Math.PI);
            var theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * Math.PI);
            var gcjLon = z * Math.Cos(theta);
            var gcjLat = z * Math.Sin(theta);

            return new MapGeoPoint(gcjLon, gcjLat, MapCoordinateType.Gcj02);
        }

        public static MapGeoPoint Convert(MapGeoPoint point, MapCoordinateType targetType)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (point.CoordinateType == targetType)
            {
                return new MapGeoPoint(point.Longitude, point.Latitude, targetType);
            }

            switch (point.CoordinateType)
            {
                case MapCoordinateType.Wgs84:
                    if (targetType == MapCoordinateType.Gcj02)
                    {
                        return Wgs84ToGcj02(point.Longitude, point.Latitude);
                    }

                    if (targetType == MapCoordinateType.Bd09)
                    {
                        return Wgs84ToBd09(point.Longitude, point.Latitude);
                    }
                    break;

                case MapCoordinateType.Gcj02:
                    if (targetType == MapCoordinateType.Wgs84)
                    {
                        return Gcj02ToWgs84(point.Longitude, point.Latitude);
                    }

                    if (targetType == MapCoordinateType.Bd09)
                    {
                        return Gcj02ToBd09(point.Longitude, point.Latitude);
                    }
                    break;

                case MapCoordinateType.Bd09:
                    if (targetType == MapCoordinateType.Wgs84)
                    {
                        return Bd09ToWgs84(point.Longitude, point.Latitude);
                    }

                    if (targetType == MapCoordinateType.Gcj02)
                    {
                        return Bd09ToGcj02(point.Longitude, point.Latitude);
                    }
                    break;
            }

            return new MapGeoPoint(point.Longitude, point.Latitude, targetType);
        }

        private static Tuple<double, double> TransformWgs84ToGcj02(double lon, double lat)
        {
            var dLat = TransformLat(lon - 105.0, lat - 35.0);
            var dLon = TransformLon(lon - 105.0, lat - 35.0);

            var radLat = lat * Math.PI / 180.0;
            var magic = Math.Sin(radLat);
            magic = 1 - EE * magic * magic;
            var sqrtMagic = Math.Sqrt(magic);

            dLat = (dLat * 180.0) / ((EarthRadius * (1 - EE)) / (sqrtMagic * 2) * Math.PI);
            dLon = (dLon * 180.0) / ((EarthRadius * (1 - EE)) / (sqrtMagic * 2) * Math.PI);

            var gcjLat = lat + dLat;
            var gcjLon = lon + dLon;

            return new Tuple<double, double>(gcjLon, gcjLat);
        }

        private static Tuple<double, double> TransformGcj02ToWgs84(double lon, double lat)
        {
            var dLat = TransformLat(lon - 105.0, lat - 35.0);
            var dLon = TransformLon(lon - 105.0, lat - 35.0);

            var radLat = lat * Math.PI / 180.0;
            var magic = Math.Sin(radLat);
            magic = 1 - EE * magic * magic;
            var sqrtMagic = Math.Sqrt(magic);

            dLat = (dLat * 180.0) / ((EarthRadius * (1 - EE)) / (sqrtMagic * 2) * Math.PI);
            dLon = (dLon * 180.0) / ((EarthRadius * (1 - EE)) / (sqrtMagic * 2) * Math.PI);

            var wgsLat = lat - dLat;
            var wgsLon = lon - dLon;

            return new Tuple<double, double>(wgsLon, wgsLat);
        }

        private static double TransformLat(double x, double y)
        {
            var ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * Math.PI) + 40.0 * Math.Sin(y / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * Math.PI) + 320.0 * Math.Sin(y * Math.PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        private static double TransformLon(double x, double y)
        {
            var ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * Math.PI) + 40.0 * Math.Sin(x / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * Math.PI) + 300.0 * Math.Sin(x * Math.PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }
    }
}
