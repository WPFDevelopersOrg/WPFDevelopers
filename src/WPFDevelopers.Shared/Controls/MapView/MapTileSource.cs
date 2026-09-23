using System;
using System.Globalization;
using System.Text;

namespace WPFDevelopers.Controls
{
    public abstract class MapTileSource
    {
        public virtual int MinZoomLevel
        {
            get { return 1; }
        }

        public virtual int MaxZoomLevel
        {
            get { return 19; }
        }

        public abstract Uri GetTileUri(int x, int y, int zoomLevel);
    }

    public static class MapTileSourceFactory
    {
        public static MapTileSource Create(string template, string subdomains = "", bool useTmsY = false, int style = 7)
        {
            return new TemplateMapTileSource(template, subdomains, useTmsY, style);
        }
    }

    public sealed class TemplateMapTileSource : MapTileSource
    {
        private readonly string _template;
        private readonly string _subdomains;
        private readonly bool _useTmsY;
        private readonly int _style;

        public TemplateMapTileSource(string template, string subdomains, bool useTmsY, int style)
        {
            _template = template;
            _subdomains = subdomains;
            _useTmsY = useTmsY;
            _style = style;
        }

        public override Uri GetTileUri(int x, int y, int zoomLevel)
        {
            if (string.IsNullOrWhiteSpace(_template))
            {
                return null;
            }

            var maxTileIndex = (1 << zoomLevel) - 1;
            var yValue = _useTmsY ? (maxTileIndex - y) : y;
            var subdomain = ResolveSubdomain(_subdomains, x, y);
            var quadKey = BuildQuadKey(zoomLevel, x, y);

            var url = _template
                .Replace("{z}", zoomLevel.ToString(CultureInfo.InvariantCulture))
                .Replace("{x}", x.ToString(CultureInfo.InvariantCulture))
                .Replace("{y}", yValue.ToString(CultureInfo.InvariantCulture))
                .Replace("{style}", _style.ToString(CultureInfo.InvariantCulture))
                .Replace("{s}", subdomain)
                .Replace("{q}", quadKey)
                .Replace("{quadkey}", quadKey)
                .Replace("{-y}", (maxTileIndex - y).ToString(CultureInfo.InvariantCulture));

            return string.IsNullOrWhiteSpace(url) ? null : new Uri(url, UriKind.Absolute);
        }

        private static string ResolveSubdomain(string tileSubdomains, int tileX, int tileY)
        {
            if (string.IsNullOrWhiteSpace(tileSubdomains))
            {
                return string.Empty;
            }

            var segments = tileSubdomains.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                return string.Empty;
            }

            var index = Math.Abs(tileX + tileY) % segments.Length;
            return segments[index].Trim();
        }

        private static string BuildQuadKey(int zoom, int tileX, int tileY)
        {
            var quadKey = new StringBuilder(zoom);
            for (var i = zoom; i > 0; i--)
            {
                char digit = '0';
                var mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }

                if ((tileY & mask) != 0)
                {
                    digit = (char)(digit + 2);
                }

                quadKey.Append(digit);
            }

            return quadKey.ToString();
        }
    }

}
