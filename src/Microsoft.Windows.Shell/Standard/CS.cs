using System;

namespace Standard
{
    [Flags]
    public enum CS : uint
    {
        VREDRAW = 1U,
        HREDRAW = 2U,
        DBLCLKS = 8U,
        OWNDC = 32U,
        CLASSDC = 64U,
        PARENTDC = 128U,
        NOCLOSE = 512U,
        SAVEBITS = 2048U,
        BYTEALIGNCLIENT = 4096U,
        BYTEALIGNWINDOW = 8192U,
        GLOBALCLASS = 16384U,
        IME = 65536U,
        DROPSHADOW = 131072U
    }
}
