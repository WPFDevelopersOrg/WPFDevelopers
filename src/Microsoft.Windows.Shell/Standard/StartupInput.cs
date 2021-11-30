using System;
using System.Runtime.InteropServices;

namespace Standard
{
    [StructLayout(LayoutKind.Sequential)]
    public class StartupInput
    {
        public int GdiplusVersion = 1;

        public IntPtr DebugEventCallback;

        public bool SuppressBackgroundThread;

        public bool SuppressExternalCodecs;
    }
}
