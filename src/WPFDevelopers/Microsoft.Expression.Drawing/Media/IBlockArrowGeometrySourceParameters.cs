namespace Microsoft.Expression.Media
{
    internal interface IBlockArrowGeometrySourceParameters : IGeometrySourceParameters
    {
        ArrowOrientation Orientation { get; }
        double ArrowheadAngle { get; }
        double ArrowBodySize { get; }
    }
}
