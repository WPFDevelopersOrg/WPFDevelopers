using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Standard
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential)]
    public class RefPOINT
    {
        public int x;

        public int y;
    }
}
