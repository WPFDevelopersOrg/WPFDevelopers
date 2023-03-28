using Microsoft.Maps.MapControl.WPF;
using System;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class AMapTitleLayer : MapTileLayer
    {
        public AMapTitleLayer()
        {
            TileSource = new AMapTileSource();
        }

        public string UriFormat
        {
            get { return TileSource.UriFormat; }
            set { TileSource.UriFormat = value; }
        }
    }
    public class AMapTileSource : TileSource
    {
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            string url = string.Format("http://wprd01.is.autonavi.com/appmaptile?x={0}&y={1}&z={2}&lang=zh_cn&size=1&scl=1&style=7", x, y, zoomLevel);
            return new Uri(url, UriKind.Absolute);
        }
    }
}
