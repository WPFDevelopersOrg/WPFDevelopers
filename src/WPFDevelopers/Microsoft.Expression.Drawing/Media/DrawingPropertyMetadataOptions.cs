using System;

namespace Microsoft.Expression.Media
{
    [Flags]
    internal enum DrawingPropertyMetadataOptions
    {
        None = 0,
        AffectsMeasure = 1,
        AffectsRender = 16
    }
}
