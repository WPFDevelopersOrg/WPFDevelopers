using System;

namespace Microsoft.Expression.Controls
{
    [Flags]
    public enum ChangedPathLayoutProperties
    {
        None = 0,
        LayoutPathIndex = 1,
        GlobalIndex = 2,
        LocalIndex = 4,
        GlobalOffset = 8,
        LocalOffset = 16,
        NormalAngle = 32,
        OrientationAngle = 64,
        IsArranged = 128
    }
}
