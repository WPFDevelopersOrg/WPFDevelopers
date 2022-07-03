using Microsoft.Expression.Drawing.Core;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Media
{
    public abstract class GeometrySource<TParameters> : IGeometrySource where TParameters : IGeometrySourceParameters
    {
        public Geometry Geometry { get; private set; }

        public Rect LogicalBounds { get; private set; }

        public Rect LayoutBounds { get; private set; }

        public bool InvalidateGeometry(InvalidateGeometryReasons reasons)
        {
            if ((reasons & InvalidateGeometryReasons.TemplateChanged) != 0)
            {
                cachedGeometry = null;
            }
            if (!geometryInvalidated)
            {
                geometryInvalidated = true;
                return true;
            }
            return false;
        }

        public bool UpdateGeometry(IGeometrySourceParameters parameters, Rect layoutBounds)
        {
            bool flag = false;
            if (parameters is TParameters)
            {
                Rect rect = ComputeLogicalBounds(layoutBounds, parameters);
                flag |= LayoutBounds != layoutBounds || LogicalBounds != rect;
                if (geometryInvalidated || flag)
                {
                    LayoutBounds = layoutBounds;
                    LogicalBounds = rect;
                    flag |= UpdateCachedGeometry((TParameters)parameters);
                    bool flag2 = flag;
                    bool force = flag;
                    flag = flag2 | ApplyGeometryEffect(parameters, force);
                }
            }
            geometryInvalidated = false;
            return flag;
        }

        protected abstract bool UpdateCachedGeometry(TParameters parameters);

        protected virtual Rect ComputeLogicalBounds(Rect layoutBounds, IGeometrySourceParameters parameters)
        {
            return GeometryHelper.Inflate(layoutBounds, -parameters.GetHalfStrokeThickness());
        }

        private bool ApplyGeometryEffect(IGeometrySourceParameters parameters, bool force)
        {
            bool result = false;
            Geometry outputGeometry = cachedGeometry;
            GeometryEffect geometryEffect = parameters.GetGeometryEffect();
            if (geometryEffect != null)
            {
                if (force)
                {
                    result = true;
                    geometryEffect.InvalidateGeometry(InvalidateGeometryReasons.ParentInvalidated);
                }
                if (geometryEffect.ProcessGeometry(cachedGeometry))
                {
                    result = true;
                    outputGeometry = geometryEffect.OutputGeometry;
                }
            }
            if (Geometry != outputGeometry)
            {
                result = true;
                Geometry = outputGeometry;
            }
            return result;
        }

        private bool geometryInvalidated;

        protected Geometry cachedGeometry;
    }
}
