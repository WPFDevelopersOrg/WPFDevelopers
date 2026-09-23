using System;
using System.Globalization;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public sealed class OSMTileSource : MapTileSource
    {
        public override Uri GetTileUri(int x, int y, int zoomLevel)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "https://tile.openstreetmap.org/{0}/{1}/{2}.png",
                zoomLevel,
                x,
                y);

            return new Uri(url, UriKind.Absolute);
        }
    }
}
