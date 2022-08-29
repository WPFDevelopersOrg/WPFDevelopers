using System;

namespace Standard
{
    [Flags]
    public enum NIF : uint
    {
        MESSAGE = 1U,
        ICON = 2U,
        TIP = 4U,
        STATE = 8U,
        INFO = 16U,
        GUID = 32U,
        REALTIME = 64U,
        SHOWTIP = 128U,
        XP_MASK = 59U,
        VISTA_MASK = 251U
    }
}
