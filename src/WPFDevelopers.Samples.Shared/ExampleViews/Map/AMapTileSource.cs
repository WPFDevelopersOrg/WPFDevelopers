using System;
using System.Globalization;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public sealed class AMapTileSource : MapTileSource
    {
        private int _style = 7;

        public override int MinZoomLevel
        {
            get { return 4; }
        }

        public AMapTileSource(int style = 7)
        {
            _style = style;
        }

        public int Style
        {
            get { return _style; }
            set { _style = value; }
        }

        public override Uri GetTileUri(int x, int y, int zoomLevel)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "http://wprd01.is.autonavi.com/appmaptile?x={0}&y={1}&z={2}&lang=zh_cn&size=1&scl=1&style={3}",
                x,
                y,
                zoomLevel,
                _style);

            return new Uri(url, UriKind.Absolute);
        }
    }
}
