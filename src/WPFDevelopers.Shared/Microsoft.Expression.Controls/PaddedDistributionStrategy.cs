using System;
using Microsoft.Expression.Drawing.Core;

namespace Microsoft.Expression.Controls
{
    internal class PaddedDistributionStrategy : DistributionStrategy
    {
        public override int ComputeAutoCapacity()
        {
            return int.MaxValue;
        }

        public override void Initialize()
        {
            base.Step = 0.0;
            this.accumulated = -1.0;
            this.preventOverlap = (base.LayoutPath.FillBehavior == FillBehavior.NoOverlap);
            this.actualSpan = this.ComputeSpan(this.preventOverlap);
        }

        public override void OnPolylineBegin(PolylineData polyline)
        {
            if (this.accumulated == -1.0 && !this.preventOverlap && polyline.IsClosed)
            {
                this.preventOverlap = true;
                this.actualSpan = this.ComputeSpan(this.preventOverlap);
            }
        }

        public override bool ShouldBreak(int numberArranged)
        {
            if (base.ShouldBreak(numberArranged) || this.actualSpan == 0.0)
            {
                return true;
            }
            double num = this.accumulated;
            if (this.preventOverlap)
            {
                num += base.PathPanel.GetChildRadius(base.ChildIndex);
            }
            return MathHelper.GreaterThan(num, this.actualSpan);
        }

        public override void OnPolylineCompleted(double remaingLength)
        {
            this.accumulated += remaingLength;
        }

        public override void OnStepCompleted(double actualStep)
        {
            if (this.accumulated != -1.0)
            {
                this.accumulated += Math.Abs(actualStep);
            }
            else
            {
                this.accumulated = 0.0;
            }
            base.Step = double.NaN;
            if (base.ChildIndex < base.PathPanel.Count - 1)
            {
                base.Step = Math.Max(base.PathPanel.GetChildRadius(base.ChildIndex) + base.LayoutPath.Padding + base.PathPanel.GetChildRadius(base.ChildIndex + 1), 0.0);
                if (base.LayoutPath.Span < 0.0)
                {
                    base.Step *= -1.0;
                }
            }
        }

        private double ComputeSpan(bool preventOverlap)
        {
            if (preventOverlap)
            {
                return Math.Max(0.0, Math.Abs(base.Span) - base.PathPanel.GetChildRadius(base.ChildIndex) - base.LayoutPath.Padding);
            }
            return Math.Abs(base.Span);
        }

        private double accumulated;

        private double actualSpan;

        private bool preventOverlap;
    }
}
