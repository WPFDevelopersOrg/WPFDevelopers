using System;

namespace Standard
{
    [Flags]
    public enum THB : uint
    {
        BITMAP = 1U,
        ICON = 2U,
        TOOLTIP = 4U,
        FLAGS = 8U
    }
}
