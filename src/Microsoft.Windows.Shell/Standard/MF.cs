using System;

namespace Standard
{
    [Flags]
    public enum MF : uint
    {
        DOES_NOT_EXIST = 4294967295U,
        ENABLED = 0U,
        BYCOMMAND = 0U,
        GRAYED = 1U,
        DISABLED = 2U
    }
}
