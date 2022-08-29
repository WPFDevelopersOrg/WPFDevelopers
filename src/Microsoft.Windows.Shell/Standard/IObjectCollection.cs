using System;
using System.Runtime.InteropServices;

namespace Standard
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("92CA9DCD-5622-4bba-A805-5E9F541BD8C9")]
    [ComImport]
    public interface IObjectCollection : IObjectArray
    {
        //new uint GetCount();

        //[return: MarshalAs(UnmanagedType.IUnknown)]
        //object GetAt([In] uint uiIndex, [In] ref Guid riid);

        void AddObject([MarshalAs(UnmanagedType.IUnknown)] object punk);

        void AddFromArray(IObjectArray poaSource);

        void RemoveObjectAt(uint uiIndex);

        void Clear();
    }
}
