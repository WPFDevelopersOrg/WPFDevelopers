using System;

namespace Standard
{
    public struct WINDOWPOS
    {
        public IntPtr hwnd;

        public IntPtr hwndInsertAfter;

        public int x;

        public int y;

        public int cx;

        public int cy;

        public int flags;
    }
}
