using System;
using System.Globalization;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public sealed class TencentTileSource : MapTileSource
    {
        private readonly int _style;
        private readonly string _key;

        public TencentTileSource(int style = 7, string key = "")
        {
            _style = style;
            _key = key ?? string.Empty;
        }

        public int Style
        {
            get { return _style; }
        }

        public string Key
        {
            get { return _key; }
        }

        public override Uri GetTileUri(int x, int y, int zoomLevel)
        {
            var server = ((x + y + zoomLevel) % 4) + 1;
            var keyQuery = string.IsNullOrWhiteSpace(_key) ? string.Empty : "&key=" + Uri.EscapeDataString(_key);
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "https://p{0}.map.gtimg.com/maptilesv2/{1}/{2}/{3}.png?version=1&styleid={4}{5}",
                server,
                zoomLevel,
                x,
                y,
                _style,
                keyQuery);

            return new Uri(url, UriKind.Absolute);
        }
    }
}
