namespace Microsoft.Expression.Drawing.Media
{
    internal interface IPolygonGeometrySourceParameters : IGeometrySourceParameters
    {
        double PointCount { get; }
        double InnerRadius { get; }
    }
}
