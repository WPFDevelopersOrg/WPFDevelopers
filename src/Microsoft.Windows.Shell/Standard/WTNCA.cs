using System;

namespace Standard
{
    [Flags]
    internal enum WTNCA : uint
    {
        NODRAWCAPTION = 1U,
        NODRAWICON = 2U,
        NOSYSMENU = 4U,
        NOMIRRORHELP = 8U,
        VALIDBITS = 15U
    }
}
