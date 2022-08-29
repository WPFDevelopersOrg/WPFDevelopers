using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Media
{
    public interface IGeometrySource
    {
        bool InvalidateGeometry(InvalidateGeometryReasons reasons);

        bool UpdateGeometry(IGeometrySourceParameters parameters, Rect layoutBounds);

        Geometry Geometry { get; }

        Rect LogicalBounds { get; }

        Rect LayoutBounds { get; }
    }
}
