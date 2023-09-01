using Microsoft.Maps.MapControl.WPF;
using System;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class AMapTitleLayer : MapTileLayer
    {
        private AMapTileSource tileSource;
        public AMapTitleLayer()
        {
            tileSource = new AMapTileSource();
            TileSource = tileSource;
        }

        public string UriFormat
        {
            get { return TileSource.UriFormat; }
            set { TileSource.UriFormat = value; }
        }
        public void UpdateTileSourceStyle(int style)
        {
            tileSource.UpdateStyle(style);
        }
    }
    public class AMapTileSource : TileSource
    {
        private int style = 7;
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            string url = string.Format("http://wprd01.is.autonavi.com/appmaptile?x={0}&y={1}&z={2}&lang=zh_cn&size=1&scl=1&style={3}", x, y, zoomLevel, style);
            return new Uri(url, UriKind.Absolute);
        }
        public void UpdateStyle(int newStyle)
        {
            if (newStyle == style) return;
            style = newStyle;
        }
    }
}
