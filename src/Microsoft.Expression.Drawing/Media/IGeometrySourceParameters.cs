using System.Windows.Media;

namespace Microsoft.Expression.Media
{
    public interface IGeometrySourceParameters
    {
        Stretch Stretch { get; }
        Brush Stroke { get; }
        double StrokeThickness { get; }
    }
}
