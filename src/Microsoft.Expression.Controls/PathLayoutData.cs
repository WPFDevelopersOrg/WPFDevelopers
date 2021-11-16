namespace Microsoft.Expression.Controls
{
    public class PathLayoutData
    {
        public int LayoutPathIndex { get; set; }

        public int GlobalIndex { get; set; }

        public int LocalIndex { get; set; }

        public double GlobalOffset { get; set; }

        public double LocalOffset { get; set; }

        public double NormalAngle { get; set; }

        public double OrientationAngle { get; set; }

        public bool IsArranged { get; set; }
    }
}
