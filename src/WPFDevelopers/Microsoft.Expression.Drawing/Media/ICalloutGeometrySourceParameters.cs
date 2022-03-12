using System.Windows;

namespace Microsoft.Expression.Media
{
    internal interface ICalloutGeometrySourceParameters : IGeometrySourceParameters
    {
        CalloutStyle CalloutStyle { get; }
        Point AnchorPoint { get; }
    }
}
