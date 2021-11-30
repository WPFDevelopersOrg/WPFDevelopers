using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Standard
{
    [StructLayout(LayoutKind.Explicit)]
    public struct WTA_OPTIONS
    {
        public const uint Size = 8U;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used by native code.")]
        [FieldOffset(0)]
        public WTNCA dwFlags;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used by native code.")]
        [FieldOffset(4)]
        public WTNCA dwMask;
    }
}
