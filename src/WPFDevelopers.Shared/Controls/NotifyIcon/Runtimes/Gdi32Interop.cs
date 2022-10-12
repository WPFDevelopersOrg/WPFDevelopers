using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using WPFDevelopers.Controls.Runtimes.Interop;

namespace WPFDevelopers.Controls.Runtimes
{
    public static class Gdi32Interop
    {
        private const string _Gdi32 = "gdi32.dll";

        ///  
        /// Critical as suppressing UnmanagedCodeSecurity
        /// 
        [SecurityCritical][SuppressUnmanagedCodeSecurity]
        [DllImport(_Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto, EntryPoint = "CreateBitmap")]
        private static extern BitmapHandle PrivateCreateBitmap(int width, int height, int planes, int bitsPerPixel, byte[] lpvBits);

        ///  
        /// Critical - The method invokes PrivateCreateBitmap. 
        /// 
        [SecurityCritical]
        internal static BitmapHandle CreateBitmap(int width, int height, int planes, int bitsPerPixel, byte[] lpvBits)
        {
            var hBitmap = PrivateCreateBitmap(width, height, planes, bitsPerPixel, lpvBits);
            var error = Marshal.GetLastWin32Error();

            if (hBitmap.IsInvalid) Debug.WriteLine("CreateBitmap failed. Error = " + error);

            return hBitmap;
        }

        ///<SecurityNote>
        /// Critical as this code performs an elevation.
        ///</SecurityNote>
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport(_Gdi32, EntryPoint = "DeleteObject", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IntDeleteObject(IntPtr hObject);

        ///<SecurityNote>
        /// Critical: calls a critical method (IntDeleteObject)
        ///</SecurityNote>
        [SecurityCritical]
        public static bool DeleteObject(IntPtr hObject)
        {
            var result = IntDeleteObject(hObject);
            var error = Marshal.GetLastWin32Error();

            if (!result)
            // To be consistent with out other PInvoke wrappers
            // we should "throw" here.  But we don't want to
            // introduce new "throws" w/o time to follow up on any
            // new problems that causes.
                Debug.WriteLine("DeleteObject failed.  Error = " + error);
            //throw new Win32Exception();

            return result;
        }

        /// <SecurityNote>
        /// Critical : Elevates to UnmanagedCode permissions
        /// </SecurityNote>
        [SecurityCritical]
        [DllImport(_Gdi32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetEnhMetaFileBits(uint cbBuffer, byte[] buffer);

        /// <SecurityNote>
        /// Critical as suppressing UnmanagedCodeSecurity
        /// </SecurityNote>
        [SecurityCritical][SuppressUnmanagedCodeSecurity]
        [DllImport(_Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto, EntryPoint = "CreateDIBSection")]
        private static extern BitmapHandle PrivateCreateDIBSection(HandleRef hdc, ref BITMAPINFO bitmapInfo, int iUsage, ref IntPtr ppvBits, User32Interop.SafeFileMappingHandle hSection, int dwOffset);

        /// <SecurityNote>
        /// Critical - The method invokes PrivateCreateDIBSection.
        /// </SecurityNote>
        [SecurityCritical]
        internal static BitmapHandle CreateDIBSection(HandleRef hdc, ref BITMAPINFO bitmapInfo, int iUsage, ref IntPtr ppvBits, User32Interop.SafeFileMappingHandle hSection, int dwOffset)
        {
            if (hSection == null)
            // PInvoke marshalling does not handle null SafeHandle, we must pass an IntPtr.Zero backed SafeHandle
                hSection = new User32Interop.SafeFileMappingHandle(IntPtr.Zero);

            var hBitmap = PrivateCreateDIBSection(hdc, ref bitmapInfo, iUsage, ref ppvBits, hSection, dwOffset);
            var error = Marshal.GetLastWin32Error();

            if (hBitmap.IsInvalid) Debug.WriteLine("CreateDIBSection failed. Error = " + error);

            return hBitmap;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class ICONINFO
        {
            public bool fIcon = false;
            public BitmapHandle hbmColor = null;
            public BitmapHandle hbmMask = null;
            public int xHotspot = 0;
            public int yHotspot = 0;
        }
    }
}
