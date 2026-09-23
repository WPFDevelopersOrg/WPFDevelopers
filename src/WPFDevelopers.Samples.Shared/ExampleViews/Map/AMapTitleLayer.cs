using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public sealed class AMapTitleLayer : MapTileLayer
    {
        private readonly AMapTileSource _tileSource;

        public AMapTitleLayer()
        {
            _tileSource = new AMapTileSource();
            TileSource = _tileSource;
        }

        public int Style
        {
            get { return _tileSource.Style; }
            set { _tileSource.Style = value; }
        }

        public void UpdateTileSourceStyle(int style)
        {
            _tileSource.Style = style;
        }
    }
}
