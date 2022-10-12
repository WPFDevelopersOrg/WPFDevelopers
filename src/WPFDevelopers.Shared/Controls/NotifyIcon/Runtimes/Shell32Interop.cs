using System;
using System.Runtime.InteropServices;
using WPFDevelopers.Controls.Runtimes.Shell32;

namespace WPFDevelopers.Controls.Runtimes
{
    public static class Shell32Interop
    {
        private const string _Shell32 = "shell32.dll";

        [DllImport(_Shell32, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Shell_NotifyIcon([In] NotifyCommand dwMessage, [In] ref NOTIFYICONDATA lpData);


        [DllImport(_Shell32, CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, string iconPath, ref IntPtr index);

    }
}
