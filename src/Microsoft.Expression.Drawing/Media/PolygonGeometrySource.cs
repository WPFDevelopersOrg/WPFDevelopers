using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Media
{
    internal class PolygonGeometrySource : GeometrySource<IPolygonGeometrySourceParameters>
    {
        protected override Rect ComputeLogicalBounds(Rect layoutBounds, IGeometrySourceParameters parameters)
        {
            Rect logicalBound = base.ComputeLogicalBounds(layoutBounds, parameters);
            return GeometryHelper.GetStretchBound(logicalBound, parameters.Stretch, new Size(1.0, 1.0));
        }

        protected override bool UpdateCachedGeometry(IPolygonGeometrySourceParameters parameters)
        {
            bool flag = false;
            int num = Math.Max(3, Math.Min(100, (int)Math.Round(parameters.PointCount)));
            double num2 = 360.0 / (double)num;
            double num3 = Math.Max(0.0, Math.Min(1.0, parameters.InnerRadius));
            if (num3 < 1.0)
            {
                double num4 = Math.Cos(3.141592653589793 / (double)num);
                double ratio = num3 * num4;
                double num5 = num2 / 2.0;
                cachedPoints.EnsureListCount(num * 2, null);
                Rect bound = LogicalBounds.Resize(ratio);
                for (int i = 0; i < num; i++)
                {
                    double num6 = num2 * (double)i;
                    cachedPoints[i * 2] = GeometryHelper.GetArcPoint(num6, LogicalBounds);
                    cachedPoints[i * 2 + 1] = GeometryHelper.GetArcPoint(num6 + num5, bound);
                }
            }
            else
            {
                cachedPoints.EnsureListCount(num, null);
                for (int j = 0; j < num; j++)
                {
                    double degree = num2 * (double)j;
                    cachedPoints[j] = GeometryHelper.GetArcPoint(degree, LogicalBounds);
                }
            }
            return flag | PathGeometryHelper.SyncPolylineGeometry(ref cachedGeometry, cachedPoints, true);
        }

        private List<Point> cachedPoints = new List<Point>();
    }
}
