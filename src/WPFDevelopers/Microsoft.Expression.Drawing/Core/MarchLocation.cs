using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Drawing.Core
{
    public class MarchLocation
    {
        public static MarchLocation Create(MarchStopReason reason, int index, double before, double after, double remain)
        {
            double num = before + after;
            return new MarchLocation
            {
                Reason = reason,
                Index = index,
                Remain = remain,
                Before = MathHelper.EnsureRange(before, new double?(0.0), new double?(num)),
                After = MathHelper.EnsureRange(after, new double?(0.0), new double?(num)),
                Ratio = MathHelper.EnsureRange(MathHelper.SafeDivide(before, num, 0.0), new double?(0.0), new double?(1.0))
            };
        }

        public MarchStopReason Reason { get; private set; }

        public int Index { get; private set; }

        public double Ratio { get; private set; }

        public double Before { get; private set; }

        public double After { get; private set; }

        public double Remain { get; private set; }

        public Point GetPoint(IList<Point> points)
        {
            return GeometryHelper.Lerp(points[this.Index], points[this.Index + 1], this.Ratio);
        }

        public Vector GetNormal(PolylineData polyline, double cornerRadius = 0.0)
        {
            return polyline.SmoothNormal(this.Index, this.Ratio, cornerRadius);
        }

        public double GetArcLength(IList<double> accumulatedLengths)
        {
            return MathHelper.Lerp(accumulatedLengths[this.Index], accumulatedLengths[this.Index + 1], this.Ratio);
        }
    }
}
