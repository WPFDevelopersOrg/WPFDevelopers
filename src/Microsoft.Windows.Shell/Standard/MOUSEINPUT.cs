using System;

namespace Standard
{
    public struct MOUSEINPUT
    {
#pragma warning disable 0649

        public int dx;

        public int dy;

        public int mouseData;

        public int dwFlags;

        public int time;

        public IntPtr dwExtraInfo;

#pragma warning restore 0649
    }
}
