using System;

namespace ManagedWin32.Api
{
    #region User32
    public delegate bool EnumDesktopWindowsProc(IntPtr hDesktop, IntPtr lParam);

    /// <summary>
    /// Callback delegate which is used by the Windows API to
    /// submit window messages.
    /// </summary>
    public delegate IntPtr WindowProcedureHandler(IntPtr hwnd, uint uMsg, IntPtr wparam, IntPtr lparam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate bool EnumDesktopProc(string lpszDesktop, IntPtr lParam);
    #endregion

    public delegate int EnumResTypeProc(IntPtr hModule, IntPtr lpszType, IntPtr lParam);
    public delegate bool EnumResNameProc(IntPtr hModule, IntPtr pType, IntPtr pName, IntPtr param);
}
