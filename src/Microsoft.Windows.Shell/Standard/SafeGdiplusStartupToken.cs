using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;

namespace Standard
{
    public sealed class SafeGdiplusStartupToken : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeGdiplusStartupToken() : base(true)
        {
        }

#pragma warning disable SYSLIB0004 // Type or member is obsolete
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#pragma warning restore SYSLIB0004 // Type or member is obsolete
        protected override bool ReleaseHandle()
        {
            Status status = NativeMethods.GdiplusShutdown(this.handle);
            return status == Status.Ok;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        public static SafeGdiplusStartupToken Startup()
        {
            SafeGdiplusStartupToken safeGdiplusStartupToken = new SafeGdiplusStartupToken();
            IntPtr handle;
            StartupOutput startupOutput;
            if (NativeMethods.GdiplusStartup(out handle, new StartupInput(), out startupOutput) == Status.Ok)
            {
                safeGdiplusStartupToken.handle = handle;
                return safeGdiplusStartupToken;
            }
            safeGdiplusStartupToken.Dispose();
            throw new Exception("Unable to initialize GDI+");
        }
    }
}
