using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using WPFDevelopers.Controls.Runtimes.Interop;
using WPFDevelopers.Helpers;
using HandleCollector = WPFDevelopers.Controls.Runtimes.Interop.HandleCollector;

namespace WPFDevelopers.Controls.Runtimes
{
    public static class Kernel32Interop
    {

        [DllImport(Win32.Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", SetLastError = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport(Win32.Kernel32, EntryPoint = "GetModuleFileName", CharSet = CharSet.Unicode,
           SetLastError = true)]
        private static extern int IntGetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);

        [SecurityCritical]
        internal static string GetModuleFileName(HandleRef hModule)
        {
            var sBuilder = new StringBuilder(260);
            while (true)
            {
                var size = IntGetModuleFileName(hModule, sBuilder, sBuilder.Capacity);
                if (size == 0) throw new Win32Exception();
                if (size == sBuilder.Capacity)
                {
                    sBuilder.EnsureCapacity(sBuilder.Capacity * 2);
                    continue;
                }
                return sBuilder.ToString();
            }
        }
        [DllImport(Win32.Kernel32, EntryPoint = "CloseHandle", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IntCloseHandle(HandleRef handle);

        ///<SecurityNote>
        /// Critical: Closes a passed in handle, LinkDemand on Marshal.GetLastWin32Error
        ///</SecurityNote>
        [SecurityCritical]
        public static bool CloseHandleNoThrow(HandleRef handle)
        {
            HandleCollector.Remove((IntPtr)handle, CommonHandles.Kernel);

            bool result = IntCloseHandle(handle);
            int error = Marshal.GetLastWin32Error();

            if (!result)
            {
                Debug.WriteLine("CloseHandle failed.  Error = " + error);
            }

            return result;

        }
    }
}
