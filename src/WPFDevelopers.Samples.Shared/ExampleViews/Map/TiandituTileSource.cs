using System;
using System.Globalization;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public sealed class TiandituTileSource : MapTileSource
    {
        private readonly string _layer;
        private readonly int _serverIndex;
        private readonly string _token;

        public TiandituTileSource(string layer = "vec_c", int serverIndex = 0, string token = "")
        {
            _layer = string.IsNullOrWhiteSpace(layer) ? "vec_c" : layer;
            _serverIndex = serverIndex;
            _token = token ?? string.Empty;
        }

        public string Layer
        {
            get { return _layer; }
        }

        public string Token
        {
            get { return _token; }
        }

        public override Uri GetTileUri(int x, int y, int zoomLevel)
        {
            if (string.IsNullOrWhiteSpace(_layer) || string.IsNullOrWhiteSpace(_token))
            {
                return null;
            }

            var server = _serverIndex % 4;
            var wmtsLayer = _layer;
            var underscoreIndex = wmtsLayer.IndexOf('_');
            if (underscoreIndex >= 0)
            {
                wmtsLayer = wmtsLayer.Substring(0, underscoreIndex);
            }

            var tokenQuery = "&tk=" + Uri.EscapeDataString(_token);
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "https://t{0}.tianditu.gov.cn/{1}/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER={2}&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILECOL={3}&TILEROW={4}&TILEMATRIX={5}{6}",
                server,
                _layer,
                wmtsLayer,
                x,
                y,
                zoomLevel,
                tokenQuery);

            return new Uri(url, UriKind.Absolute);
        }
    }
}
