using System;

namespace Standard
{
    public delegate IntPtr WndProcHook(IntPtr hwnd, WM uMsg, IntPtr wParam, IntPtr lParam, ref bool handled);
}
