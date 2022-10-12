using Microsoft.Win32.SafeHandles;
using System.Security;

namespace WPFDevelopers.Controls.Runtimes.Interop
{
    internal abstract class WpfSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private int _collectorId;

        /// <SecurityNote>
        ///      Critical:This code calls into a base class which is protected by link demand and by inheritance demand
        /// </SecurityNote>
        [SecurityCritical]
        protected WpfSafeHandle(bool ownsHandle, int collectorId) : base(ownsHandle)
        {
            HandleCollector.Add(collectorId);
            _collectorId = collectorId;
        }

        /// <SecurityNote>
        /// Critical: Conceptually, this would be accessing critical data as it's in the destroy call path.
        /// TreatAsSafe: This is just destroying a handle that this object owns.
        /// </SecurityNote>
        [SecurityCritical, SecurityTreatAsSafe]
        protected override void Dispose(bool disposing)
        {
            HandleCollector.Remove(_collectorId);
            base.Dispose(disposing);
        }

        // ReleaseHandle implementation is deferred to derived classes.
        // protected override bool ReleaseHandle() {}
    }
}
