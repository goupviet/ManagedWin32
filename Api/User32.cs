using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public enum EventObjects : int
    {
        OBJID_ALERT = -10,
        OBJID_CARET = -8,
        OBJID_CLIENT = -4,
        OBJID_CURSOR = -9,
        OBJID_HSCROLL = -6,
        OBJID_MENU = -3,
        OBJID_SIZEGRIP = -7,
        OBJID_SOUND = -11,
        OBJID_SYSMENU = -1,
        OBJID_TITLEBAR = -2,
        OBJID_VSCROLL = -5,
        OBJID_WINDOW = 0
    }

    public delegate void WinEventDelegate(IntPtr hWinEventHook, WinEvent eventType, IntPtr hwnd, EventObjects idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    public enum WinEventHookFlags : int
    {
        WINEVENT_SKIPOWNTHREAD = 1,
        WINEVENT_SKIPOWNPROCESS = 2,
        WINEVENT_OUTOFCONTEXT = 0,
        WINEVENT_INCONTEXT = 4
    }

    public enum WinEvent : uint
    {
        EVENT_OBJECT_ACCELERATORCHANGE = 32786,
        EVENT_OBJECT_CREATE = 32768,
        EVENT_OBJECT_DESTROY = 32769,
        EVENT_OBJECT_DEFACTIONCHANGE = 32785,
        EVENT_OBJECT_DESCRIPTIONCHANGE = 32781,
        EVENT_OBJECT_FOCUS = 32773,
        EVENT_OBJECT_HELPCHANGE = 32784,
        EVENT_OBJECT_SHOW = 32770,
        EVENT_OBJECT_HIDE = 32771,
        EVENT_OBJECT_LOCATIONCHANGE = 32779,
        EVENT_OBJECT_NAMECHANGE = 32780,
        EVENT_OBJECT_PARENTCHANGE = 32783,
        EVENT_OBJECT_REORDER = 32772,
        EVENT_OBJECT_SELECTION = 32774,
        EVENT_OBJECT_SELECTIONADD = 32775,
        EVENT_OBJECT_SELECTIONREMOVE = 32776,
        EVENT_OBJECT_SELECTIONWITHIN = 32777,
        EVENT_OBJECT_STATECHANGE = 32778,
        EVENT_OBJECT_VALUECHANGE = 32782,
        EVENT_SYSTEM_ALERT = 2,
        EVENT_SYSTEM_CAPTUREEND = 9,
        EVENT_SYSTEM_CAPTURESTART = 8,
        EVENT_SYSTEM_CONTEXTHELPEND = 13,
        EVENT_SYSTEM_CONTEXTHELPSTART = 12,
        EVENT_SYSTEM_DIALOGEND = 17,
        EVENT_SYSTEM_DIALOGSTART = 16,
        EVENT_SYSTEM_DRAGDROPEND = 15,
        EVENT_SYSTEM_DRAGDROPSTART = 14,
        EVENT_SYSTEM_FOREGROUND = 3,
        EVENT_SYSTEM_MENUEND = 5,
        EVENT_SYSTEM_MENUPOPUPEND = 7,
        EVENT_SYSTEM_MENUPOPUPSTART = 6,
        EVENT_SYSTEM_MENUSTART = 4,
        EVENT_SYSTEM_MINIMIZEEND = 23,
        EVENT_SYSTEM_MINIMIZESTART = 22,
        EVENT_SYSTEM_MOVESIZEEND = 11,
        EVENT_SYSTEM_MOVESIZESTART = 10,
        EVENT_SYSTEM_SCROLLINGEND = 19,
        EVENT_SYSTEM_SCROLLINGSTART = 18,
        EVENT_SYSTEM_SOUND = 1,
        EVENT_SYSTEM_SWITCHEND = 21,
        EVENT_SYSTEM_SWITCHSTART = 20
    }

    public enum Orientation
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum ScrollBarDirection : int
    {
        SB_HORZ = 0,
        SB_VERT = 1,
        SB_CTL = 2,
        SB_BOTH = 3
    }

    public enum RegionResult
    {
        REGION_ERROR = 0,
        REGION_NULLREGION = 1,
        REGION_SIMPLEREGION = 2,
        REGION_COMPLEXREGION = 3
    }

    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SCROLLINFO
    {
        public int cbSize;
        public int fMask;
        public int nMin;
        public int nMax;
        public int nPage;
        public int nPos;
        public int nTrackPos;
    }

    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct WindowInfo
    {
        public uint cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        public WindowInfo(Boolean? filler)
            : this()
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WindowInfo)));
        }
    }

    [Flags]
    public enum WindowPlacementFlags : uint
    {
        // The coordinates of the minimized window may be specified.
        // This flag must be specified if the coordinates are set in the ptMinPosition member.
        WPF_SETMINPOSITION = 0x0001,
        // If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        WPF_ASYNCWINDOWPLACEMENT = 0x0004,
        // The restored window will be maximized, regardless of whether it was maximized before it was minimized. This setting is only valid the next time the window is restored. It does not change the default restoration behavior.
        // This flag is only valid when the SW_SHOWMINIMIZED value is specified for the showCmd member.
        WPF_RESTORETOMAXIMIZED = 0x0002
    }

    public enum ShowWindowCommand : uint
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window
        /// for the first time.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        ShowMaximized = 3,
        /// <summary>
        /// Displays a window in its most recent size and position. This value
        /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
        /// the window is not actived.
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level
        /// window in the Z order.
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is
        /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
        /// window is not activated.
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Sets the show state based on the SW_* value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        /// that owns the window is not responding. This flag should only be
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }

    /// <summary>
    /// Contains information about the placement of a window on the screen.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct WindowPlacement
    {
        /// <summary>
        /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
        /// <para>
        /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
        /// </para>
        /// </summary>
        public int Length;

        /// <summary>
        /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
        /// </summary>
        public WindowPlacementFlags Flags;

        /// <summary>
        /// The current show state of the window.
        /// </summary>
        public ShowWindowCommand ShowCmd;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is minimized.
        /// </summary>
        public POINT MinPosition;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is maximized.
        /// </summary>
        public POINT MaxPosition;

        /// <summary>
        /// The window's coordinates when the window is in the restored position.
        /// </summary>
        public RECT NormalPosition;

        /// <summary>
        /// Gets the default (empty) value.
        /// </summary>
        public static WindowPlacement Default
        {
            get
            {
                WindowPlacement result = new WindowPlacement();
                result.Length = Marshal.SizeOf(result);
                return result;
            }
        }
    }

    public static class User32
    {
        public const int SPIF_SENDWININICHANGE = 2;

        public const int WHEEL_DELTA = 120;

        public const int CURSOR_SHOWING = 0x00000001;

        #region System Parameters Info
        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(SystemInfoParamsAction uAction, int uParam, ref int lpvParam, int flags);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(SystemInfoParamsAction uAction, int uParam, ref bool lpvParam, int flags);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(SystemInfoParamsAction uAction, int uParam, IntPtr lpvParam, int flags);
        #endregion

        #region Desktop
        [DllImport("user32.dll")]
        public static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags, DesktopAccessRights dwDesiredAccess, IntPtr lpsa);

        [DllImport("user32.dll")]
        public static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        public static extern IntPtr OpenDesktop(string lpszDesktop, int dwFlags, bool fInherit, DesktopAccessRights dwDesiredAccess);

        [DllImport("user32.dll")]
        public static extern IntPtr OpenInputDesktop(int dwFlags, bool fInherit, DesktopAccessRights dwDesiredAccess);

        [DllImport("user32.dll")]
        public static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktops(IntPtr hwinsta, EnumDesktopProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool SetThreadDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        public static extern IntPtr GetThreadDesktop(int dwThreadId);
        #endregion

        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern int LookupIconIdFromDirectory(IntPtr presbits, bool fIcon);

        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern int LookupIconIdFromDirectoryEx(IntPtr presbits, bool fIcon, int cxDesired, int cyDesired, LookupIconIdFromDirectoryExFlags Flags);

        [DllImport("user32.dll", EntryPoint = "LoadImageW", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr LoadImage(IntPtr hInstance, IntPtr lpszName, LoadImageTypes imageType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("user32.dll")]
        public static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll")]
        public static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex, IntPtr pvInfo, int nLength, ref int lpnLengthNeeded);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetSystemMetrics(SystemMetrics nIndex);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool ExitWindowsEx(ShutdownFlags Flag, int Reason);

        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        [DllImport("shell32.dll", EntryPoint = "#62", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SHPickIconDialog(IntPtr hWnd, StringBuilder pszFilename, int cchFilenameMax, out int pnIconIndex);

        #region HotKey
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeyCodes fdModifiers, VirtualKeyCodes vk);
        #endregion

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, WindowsMessage wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowsMessage Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);

        #region Windows Input
        /// <summary>
        /// The SendInput function synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        /// <param name="numberOfInputs">Number of structures in the Inputs array.</param>
        /// <param name="inputs">Pointer to an array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
        /// <param name="sizeOfInputStructure">Specifies the size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.</param>
        /// <returns>The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns zero, the input was already blocked by another thread. To get extended error information, call GetLastError.Microsoft Windows Vista. This function fails when it is blocked by User Interface Privilege Isolation (UIPI). Note that neither GetLastError nor the return value will indicate the failure was caused by UIPI blocking.</returns>
        /// <remarks>
        /// Microsoft Windows Vista. This function is subject to UIPI. Applications are permitted to inject input only into applications that are at an equal or lesser integrity level.
        /// The SendInput function inserts the events in the INPUT structures serially into the keyboard or mouse input stream. These events are not interspersed with other keyboard or mouse input events inserted either by the user (with the keyboard or mouse) or by calls to keybd_event, mouse_event, or other calls to SendInput.
        /// This function does not reset the keyboard's current state. Any keys that are already pressed when the function is called might interfere with the events that this function generates. To avoid this problem, check the keyboard's state with the GetAsyncKeyState function and correct as necessary.
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        /// <summary>
        /// The GetAsyncKeyState function determines whether a key is up or down at the time the function is called, and whether the key was pressed after a previous call to GetAsyncKeyState. (See: http://msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        /// </summary>
        /// <param name="virtualKeyCode">Specifies one of 256 possible virtual-key codes. For more information, see Virtual Key Codes. Windows NT/2000/XP: You can use left- and right-distinguishing constants to specify certain keys. See the Remarks section for further information.</param>
        /// <returns>
        /// If the function succeeds, the return value specifies whether the key was pressed since the last call to GetAsyncKeyState, and whether the key is currently up or down. If the most significant bit is set, the key is down, and if the least significant bit is set, the key was pressed after the previous call to GetAsyncKeyState. However, you should not rely on this last behavior; for more information, see the Remarks. 
        /// 
        /// Windows NT/2000/XP: The return value is zero for the following cases: 
        /// - The current desktop is not the active desktop
        /// - The foreground thread belongs to another process and the desktop does not allow the hook or the journal record.
        /// 
        /// Windows 95/98/Me: The return value is the global asynchronous key state for each virtual key. The system does not check which thread has the keyboard focus.
        /// 
        /// Windows 95/98/Me: Windows 95 does not support the left- and right-distinguishing constants. If you call GetAsyncKeyState with these constants, the return value is zero. 
        /// </returns>
        /// <remarks>
        /// The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it is mapped to the left or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling 
        /// Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        /// 
        /// Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState and receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) and should not be relied upon.
        /// 
        /// You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. 
        /// 
        /// Windows NT/2000/XP: You can use the following virtual-key code constants as values for vKey to distinguish between the left and right instances of those keys. 
        /// 
        /// Code Meaning 
        /// VK_LSHIFT Left-shift key. 
        /// VK_RSHIFT Right-shift key. 
        /// VK_LCONTROL Left-control key. 
        /// VK_RCONTROL Right-control key. 
        /// VK_LMENU Left-menu key. 
        /// VK_RMENU Right-menu key. 
        /// 
        /// These left- and right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern short GetAsyncKeyState(ushort virtualKeyCode);

        /// <summary>
        /// The GetKeyState function retrieves the status of the specified virtual key. The status specifies whether the key is up, down, or toggled (on, off alternating each time the key is pressed). (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// </summary>
        /// <param name="virtualKeyCode">
        /// Specifies a virtual key. If the desired virtual key is a letter or digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that character. For other keys, it must be a virtual-key code. 
        /// If a non-English keyboard layout is used, virtual keys with values in the range ASCII A through Z and 0 through 9 are used to specify most of the character keys. For example, for the German keyboard layout, the virtual key of value ASCII O (0x4F) refers to the "o" key, whereas VK_OEM_1 refers to the "o with umlaut" key.
        /// </param>
        /// <returns>
        /// The return value specifies the status of the specified virtual key, as follows: 
        /// If the high-order bit is 1, the key is down; otherwise, it is up.
        /// If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key, is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.
        /// </returns>
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        /// An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        /// To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        /// An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys. 
        /// VK_LSHIFT
        /// VK_RSHIFT
        /// VK_LCONTROL
        /// VK_RCONTROL
        /// VK_LMENU
        /// VK_RMENU
        /// 
        /// These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern short GetKeyState(ushort virtualKeyCode);
        #endregion

        /// <summary>
        /// The GetMessageExtraInfo function retrieves the extra message information for the current thread. Extra message information is an application- or driver-defined value associated with the current thread's message queue. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>To set a thread's extra message information, use the SetMessageExtraInfo function. </remarks>
        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

        /// <summary>
        /// Creates the helper window that receives messages from the taskar icon.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, int dwStyle, int x, int y,
            int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance,
            IntPtr lpParam);

        /// <summary>
        /// Processes a default windows procedure.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wparam, IntPtr lparam);

        /// <summary>
        /// Registers the helper window class.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass(ref WindowClass lpWndClass);

        /// <summary>
        /// Registers a listener for a window message.
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW")]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr WindowFromDC(HandleRef hDC);

        /// <summary>
        /// Used to destroy the hidden helper window that receives messages from the
        /// taskbar icon.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        #region DoubleClickTime
        /// <summary>
        /// Gets the maximum number of milliseconds that can elapse between a
        /// first click and a second click for the OS to consider the
        /// mouse action a double-click.
        /// </summary>
        /// <returns>The maximum amount of time, in milliseconds, that can
        /// elapse between a first click and a second click for the OS to
        /// consider the mouse action a double-click.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern int GetDoubleClickTime();

        public static int DoubleClickTime { get { return GetDoubleClickTime(); } }
        #endregion

        /// <summary>
        /// Gets the screen coordinates of the current mouse position.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetPhysicalCursorPos(ref POINT lpPoint);

        public static POINT PhysicalCursorPosition
        {
            get
            {
                var P = new POINT();
                GetPhysicalCursorPos(ref P);
                return P;
            }
        }

        #region Cursor Position
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetCursorPos(ref POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        public static POINT CursorPosition
        {
            get
            {
                var P = new POINT();
                GetCursorPos(ref P);
                return P;
            }
            set { SetCursorPos(value.X, value.Y); }
        }
        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, EntryPoint = "MessageBeep")]
        public static extern bool Play(BeepType BeepType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Beep(int Frequency, int Duration);

        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport("user32.dll")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        #region Window
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, WindowStates nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowEnum uCmd);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        #region Window Text
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder title = new StringBuilder(GetWindowTextLength(hWnd) + 1);
            GetWindowText(hWnd, title, title.Capacity);
            return title.ToString();
        }

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);
        #endregion

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowModuleFileName(IntPtr hWnd, [Out] StringBuilder module, int size);

        [DllImport("user32.dll")]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWND);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPositionFlags wFlags);
        #endregion

        [DllImport("user32.dll")]
        public static extern WindowStyles GetWindowLong(IntPtr hWnd, GetWindowLongValue nIndex);

        [DllImport("user32.dll")]
        public static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32")]
        public static extern bool AnimateWindow(IntPtr hwnd, int time, AnimateWindowFlags flags);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32", SetLastError = true)]
        public static extern int ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

        [DllImport("user32", SetLastError = true)]
        public static extern uint GetSysColor(int nIndex);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        public extern static int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClassLong(IntPtr hWnd, int nIndex);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32", SetLastError = true)]
        public extern static int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", SetLastError = true, EntryPoint = "SendMessageA")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32", SetLastError = true)]
        public extern static uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32", SetLastError = true)]
        public extern static uint GetWindowLongPtr(IntPtr hwnd, int nIndex);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int index, int styleFlags);

        [DllImport("user32", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int index, IntPtr styleFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WindowInfo pwi);

        [DllImport("user32", SetLastError = true)]
        public extern static int EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32", SetLastError = true)]
        public extern static int EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hwnd, ScrollBarDirection scrollBar, bool show);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetScrollPos(IntPtr hWnd, Orientation nBar, int nPos, bool bRedraw);

        [DllImport("user32", SetLastError = true, EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32", SetLastError = true)]
        public static extern RegionResult GetWindowRgn(IntPtr hWnd, SafeHandle hRgn);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern void ReleaseDC(IntPtr dc);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClipboardOwner();

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        // Added for WinEventHook logic, Greenshot 1.2
        [DllImport("user32", SetLastError = true)]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetWinEventHook(WinEvent eventMin, WinEvent eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, int idProcess, int idThread, WinEventHookFlags dwFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// uiFlags: 0 - Count of GDI objects
        /// uiFlags: 1 - Count of USER objects
        /// - Win32 GDI objects (pens, brushes, fonts, palettes, regions, device contexts, bitmap headers)
        /// - Win32 USER objects:
        ///	- 	WIN32 resources (accelerator tables, bitmap resources, dialog box templates, font resources, menu resources, raw data resources, string table entries, message table entries, cursors/icons)
        /// - Other USER objects (windows, menus)
        ///
        [DllImport("user32", SetLastError = true)]
        public static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        [DllImport("user32", SetLastError = true)]
        public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);
        
        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);
    }
}
