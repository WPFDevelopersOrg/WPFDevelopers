using System;
using System.Runtime.InteropServices;

namespace Standard
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [ComImport]
    public interface ITaskbarList4 : ITaskbarList3, ITaskbarList2, ITaskbarList
    {
        void SetTabProperties(IntPtr hwndTab, STPF stpFlags);
    }
}
