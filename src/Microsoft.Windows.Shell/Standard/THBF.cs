using System;

namespace Standard
{
    [Flags]
    public enum THBF : uint
    {
        ENABLED = 0U,
        DISABLED = 1U,
        DISMISSONCLICK = 2U,
        NOBACKGROUND = 4U,
        HIDDEN = 8U,
        NONINTERACTIVE = 16U
    }
}
