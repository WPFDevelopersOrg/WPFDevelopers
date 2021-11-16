namespace Microsoft.Expression.Media
{
    internal interface IPolygonGeometrySourceParameters : IGeometrySourceParameters
    {
        double PointCount { get; }
        double InnerRadius { get; }
    }
}
