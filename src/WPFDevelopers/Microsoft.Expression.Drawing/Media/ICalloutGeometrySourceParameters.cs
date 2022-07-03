using System.Windows;

namespace Microsoft.Expression.Drawing.Media
{
    internal interface ICalloutGeometrySourceParameters : IGeometrySourceParameters
    {
        CalloutStyle CalloutStyle { get; }
        Point AnchorPoint { get; }
    }
}
