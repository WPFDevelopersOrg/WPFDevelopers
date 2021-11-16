using Microsoft.Expression.Drawing.Core;
using System;
using System.Linq;

namespace Microsoft.Expression.Controls
{
    internal class EvenDistributionStrategy : DistributionStrategy
    {
        public override int ComputeAutoCapacity()
        {
            return (int)Math.Ceiling((double)base.PathPanel.Count / (double)base.PathPanel.ValidPaths.Count);
        }

        public override void OnPolylineBegin(PolylineData polyline)
        {
            base.Step = double.NaN;
            if (base.Capacity > 1)
            {
                if (polyline.IsClosed || base.LayoutPath.FillBehavior == FillBehavior.NoOverlap)
                {
                    base.Step = base.Span / (double)base.Capacity;
                    return;
                }
                base.Step = base.Span / (double)(base.Capacity - 1);
            }
        }
    }
}
