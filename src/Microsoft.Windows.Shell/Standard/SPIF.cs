using System;

namespace Standard
{
    [Flags]
    public enum SPIF
    {
        None = 0,
        UPDATEINIFILE = 1,
        SENDCHANGE = 2,
        SENDWININICHANGE = 2
    }
}
