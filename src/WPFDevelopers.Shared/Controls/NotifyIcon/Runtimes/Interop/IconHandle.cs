using System;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace WPFDevelopers.Controls.Runtimes.Interop
{
    internal sealed class IconHandle : WpfSafeHandle
    {
        /// <SecurityNote>
        /// Critical: This code calls into a base class which is protected by a SecurityCritical constructor.
        /// </SecurityNote>
        [SecurityCritical]
        private IconHandle() : base(true, CommonHandles.Icon)
        {
        }

        /// <SecurityNote>
        ///     Critical: This calls into DestroyIcon
        /// </SecurityNote>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return User32Interop.DestroyIcon(handle);
        }

        /// <SecurityNote>
        ///     Critical: This creates a new SafeHandle, which has a critical constructor.
        ///     TreatAsSafe: The handle this creates is invalid.  It contains no critical data.
        /// </SecurityNote>
        [SecurityCritical, SecurityTreatAsSafe]
        internal static IconHandle GetInvalidIcon()
        {
            return new IconHandle();
        }

        /// <summary>
        /// Get access to the raw handle for native APIs that require it.
        /// </summary>
        /// <SecurityNote>
        ///     Critical: This accesses critical data for the safe handle.
        /// </SecurityNote>
        [SecurityCritical]
        internal IntPtr CriticalGetHandle()
        {
            return handle;
        }
    }
}
