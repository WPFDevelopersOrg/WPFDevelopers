using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace WPFDevelopers.Controls.Runtimes.Interop
{
    internal sealed class BitmapHandle : WpfSafeHandle
    {
        /// <SecurityNote>
        /// Critical: This code calls into a base class which is protected by a SecurityCritical constructor.
        /// </SecurityNote>
        [SecurityCritical]
        private BitmapHandle() : this(true)
        {
        }

        /// <SecurityNote>
        /// Critical: This code calls into a base class which is protected by a SecurityCritical constructor.
        /// </SecurityNote>
        [SecurityCritical]
        private BitmapHandle(bool ownsHandle) : base(ownsHandle, CommonHandles.GDI)
        {
        }
        /// <SecurityNote>
        ///     Critical: This calls into DeleteObject
        /// </SecurityNote>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return Gdi32Interop.DeleteObject(handle);
        }

        /// <SecurityNote>
        ///     Critical: Accesses internal critical data.
        /// </SecurityNote>
        [SecurityCritical]
        internal HandleRef MakeHandleRef(object obj)
        {
            return new HandleRef(obj, handle);
        }

        /// <SecurityNote>
        ///     Critical: Creates a new BitmapHandle using Critical constructor.
        /// </SecurityNote>
        [SecurityCritical]
        internal static BitmapHandle CreateFromHandle(IntPtr hbitmap, bool ownsHandle = true)
        {
            return new BitmapHandle(ownsHandle)
            {
                handle = hbitmap,
            };
        }
    }
}
