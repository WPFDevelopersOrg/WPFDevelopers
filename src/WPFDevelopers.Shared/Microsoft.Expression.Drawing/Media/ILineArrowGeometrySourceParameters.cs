namespace Microsoft.Expression.Drawing.Media
{
    internal interface ILineArrowGeometrySourceParameters : IGeometrySourceParameters
    {
        double BendAmount { get; }

        double ArrowSize { get; }

        ArrowType StartArrow { get; }

        ArrowType EndArrow { get; }

        CornerType StartCorner { get; }
    }
}
