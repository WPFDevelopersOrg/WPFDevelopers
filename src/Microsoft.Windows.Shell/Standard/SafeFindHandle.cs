using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;

namespace Standard
{
    public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
#pragma warning disable SYSLIB0003 // Type or member is obsolete
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
#pragma warning restore SYSLIB0003 // Type or member is obsolete
        private SafeFindHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.FindClose(this.handle);
        }
    }
}
