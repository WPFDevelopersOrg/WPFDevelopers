using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace Standard
{
    public sealed class SafeHBITMAP : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeHBITMAP() : base(true)
        {
        }

#pragma warning disable SYSLIB0004 // Type or member is obsolete
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#pragma warning restore SYSLIB0004 // Type or member is obsolete
        protected override bool ReleaseHandle()
        {
            return NativeMethods.DeleteObject(this.handle);
        }
    }
}
