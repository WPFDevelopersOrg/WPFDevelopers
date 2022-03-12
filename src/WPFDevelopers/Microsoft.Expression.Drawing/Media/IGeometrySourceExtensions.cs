using System;
using System.Windows;

namespace Microsoft.Expression.Media
{
    internal static class IGeometrySourceExtensions
    {
        public static double GetHalfStrokeThickness(this IGeometrySourceParameters parameter)
        {
            if (parameter.Stroke != null)
            {
                double strokeThickness = parameter.StrokeThickness;
                if (!double.IsNaN(strokeThickness) && !double.IsInfinity(strokeThickness))
                    return Math.Abs(strokeThickness) / 2.0;
            }
            return 0.0;
        }

        public static GeometryEffect GetGeometryEffect(this IGeometrySourceParameters parameters)
        {
            DependencyObject dependencyObject = parameters as DependencyObject;
            if (dependencyObject == null)
                return null;

            GeometryEffect geometryEffect = GeometryEffect.GetGeometryEffect(dependencyObject);
            if (geometryEffect == null || !dependencyObject.Equals(geometryEffect.Parent))
                return null;

            return geometryEffect;
        }
    }
}
