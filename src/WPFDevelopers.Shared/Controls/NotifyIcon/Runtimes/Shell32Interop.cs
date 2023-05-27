using System;
using System.Runtime.InteropServices;
using WPFDevelopers.Controls.Runtimes.Shell32;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls.Runtimes
{
    public static class Shell32Interop
    {

        [DllImport(Win32.Shell32, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Shell_NotifyIcon([In] NotifyCommand dwMessage, [In] ref NOTIFYICONDATA lpData);


        [DllImport(Win32.Shell32, CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, string iconPath, ref IntPtr index);

    }
}
