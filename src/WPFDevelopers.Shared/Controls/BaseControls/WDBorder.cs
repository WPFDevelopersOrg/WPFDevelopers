using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class WDBorder : Border
    {
        public static readonly DependencyPropertyKey ContentClipPropertyKey =
            DependencyProperty.RegisterReadOnly("ContentClip", typeof(Geometry), typeof(WDBorder),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ContentClipProperty = ContentClipPropertyKey.DependencyProperty;

        public Geometry ContentClip
        {
            get => (Geometry) GetValue(ContentClipProperty);
            set => SetValue(ContentClipProperty, value);
        }

        private Geometry CalculateContentClip()
        {
            var borderThickness = BorderThickness;
            var cornerRadius = CornerRadius;
            var renderSize = RenderSize;
            var width = renderSize.Width - borderThickness.Left - borderThickness.Right;
            var height = renderSize.Height - borderThickness.Top - borderThickness.Bottom;
            if (width > 0.0 && height > 0.0)
            {
                var rect = new Rect(0.0, 0.0, width, height);
                var radii = new GeometryHelper.Radii(cornerRadius, borderThickness, false);
                var streamGeometry = new StreamGeometry();
                using (var streamGeometryContext = streamGeometry.Open())
                {
                    GeometryHelper.GenerateGeometry(streamGeometryContext, rect, radii);
                    streamGeometry.Freeze();
                    return streamGeometry;
                }
            }

            return null;
        }

        protected override void OnRender(DrawingContext dc)
        {
            SetValue(ContentClipPropertyKey, CalculateContentClip());
            base.OnRender(dc);
        }
    }
}