using System;

namespace Standard
{
    [Flags]
    public enum WS_EX : uint
    {
        None = 0U,
        DLGMODALFRAME = 1U,
        NOPARENTNOTIFY = 4U,
        TOPMOST = 8U,
        ACCEPTFILES = 16U,
        TRANSPARENT = 32U,
        MDICHILD = 64U,
        TOOLWINDOW = 128U,
        WINDOWEDGE = 256U,
        CLIENTEDGE = 512U,
        CONTEXTHELP = 1024U,
        RIGHT = 4096U,
        LEFT = 0U,
        RTLREADING = 8192U,
        LTRREADING = 0U,
        LEFTSCROLLBAR = 16384U,
        RIGHTSCROLLBAR = 0U,
        CONTROLPARENT = 65536U,
        STATICEDGE = 131072U,
        APPWINDOW = 262144U,
        LAYERED = 524288U,
        NOINHERITLAYOUT = 1048576U,
        LAYOUTRTL = 4194304U,
        COMPOSITED = 33554432U,
        NOACTIVATE = 134217728U,
        OVERLAPPEDWINDOW = 768U,
        PALETTEWINDOW = 392U
    }
}
