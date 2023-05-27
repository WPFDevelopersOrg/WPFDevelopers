using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32.SafeHandles;
using WPFDevelopers.Controls.Runtimes.Interop;
using WPFDevelopers.Controls.Runtimes.User32;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls.Runtimes
{
    public static class User32Interop
    {

        [DllImport(Win32.User32, EntryPoint = "RegisterClassExW", SetLastError = true)]
        public static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport(Win32.User32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int RegisterWindowMessage(string lpString);

        [DllImport(Win32.User32, CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(Win32.User32, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(WS_EX dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport(Win32.User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hwnd);

        [DllImport(Win32.User32, CharSet = CharSet.Unicode, EntryPoint = "UnregisterClass", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterClassName(string lpClassName, IntPtr hInstance);

        [DllImport(Win32.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport(Win32.User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, ImageType uType, int cxDesired, int cyDesired, LoadImageFlags fuLoad);

        //[DllImport(Win32.User32, SetLastError = true)]
        //public static extern int DestroyIcon(IntPtr hIcon);

        [DllImport(Win32.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        public static IntPtr GetHandle(this Visual visual) => (PresentationSource.FromVisual(visual) as HwndSource)?.Handle ?? IntPtr.Zero;

        /// <summary>
        /// Win32 GetLayeredWindowAttributes.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="pcrKey"></param>
        /// <param name="pbAlpha"></param>
        /// <param name="pdwFlags"></param>
        /// <returns></returns>
        /// <SecurityNote>
        /// Critical: This code calls into unmanaged code
        /// </SecurityNote>
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport(Win32.User32)]
        public static extern bool GetLayeredWindowAttributes(
                HandleRef hwnd, IntPtr pcrKey, IntPtr pbAlpha, IntPtr pdwFlags);
        internal sealed class SafeFileMappingHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <SecurityNote>
            ///   Critical: base class enforces link demand and inheritance demand
            /// </SecurityNote>
            [SecurityCritical]
            internal SafeFileMappingHandle(IntPtr handle) : base(false)
            {
                SetHandle(handle);
            }

            /// <SecurityNote>
            ///   Critical: base class enforces link demand and inheritance demand
            ///   TreatAsSafe: Creating this is ok, accessing the pointer is bad
            /// </SecurityNote>
            [SecurityCritical, SecurityTreatAsSafe]
            internal SafeFileMappingHandle() : base(true)
            {
            }

            /// <SecurityNote>
            ///   Critical: base class enforces link demand and inheritance demand
            ///   TreatAsSafe: This call is safe
            /// </SecurityNote>
            public override bool IsInvalid
            {
                [SecurityCritical, SecurityTreatAsSafe]
                get
                {
                    return handle == IntPtr.Zero;
                }
            }

            /// <SecurityNote>
            ///     Critical - as this function does an elevation to close a handle.
            ///     TreatAsSafe - as this can at best be used to destabilize one's own app.
            /// </SecurityNote>
            [SecurityCritical, SecurityTreatAsSafe]
            protected override bool ReleaseHandle()
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
                try
                {
                    return Kernel32Interop.CloseHandleNoThrow(new HandleRef(null, handle));
                }
                finally
                {
                    SecurityPermission.RevertAssert();
                }
            }
        }
        ///<SecurityNote>
        /// Critical as this code performs an elevation.
        ///</SecurityNote>
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport(Win32.User32, EntryPoint = "DestroyIcon", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern bool IntDestroyIcon(IntPtr hIcon);

        ///<SecurityNote>
        /// Critical: calls a critical method (IntDestroyIcon)
        ///</SecurityNote>
        [SecurityCritical]
        public static bool DestroyIcon(IntPtr hIcon)
        {
            bool result = IntDestroyIcon(hIcon);
            int error = Marshal.GetLastWin32Error();

            if (!result)
            {
                // To be consistent with out other PInvoke wrappers
                // we should "throw" here.  But we don't want to
                // introduce new "throws" w/o time to follow up on any
                // new problems that causes.
                Debug.WriteLine("DestroyIcon failed.  Error = " + error);
                //throw new Win32Exception();
            }

            return result;
        }
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport(Win32.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto,
            EntryPoint = nameof(CreateIconIndirect))]
        private static extern IconHandle PrivateCreateIconIndirect([In] [MarshalAs(UnmanagedType.LPStruct)]
            Gdi32Interop.ICONINFO iconInfo);

        [SecurityCritical]
        internal static IconHandle CreateIconIndirect([In] [MarshalAs(UnmanagedType.LPStruct)]
            Gdi32Interop.ICONINFO iconInfo)
        {
            var hIcon = PrivateCreateIconIndirect(iconInfo);
            return hIcon;
        }
    }
}
