using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Controls
{
    internal abstract class DistributionStrategy
    {
        private protected PathPanel PathPanel { get; private set; }

        private protected LayoutPath LayoutPath { get; private set; }

        private protected int ChildIndex { get; private set; }

        private protected double Span { get; private set; }

        protected double Step { get; set; }

        private protected int Capacity { get; private set; }

        public virtual void Initialize()
        {
        }

        public abstract int ComputeAutoCapacity();

        public virtual bool ShouldBreak(int numberArranged)
        {
            return numberArranged >= this.Capacity;
        }

        public virtual void OnPolylineBegin(PolylineData polyline)
        {
        }

        public virtual void OnPolylineCompleted(double remaingLength)
        {
        }

        public virtual void OnStepCompleted(double actualStepDistance)
        {
        }

        public static int Distribute(PathPanel pathPanel, int pathIndex, int childIndex)
        {
            if (pathPanel == null)
            {
                throw new ArgumentNullException("pathPanel");
            }
            if (pathIndex < 0 || pathIndex >= pathPanel.LayoutPaths.Count)
            {
                throw new ArgumentOutOfRangeException("pathIndex");
            }
            if (childIndex < 0 || childIndex >= pathPanel.Count)
            {
                throw new ArgumentOutOfRangeException("childIndex");
            }
            if (pathPanel.ValidPaths == null || pathPanel.ValidPaths.Count == 0)
            {
                throw new InvalidOperationException();
            }
            LayoutPath layoutPath = pathPanel.LayoutPaths[pathIndex];
            if (!layoutPath.IsAttached || layoutPath.Polylines.Count == 0)
            {
                return childIndex;
            }
            if (layoutPath.Distribution == Distribution.Even)
            {
                return new EvenDistributionStrategy().DistributeInternal(pathPanel, pathIndex, childIndex);
            }
            return new PaddedDistributionStrategy().DistributeInternal(pathPanel, pathIndex, childIndex);
        }

        private int DistributeInternal(PathPanel pathPanel, int pathIndex, int childIndex)
        {
            this.PathPanel = pathPanel;
            this.ChildIndex = childIndex;
            this.LayoutPath = this.PathPanel.LayoutPaths[pathIndex];
            this.Capacity = (double.IsNaN(this.LayoutPath.Capacity) ? this.ComputeAutoCapacity() : ((int)Math.Round(this.LayoutPath.Capacity)));
            this.Capacity = Math.Min(this.Capacity, pathPanel.Count - childIndex);
            if (this.Capacity <= 0)
            {
                this.LayoutPath.ActualCapacity = 0.0;
                return childIndex;
            }
            double totalLength = this.LayoutPath.TotalLength;
            double start = this.LayoutPath.Start * totalLength % totalLength;
            if (this.LayoutPath.Start != 0.0 && MathHelper.IsVerySmall(start))
            {
                if (this.LayoutPath.Start > 0.0)
                {
                    start = totalLength;
                }
                else
                {
                    start = 0.0;
                }
            }
            if (MathHelper.LessThan(start, 0.0))
            {
                start += totalLength;
            }
            this.Span = MathHelper.EnsureRange(this.LayoutPath.Span, new double?(-1.0), new double?(1.0)) * totalLength;
            bool isReversed = this.LayoutPath.Span < 0.0;
            IEnumerable<PolylineData> enumerable = PolylineHelper.GetWrappedPolylines(this.LayoutPath.Polylines, ref start);
            if (isReversed)
            {
                enumerable = enumerable.Reverse<PolylineData>();
                start = enumerable.First<PolylineData>().TotalLength - start;
            }
            this.Initialize();
            int numberArranged = 0;
            PolylineData line;
            foreach (PolylineData line2 in enumerable)
            {
                line = line2;
                if (this.ShouldBreak(numberArranged))
                {
                    break;
                }
                bool isFirstStep = true;
                double remaining = line.TotalLength;
                if (isReversed)
                {
                    start = remaining - start;
                }
                this.OnPolylineBegin(line);
                PolylineHelper.PathMarch(line, start, 0.0, delegate (MarchLocation loc)
                {
                    if (loc.Reason == MarchStopReason.CompletePolyline)
                    {
                        if (isFirstStep)
                        {
                            start -= remaining;
                            isFirstStep = false;
                        }
                        else
                        {
                            start = Math.Abs(this.Step) - remaining;
                        }
                        this.OnPolylineCompleted(remaining);
                        return double.NaN;
                    }
                    if (loc.Reason == MarchStopReason.CornerPoint)
                    {
                        return loc.Remain;
                    }
                    double num = this.Step;
                    if (isFirstStep)
                    {
                        if (!isReversed)
                        {
                            remaining -= start;
                            num = start;
                        }
                        else
                        {
                            remaining = start;
                            num = line.TotalLength - start;
                        }
                    }
                    else
                    {
                        remaining -= Math.Abs(num);
                    }
                    this.OnStepCompleted(num);
                    isFirstStep = false;
                    if (this.ShouldBreak(numberArranged))
                    {
                        return double.NaN;
                    }
                    this.PathPanel.ArrangeChild(this.ChildIndex, pathIndex, line, loc, numberArranged);
                    numberArranged++;
                    this.ChildIndex++;
                    return this.Step;
                });
            }
            this.LayoutPath.ActualCapacity = (double)numberArranged;
            return this.ChildIndex;
        }
    }
}
