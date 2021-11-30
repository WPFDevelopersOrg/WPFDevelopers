using System.Runtime.InteropServices;

namespace Standard
{
    [StructLayout(LayoutKind.Sequential)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

        public RECT rcMonitor;

        public RECT rcWork;

        public int dwFlags;
    }
}
