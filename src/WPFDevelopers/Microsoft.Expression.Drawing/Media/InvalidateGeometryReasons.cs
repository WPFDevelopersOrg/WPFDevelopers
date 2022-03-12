using System;

namespace Microsoft.Expression.Media
{
    [Flags]
    public enum InvalidateGeometryReasons
    {
        PropertyChanged = 1,
        IsAnimated = 2,
        ChildInvalidated = 4,
        ParentInvalidated = 8,
        TemplateChanged = 16
    }
}
