namespace Microsoft.Expression.Drawing.Media
{
    internal interface IBlockArrowGeometrySourceParameters : IGeometrySourceParameters
    {
        ArrowOrientation Orientation { get; }
        double ArrowheadAngle { get; }
        double ArrowBodySize { get; }
    }
}
