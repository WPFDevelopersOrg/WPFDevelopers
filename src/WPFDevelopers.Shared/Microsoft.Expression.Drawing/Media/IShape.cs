using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Media
{
    public interface IShape
    {
        Brush Fill { get; set; }
        Brush Stroke { get; set; }
        double StrokeThickness { get; set; }
        Stretch Stretch { get; set; }
        Geometry RenderedGeometry { get; }
        Thickness GeometryMargin { get; }
        void InvalidateGeometry(InvalidateGeometryReasons reasons);
        event EventHandler RenderedGeometryChanged;
    }
}
