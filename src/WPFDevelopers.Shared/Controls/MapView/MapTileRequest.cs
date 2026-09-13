namespace WPFDevelopers.Controls
{
    internal sealed class TileRequest
    {
        public TileRequest(int zoom, int tileX, int tileY, string cacheKey, TileConfigSnapshot config)
        {
            Zoom = zoom;
            TileX = tileX;
            TileY = tileY;
            CacheKey = cacheKey;
            Config = config;
        }

        public int Zoom { get; private set; }
        public int TileX { get; private set; }
        public int TileY { get; private set; }
        public string CacheKey { get; private set; }
        public TileConfigSnapshot Config { get; private set; }
    }

    internal sealed class TileConfigSnapshot
    {
        public TileConfigSnapshot(MapTileSource tileSource, int maxConcurrentTileDownloads, int tileSourceVersion)
        {
            TileSource = tileSource;
            MaxConcurrentTileDownloads = maxConcurrentTileDownloads;
            TileSourceVersion = tileSourceVersion;
        }

        public MapTileSource TileSource { get; private set; }
        public int MaxConcurrentTileDownloads { get; private set; }
        public int TileSourceVersion { get; private set; }
    }

    internal sealed class MapItemLayoutInfo
    {
        public MapItemLayoutInfo(object item, double x, double y)
        {
            Item = item;
            X = x;
            Y = y;
        }

        public object Item { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
    }

    internal sealed class PushpinLayout
    {
        public PushpinLayout(Pushpin pushpin, double x, double y)
        {
            Pushpin = pushpin;
            X = x;
            Y = y;
        }

        public Pushpin Pushpin { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
    }

    internal sealed class ClusterAggregate
    {
        public ClusterAggregate(Pushpin representative)
        {
            Representative = representative;
            Items = new System.Collections.Generic.List<Pushpin>();
        }

        public double SumX { get; set; }
        public double SumY { get; set; }
        public int Count { get; set; }
        public Pushpin Representative { get; private set; }
        public System.Collections.Generic.List<Pushpin> Items { get; private set; }
    }

    internal sealed class MapPolylineLayoutInfo
    {
        public MapPolylineLayoutInfo(MapPolyline polyline)
        {
            Polyline = polyline;
        }

        public MapPolyline Polyline { get; private set; }
        public System.Collections.Generic.List<System.Windows.Point> Points { get; } = new System.Collections.Generic.List<System.Windows.Point>();
    }

    internal sealed class MapPolygonLayoutInfo
    {
        public MapPolygonLayoutInfo(MapPolygon polygon)
        {
            Polygon = polygon;
        }

        public MapPolygon Polygon { get; private set; }
        public System.Collections.Generic.List<System.Windows.Point> Points { get; } = new System.Collections.Generic.List<System.Windows.Point>();
    }
}
