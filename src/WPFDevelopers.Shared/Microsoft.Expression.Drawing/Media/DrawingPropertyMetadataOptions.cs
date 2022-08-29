using System;

namespace Microsoft.Expression.Drawing.Media
{
    [Flags]
    internal enum DrawingPropertyMetadataOptions
    {
        None = 0,
        AffectsMeasure = 1,
        AffectsRender = 16
    }
}
