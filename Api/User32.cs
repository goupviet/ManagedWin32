using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    #region Structures
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IconInfo
    {
        public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
        public int xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
        public int yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
        public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
        public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public int cbSize;        // Specifies the size, in bytes, of the structure. 
        public int flags;         // Specifies the cursor state. This parameter can be one of the following values:
        public IntPtr hCursor;          // Handle to the cursor. 
        public POINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
    }

    /// <summary>
    /// Win API WNDCLASS struct - represents a single window.
    /// Used to receive window messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
        public uint style;
        public WindowProcedureHandler lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int X, Y; }

    public enum BitmapImageCompression : int
    {
        RGB = 0,
        RLE8 = 1,
        RLE4 = 2,
        BITFIELDS = 3,
        JPEG = 4,
        BMP = 0,
        PNG = 5,
        UNKNOWN = 255
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BitmapInfoHeader
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public BitmapImageCompression biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;

        public BitmapInfoHeader(Stream stream)
        {
            this = new BitmapInfoHeader();
            this.Read(stream);
        }

        public unsafe void Read(Stream stream)
        {
            byte[] array = new byte[sizeof(BitmapInfoHeader)];
            stream.Read(array, 0, array.Length);
            fixed (byte* pData = array)
                this = *(BitmapInfoHeader*)pData;
        }

        public unsafe void Write(Stream stream)
        {
            byte[] array = new byte[sizeof(BitmapInfoHeader)];
            fixed (BitmapInfoHeader* ptr = &this)
                Marshal.Copy((IntPtr)ptr, array, 0, sizeof(BitmapInfoHeader));
            stream.Write(array, 0, sizeof(BitmapInfoHeader));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BitmapInfo
    {
        public BitmapInfoHeader bmiHeader;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public RGBQuad[] bmiColors;
    }

    /// <summary>
    /// The INPUT structure is used by SendInput to store information for synthesizing input events such as keystrokes, mouse movement, and mouse clicks. (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    /// <remarks>
    /// This structure contains information identical to that used in the parameter list of the keybd_event or mouse_event function.
    /// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard input methods, such as handwriting recognition or voice recognition, as if it were text input by using the KEYEVENTF_UNICODE flag. For more information, see the remarks section of KEYBDINPUT.
    /// </remarks>
    public struct INPUT
    {
        /// <summary>
        /// Specifies the type of the input event. This member can be one of the following values. 
        /// <see cref="InputType.Mouse"/> - The event is a mouse event. Use the mi structure of the union.
        /// <see cref="InputType.Keyboard"/> - The event is a keyboard event. Use the ki structure of the union.
        /// <see cref="InputType.Hardware"/> - Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.
        /// </summary>
        public uint Type;

        /// <summary>
        /// The data structure that contains information about the simulated Mouse, Keyboard or Hardware event.
        /// </summary>
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    /// <summary>
    /// The combined/overlayed structure that includes Mouse, Keyboard and Hardware Input message data (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        /// <summary>
        /// The <see cref="MOUSEINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;

        /// <summary>
        /// The <see cref="KEYBDINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;

        /// <summary>
        /// The <see cref="HARDWAREINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

    /// <summary>
    /// The MOUSEINPUT structure contains information about a simulated mouse event.
    /// </summary>
    /// <remarks>
    /// If the mouse has moved, indicated by MOUSEEVENTF_MOVE, dx and dy specify information about that movement. The information is specified as absolute or relative integer values. 
    /// If MOUSEEVENTF_ABSOLUTE value is specified, dx and dy contain normalized absolute coordinates between 0 and 65,535. The event procedure maps these coordinates onto the display surface. Coordinate (0,0) maps onto the upper-left corner of the display surface; coordinate (65535,65535) maps onto the lower-right corner. In a multimonitor system, the coordinates map to the primary monitor. 
    /// Windows 2000/XP: If MOUSEEVENTF_VIRTUALDESK is specified, the coordinates map to the entire virtual desktop.
    /// If the MOUSEEVENTF_ABSOLUTE value is not specified, dx and dy specify movement relative to the previous mouse event (the last reported position). Positive values mean the mouse moved right (or down); negative values mean the mouse moved left (or up). 
    /// Relative mouse motion is subject to the effects of the mouse speed and the two-mouse threshold values. A user sets these three values with the Pointer Speed slider of the Control Panel's Mouse Properties sheet. You can obtain and set these values using the SystemParametersInfo function. 
    /// The system applies two tests to the specified relative mouse movement. If the specified distance along either the x or y axis is greater than the first mouse threshold value, and the mouse speed is not zero, the system doubles the distance. If the specified distance along either the x or y axis is greater than the second mouse threshold value, and the mouse speed is equal to two, the system doubles the distance that resulted from applying the first threshold test. It is thus possible for the system to multiply specified relative mouse movement along the x or y axis by up to four times.
    /// </remarks>
    public struct MOUSEINPUT
    {
        /// <summary>
        /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
        /// </summary>
        public Int32 X;

        /// <summary>
        /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
        /// </summary>
        public Int32 Y;

        /// <summary>
        /// If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120. 
        /// Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left. One wheel click is defined as WHEEL_DELTA, which is 120.
        /// Windows 2000/XP: IfdwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 
        /// If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 
        /// </summary>
        public UInt32 MouseData;

        /// <summary>
        /// A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. 
        /// The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 
        /// You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
        /// </summary>
        public UInt32 Flags;

        /// <summary>
        /// Time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
        /// </summary>
        public UInt32 Time;

        /// <summary>
        /// Specifies an additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 
        /// </summary>
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// The KEYBDINPUT structure contains information about a simulated keyboard event.  (see: http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    /// <remarks>
    /// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input methodssuch as handwriting recognition or voice recognitionas if it were text input by using the KEYEVENTF_UNICODE flag. If KEYEVENTF_UNICODE is specified, SendInput sends a WM_KEYDOWN or WM_KEYUP message to the foreground thread's message queue with wParam equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, passing the message to TranslateMessage posts a WM_CHAR message with the Unicode character originally specified by wScan. This Unicode character will automatically be converted to the appropriate ANSI value if it is posted to an ANSI window.
    /// Windows 2000/XP: Set the KEYEVENTF_SCANCODE flag to define keyboard input in terms of the scan code. This is useful to simulate a physical keystroke regardless of which keyboard is currently being used. The virtual key value of a key may alter depending on the current keyboard layout or what other keys were pressed, but the scan code will always be the same.
    /// </remarks>
    public struct KEYBDINPUT
    {
        /// <summary>
        /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. The Winuser.h header file provides macro definitions (VK_*) for each value. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
        /// </summary>
        public UInt16 KeyCode;

        /// <summary>
        /// Specifies a hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application. 
        /// </summary>
        public UInt16 Scan;

        /// <summary>
        /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
        /// KEYEVENTF_EXTENDEDKEY - If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
        /// KEYEVENTF_KEYUP - If specified, the key is being released. If not specified, the key is being pressed.
        /// KEYEVENTF_SCANCODE - If specified, wScan identifies the key and wVk is ignored. 
        /// KEYEVENTF_UNICODE - Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section. 
        /// </summary>
        public UInt32 Flags;

        /// <summary>
        /// Time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp. 
        /// </summary>
        public UInt32 Time;

        /// <summary>
        /// Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
        /// </summary>
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse.  (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    public struct HARDWAREINPUT
    {
        /// <summary>
        /// Value specifying the message generated by the input hardware. 
        /// </summary>
        public UInt32 Msg;

        /// <summary>
        /// Specifies the low-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 ParamL;

        /// <summary>
        /// Specifies the high-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 ParamH;
    }
    #endregion

    #region Enums
    public enum SystemInfoParamsAction
    {
        GetScreenSaverActive = 16,
        SetScreenSaverActive = 17,
        GetScreenSaverTimeout = 14,
        SetScreenSaverTimeout = 15,
        GetScreenSaverRunning = 114,

        GETFONTSMOOTHING = 74,

        GETDROPSHADOW = 0x1024,

        GETFLATMENU = 0x1022,

        GETFONTSMOOTHINGTYPE = 0x200a,

        GETFONTSMOOTHINGCONTRAST = 0x200c,

        ICONHORIZONTALSPACING = 13,

        ICONVERTICALSPACING = 24,

        GETICONTITLEWRAP = 25,

        GETICONTITLELOGFONT = 31,

        GETKEYBOARDCUES = 0x100a,

        GETKEYBOARDDELAY = 22,

        GETKEYBOARDPREF = 68,

        GETKEYBOARDSPEED = 10,

        GETMOUSEHOVERWIDTH = 98,

        GETMOUSEHOVERHEIGHT = 100,

        GETMOUSEHOVERTIME = 102,

        GETMOUSESPEED = 112,

        GETMENUDROPALIGNMENT = 27,

        GETMENUFADE = 0x1012,

        GETMENUSHOWDELAY = 106,

        GETCOMBOBOXANIMATION = 0x1004,

        GETGRADIENTCAPTIONS = 0x1008,

        GETHOTTRACKING = 0x100e,

        GETLISTBOXSMOOTHSCROLLING = 0x1006,

        GETMENUANIMATION = 0x1002,

        GETSELECTIONFADE = 0x1014,

        GETTOOLTIPANIMATION = 0x1016,

        GETUIEFFECTS = 0x103e,

        GETACTIVEWINDOWTRACKING = 0x1000,

        GETACTIVEWNDTRKTIMEOUT = 0x2002,

        GETANIMATION = 72,

        GETBORDER = 5,

        GETCARETWIDTH = 0x2006,

        GETDRAGFULLWINDOWS = 38,

        GETNONCLIENTMETRICS = 41,

        GETWORKAREA = 48,

        GETHIGHCONTRAST = 66,

        GETDEFAULTINPUTLANG = 89,

        GETSNAPTODEFBUTTON = 95,

        GETWHEELSCROLLLINES = 104
    }

    public enum WindowStates
    {
        Hidden = 0,
        Normal = 1,
        Minimized = 2,
        Maximized = 3,
        Inactive = 4,
        Restore = 9,
        Default = 10
    }

    /// <summary>Specifies the boot mode in which the system was started.</summary>
    public enum BootMode
    {
        /// <summary>The computer was started in the standard boot mode. This mode uses the normal drivers and settings for the system.</summary>
        Normal,
        /// <summary>The computer was started in safe mode without network support. This mode uses a limited drivers and settings profile.</summary>
        FailSafe,
        /// <summary>The computer was started in safe mode with network support. This mode uses a limited drivers and settings profile, and loads the services needed to start networking.</summary>
        FailSafeWithNetwork
    }

    public enum BeepType
    {
        Beep = 0,
        Hand = 16,
        Question = 32,
        Exclamation = 48,
        Asterisk = 64
    }

    public enum SystemMetrics
    {
        CXSCREEN = 0,
        CYSCREEN = 1,

        CXVSCROLL = 2,
        CYHSCROLL = 3,

        CYCAPTION = 4,

        CXBORDER = 5,
        CYBORDER = 6,

        CYVTHUMB = 9,
        CXHTHUMB = 10,

        CXICON = 11,
        CYICON = 12,

        CXCURSOR = 13,
        CYCURSOR = 14,

        CYMENU = 15,

        CYKANJIWINDOW = 18,

        MOUSEPRESENT = 19,

        CYVSCROLL = 20,
        CXHSCROLL = 21,

        DEBUG = 22,

        SWAPBUTTON = 23,

        CXMIN = 28,

        CYMIN = 29,

        CXSIZE = 30,
        CYSIZE = 31,

        CXFRAME = 32,
        CYFRAME = 33,

        CYFOCUSBORDER = 84,
        CXFOCUSBORDER = 83,

        CYSIZEFRAME = 33,
        CXSIZEFRAME = 32,

        CXMINTRACK = 34,
        CYMINTRACK = 35,

        CXDOUBLECLK = 36,
        CYDOUBLECLK = 37,

        CXICONSPACING = 38,

        CYICONSPACING = 39,

        MENUDROPALIGNMENT = 40,

        PENWINDOWS = 41,

        DBCSENABLED = 42,

        CMOUSEBUTTONS = 43,

        CXFIXEDFRAME = 7,

        CYFIXEDFRAME = 8,

        SECURE = 44,

        CXEDGE = 45,
        CYEDGE = 46,

        CXMINSPACING = 47,
        CYMINSPACING = 48,

        CXSMICON = 49,
        CYSMICON = 50,

        CYSMCAPTION = 51,

        CXSMSIZE = 52,

        CYSMSIZE = 53,

        CXMENUSIZE = 54,

        CYMENUSIZE = 55,

        ARRANGE = 56,

        CXMINIMIZED = 57,

        CYMINIMIZED = 58,

        CXMAXTRACK = 59,

        CYMAXTRACK = 60,

        CXMAXIMIZED = 61,

        CYMAXIMIZED = 62,

        NETWORK = 63,

        CLEANBOOT = 67,

        CXDRAG = 68,
        CYDRAG = 69,

        SHOWSOUNDS = 70,

        CXMENUCHECK = 71,
        CYMENUCHECK = 72,

        MIDEASTENABLED = 74,

        MOUSEWHEELPRESENT = 75,

        XVIRTUALSCREEN = 76,
        YVIRTUALSCREEN = 77,

        CXVIRTUALSCREEN = 78,
        CYVIRTUALSCREEN = 79,

        CMONITORS = 80,

        SAMEDISPLAYFORMAT = 81,

        REMOTESESSION = 0x1000
    }

    public enum DesktopAccessRights
    {
        CreateWindow = 0x0002,
        Enumerate = 0x0040,
        WriteObjects = 0x0080,
        SwitchDesktop = 0x0100,
        CreateMenu = 0x0004,
        HookControl = 0x0008,
        ReadObjects = 0x0001,
        JournalRecord = 0x0010,
        JournalPlayback = 0x0020,
        AllRights = JournalRecord | JournalPlayback | CreateWindow
            | Enumerate | WriteObjects | SwitchDesktop
            | CreateMenu | HookControl | ReadObjects
    }

    /// <summary>
    /// Modifier Key Codes
    /// </summary>
    [Flags]
    public enum ModifierKeyCodes : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }

    /// <summary>
    /// Virtual Key Codes
    /// </summary>
    public enum VirtualKeyCodes : uint
    {
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90
    }

    public enum KeyCode
    {
        /// <summary>
        /// Left mouse button
        /// </summary>
        LBUTTON = 0x01,

        /// <summary>
        /// Right mouse button
        /// </summary>
        RBUTTON = 0x02,

        /// <summary>
        /// Control-break processing
        /// </summary>
        CANCEL = 0x03,

        /// <summary>
        /// Middle mouse button (three-button mouse) - NOT contiguous with LBUTTON and RBUTTON
        /// </summary>
        MBUTTON = 0x04,

        /// <summary>
        /// Windows 2000/XP: X1 mouse button - NOT contiguous with LBUTTON and RBUTTON
        /// </summary>
        XBUTTON1 = 0x05,

        /// <summary>
        /// Windows 2000/XP: X2 mouse button - NOT contiguous with LBUTTON and RBUTTON
        /// </summary>
        XBUTTON2 = 0x06,

        // 0x07 : Undefined

        /// <summary>
        /// BACKSPACE key
        /// </summary>
        BACK = 0x08,

        /// <summary>
        /// TAB key
        /// </summary>
        TAB = 0x09,

        // 0x0A - 0x0B : Reserved

        /// <summary>
        /// CLEAR key
        /// </summary>
        CLEAR = 0x0C,

        /// <summary>
        /// ENTER key
        /// </summary>
        RETURN = 0x0D,

        // 0x0E - 0x0F : Undefined

        /// <summary>
        /// SHIFT key
        /// </summary>
        SHIFT = 0x10,

        /// <summary>
        /// CTRL key
        /// </summary>
        CONTROL = 0x11,

        /// <summary>
        /// ALT key
        /// </summary>
        MENU = 0x12,

        /// <summary>
        /// PAUSE key
        /// </summary>
        PAUSE = 0x13,

        /// <summary>
        /// CAPS LOCK key
        /// </summary>
        CAPITAL = 0x14,

        /// <summary>
        /// Input Method Editor (IME) Kana mode
        /// </summary>
        KANA = 0x15,

        /// <summary>
        /// IME Hanguel mode (maintained for compatibility; use HANGUL)
        /// </summary>
        HANGEUL = 0x15,

        /// <summary>
        /// IME Hangul mode
        /// </summary>
        HANGUL = 0x15,

        // 0x16 : Undefined

        /// <summary>
        /// IME Junja mode
        /// </summary>
        JUNJA = 0x17,

        /// <summary>
        /// IME final mode
        /// </summary>
        FINAL = 0x18,

        /// <summary>
        /// IME Hanja mode
        /// </summary>
        HANJA = 0x19,

        /// <summary>
        /// IME Kanji mode
        /// </summary>
        KANJI = 0x19,

        // 0x1A : Undefined

        /// <summary>
        /// ESC key
        /// </summary>
        ESCAPE = 0x1B,

        /// <summary>
        /// IME convert
        /// </summary>
        CONVERT = 0x1C,

        /// <summary>
        /// IME nonconvert
        /// </summary>
        NONCONVERT = 0x1D,

        /// <summary>
        /// IME accept
        /// </summary>
        ACCEPT = 0x1E,

        /// <summary>
        /// IME mode change request
        /// </summary>
        MODECHANGE = 0x1F,

        /// <summary>
        /// SPACEBAR
        /// </summary>
        SPACE = 0x20,

        /// <summary>
        /// PAGE UP key
        /// </summary>
        PRIOR = 0x21,

        /// <summary>
        /// PAGE DOWN key
        /// </summary>
        NEXT = 0x22,

        /// <summary>
        /// END key
        /// </summary>
        END = 0x23,

        /// <summary>
        /// HOME key
        /// </summary>
        HOME = 0x24,

        /// <summary>
        /// LEFT ARROW key
        /// </summary>
        LEFT = 0x25,

        /// <summary>
        /// UP ARROW key
        /// </summary>
        UP = 0x26,

        /// <summary>
        /// RIGHT ARROW key
        /// </summary>
        RIGHT = 0x27,

        /// <summary>
        /// DOWN ARROW key
        /// </summary>
        DOWN = 0x28,

        /// <summary>
        /// SELECT key
        /// </summary>
        SELECT = 0x29,

        /// <summary>
        /// PRINT key
        /// </summary>
        PRINT = 0x2A,

        /// <summary>
        /// EXECUTE key
        /// </summary>
        EXECUTE = 0x2B,

        /// <summary>
        /// PRINT SCREEN key
        /// </summary>
        SNAPSHOT = 0x2C,

        /// <summary>
        /// INS key
        /// </summary>
        INSERT = 0x2D,

        /// <summary>
        /// DEL key
        /// </summary>
        DELETE = 0x2E,

        /// <summary>
        /// HELP key
        /// </summary>
        HELP = 0x2F,

        /// <summary>
        /// 0 key
        /// </summary>
        VK_0 = 0x30,

        /// <summary>
        /// 1 key
        /// </summary>
        VK_1 = 0x31,

        /// <summary>
        /// 2 key
        /// </summary>
        VK_2 = 0x32,

        /// <summary>
        /// 3 key
        /// </summary>
        VK_3 = 0x33,

        /// <summary>
        /// 4 key
        /// </summary>
        VK_4 = 0x34,

        /// <summary>
        /// 5 key
        /// </summary>
        VK_5 = 0x35,

        /// <summary>
        /// 6 key
        /// </summary>
        VK_6 = 0x36,

        /// <summary>
        /// 7 key
        /// </summary>
        VK_7 = 0x37,

        /// <summary>
        /// 8 key
        /// </summary>
        VK_8 = 0x38,

        /// <summary>
        /// 9 key
        /// </summary>
        VK_9 = 0x39,

        //
        // 0x3A - 0x40 : Udefined
        //

        /// <summary>
        /// A key
        /// </summary>
        VK_A = 0x41,

        /// <summary>
        /// B key
        /// </summary>
        VK_B = 0x42,

        /// <summary>
        /// C key
        /// </summary>
        VK_C = 0x43,

        /// <summary>
        /// D key
        /// </summary>
        VK_D = 0x44,

        /// <summary>
        /// E key
        /// </summary>
        VK_E = 0x45,

        /// <summary>
        /// F key
        /// </summary>
        VK_F = 0x46,

        /// <summary>
        /// G key
        /// </summary>
        VK_G = 0x47,

        /// <summary>
        /// H key
        /// </summary>
        VK_H = 0x48,

        /// <summary>
        /// I key
        /// </summary>
        VK_I = 0x49,

        /// <summary>
        /// J key
        /// </summary>
        VK_J = 0x4A,

        /// <summary>
        /// K key
        /// </summary>
        VK_K = 0x4B,

        /// <summary>
        /// L key
        /// </summary>
        VK_L = 0x4C,

        /// <summary>
        /// M key
        /// </summary>
        VK_M = 0x4D,

        /// <summary>
        /// N key
        /// </summary>
        VK_N = 0x4E,

        /// <summary>
        /// O key
        /// </summary>
        VK_O = 0x4F,

        /// <summary>
        /// P key
        /// </summary>
        VK_P = 0x50,

        /// <summary>
        /// Q key
        /// </summary>
        VK_Q = 0x51,

        /// <summary>
        /// R key
        /// </summary>
        VK_R = 0x52,

        /// <summary>
        /// S key
        /// </summary>
        VK_S = 0x53,

        /// <summary>
        /// T key
        /// </summary>
        VK_T = 0x54,

        /// <summary>
        /// U key
        /// </summary>
        VK_U = 0x55,

        /// <summary>
        /// V key
        /// </summary>
        VK_V = 0x56,

        /// <summary>
        /// W key
        /// </summary>
        VK_W = 0x57,

        /// <summary>
        /// X key
        /// </summary>
        VK_X = 0x58,

        /// <summary>
        /// Y key
        /// </summary>
        VK_Y = 0x59,

        /// <summary>
        /// Z key
        /// </summary>
        VK_Z = 0x5A,

        /// <summary>
        /// Left Windows key (Microsoft Natural keyboard)
        /// </summary>
        LWIN = 0x5B,

        /// <summary>
        /// Right Windows key (Natural keyboard)
        /// </summary>
        RWIN = 0x5C,

        /// <summary>
        /// Applications key (Natural keyboard)
        /// </summary>
        APPS = 0x5D,

        // 0x5E : reserved

        /// <summary>
        /// Computer Sleep key
        /// </summary>
        SLEEP = 0x5F,

        /// <summary>
        /// Numeric keypad 0 key
        /// </summary>
        NUMPAD0 = 0x60,

        /// <summary>
        /// Numeric keypad 1 key
        /// </summary>
        NUMPAD1 = 0x61,

        /// <summary>
        /// Numeric keypad 2 key
        /// </summary>
        NUMPAD2 = 0x62,

        /// <summary>
        /// Numeric keypad 3 key
        /// </summary>
        NUMPAD3 = 0x63,

        /// <summary>
        /// Numeric keypad 4 key
        /// </summary>
        NUMPAD4 = 0x64,

        /// <summary>
        /// Numeric keypad 5 key
        /// </summary>
        NUMPAD5 = 0x65,

        /// <summary>
        /// Numeric keypad 6 key
        /// </summary>
        NUMPAD6 = 0x66,

        /// <summary>
        /// Numeric keypad 7 key
        /// </summary>
        NUMPAD7 = 0x67,

        /// <summary>
        /// Numeric keypad 8 key
        /// </summary>
        NUMPAD8 = 0x68,

        /// <summary>
        /// Numeric keypad 9 key
        /// </summary>
        NUMPAD9 = 0x69,

        /// <summary>
        /// Multiply key
        /// </summary>
        MULTIPLY = 0x6A,

        /// <summary>
        /// Add key
        /// </summary>
        ADD = 0x6B,

        /// <summary>
        /// Separator key
        /// </summary>
        SEPARATOR = 0x6C,

        /// <summary>
        /// Subtract key
        /// </summary>
        SUBTRACT = 0x6D,

        /// <summary>
        /// Decimal key
        /// </summary>
        DECIMAL = 0x6E,

        /// <summary>
        /// Divide key
        /// </summary>
        DIVIDE = 0x6F,

        /// <summary>
        /// F1 key
        /// </summary>
        F1 = 0x70,

        /// <summary>
        /// F2 key
        /// </summary>
        F2 = 0x71,

        /// <summary>
        /// F3 key
        /// </summary>
        F3 = 0x72,

        /// <summary>
        /// F4 key
        /// </summary>
        F4 = 0x73,

        /// <summary>
        /// F5 key
        /// </summary>
        F5 = 0x74,

        /// <summary>
        /// F6 key
        /// </summary>
        F6 = 0x75,

        /// <summary>
        /// F7 key
        /// </summary>
        F7 = 0x76,

        /// <summary>
        /// F8 key
        /// </summary>
        F8 = 0x77,

        /// <summary>
        /// F9 key
        /// </summary>
        F9 = 0x78,

        /// <summary>
        /// F10 key
        /// </summary>
        F10 = 0x79,

        /// <summary>
        /// F11 key
        /// </summary>
        F11 = 0x7A,

        /// <summary>
        /// F12 key
        /// </summary>
        F12 = 0x7B,

        /// <summary>
        /// F13 key
        /// </summary>
        F13 = 0x7C,

        /// <summary>
        /// F14 key
        /// </summary>
        F14 = 0x7D,

        /// <summary>
        /// F15 key
        /// </summary>
        F15 = 0x7E,

        /// <summary>
        /// F16 key
        /// </summary>
        F16 = 0x7F,

        /// <summary>
        /// F17 key
        /// </summary>
        F17 = 0x80,

        /// <summary>
        /// F18 key
        /// </summary>
        F18 = 0x81,

        /// <summary>
        /// F19 key
        /// </summary>
        F19 = 0x82,

        /// <summary>
        /// F20 key
        /// </summary>
        F20 = 0x83,

        /// <summary>
        /// F21 key
        /// </summary>
        F21 = 0x84,

        /// <summary>
        /// F22 key
        /// </summary>
        F22 = 0x85,

        /// <summary>
        /// F23 key
        /// </summary>
        F23 = 0x86,

        /// <summary>
        /// F24 key
        /// </summary>
        F24 = 0x87,

        //
        // 0x88 - 0x8F : Unassigned
        //

        /// <summary>
        /// NUM LOCK key
        /// </summary>
        NUMLOCK = 0x90,

        /// <summary>
        /// SCROLL LOCK key
        /// </summary>
        SCROLL = 0x91,

        // 0x92 - 0x96 : OEM Specific

        // 0x97 - 0x9F : Unassigned

        //
        // L* & R* - left and right Alt, Ctrl and Shift virtual keys.
        // Used only as parameters to GetAsyncKeyState() and GetKeyState().
        // No other API or message will distinguish left and right keys in this way.
        //

        /// <summary>
        /// Left SHIFT key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        LSHIFT = 0xA0,

        /// <summary>
        /// Right SHIFT key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        RSHIFT = 0xA1,

        /// <summary>
        /// Left CONTROL key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        LCONTROL = 0xA2,

        /// <summary>
        /// Right CONTROL key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        RCONTROL = 0xA3,

        /// <summary>
        /// Left MENU key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        LMENU = 0xA4,

        /// <summary>
        /// Right MENU key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        RMENU = 0xA5,

        /// <summary>
        /// Windows 2000/XP: Browser Back key
        /// </summary>
        BROWSER_BACK = 0xA6,

        /// <summary>
        /// Windows 2000/XP: Browser Forward key
        /// </summary>
        BROWSER_FORWARD = 0xA7,

        /// <summary>
        /// Windows 2000/XP: Browser Refresh key
        /// </summary>
        BROWSER_REFRESH = 0xA8,

        /// <summary>
        /// Windows 2000/XP: Browser Stop key
        /// </summary>
        BROWSER_STOP = 0xA9,

        /// <summary>
        /// Windows 2000/XP: Browser Search key
        /// </summary>
        BROWSER_SEARCH = 0xAA,

        /// <summary>
        /// Windows 2000/XP: Browser Favorites key
        /// </summary>
        BROWSER_FAVORITES = 0xAB,

        /// <summary>
        /// Windows 2000/XP: Browser Start and Home key
        /// </summary>
        BROWSER_HOME = 0xAC,

        /// <summary>
        /// Windows 2000/XP: Volume Mute key
        /// </summary>
        VOLUME_MUTE = 0xAD,

        /// <summary>
        /// Windows 2000/XP: Volume Down key
        /// </summary>
        VOLUME_DOWN = 0xAE,

        /// <summary>
        /// Windows 2000/XP: Volume Up key
        /// </summary>
        VOLUME_UP = 0xAF,

        /// <summary>
        /// Windows 2000/XP: Next Track key
        /// </summary>
        MEDIA_NEXT_TRACK = 0xB0,

        /// <summary>
        /// Windows 2000/XP: Previous Track key
        /// </summary>
        MEDIA_PREV_TRACK = 0xB1,

        /// <summary>
        /// Windows 2000/XP: Stop Media key
        /// </summary>
        MEDIA_STOP = 0xB2,

        /// <summary>
        /// Windows 2000/XP: Play/Pause Media key
        /// </summary>
        MEDIA_PLAY_PAUSE = 0xB3,

        /// <summary>
        /// Windows 2000/XP: Start Mail key
        /// </summary>
        LAUNCH_MAIL = 0xB4,

        /// <summary>
        /// Windows 2000/XP: Select Media key
        /// </summary>
        LAUNCH_MEDIA_SELECT = 0xB5,

        /// <summary>
        /// Windows 2000/XP: Start Application 1 key
        /// </summary>
        LAUNCH_APP1 = 0xB6,

        /// <summary>
        /// Windows 2000/XP: Start Application 2 key
        /// </summary>
        LAUNCH_APP2 = 0xB7,

        //
        // 0xB8 - 0xB9 : Reserved
        //

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the ';:' key 
        /// </summary>
        OEM_1 = 0xBA,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the '+' key
        /// </summary>
        OEM_PLUS = 0xBB,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the ',' key
        /// </summary>
        OEM_COMMA = 0xBC,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the '-' key
        /// </summary>
        OEM_MINUS = 0xBD,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the '.' key
        /// </summary>
        OEM_PERIOD = 0xBE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '/?' key 
        /// </summary>
        OEM_2 = 0xBF,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '`~' key 
        /// </summary>
        OEM_3 = 0xC0,

        //
        // 0xC1 - 0xD7 : Reserved
        //

        //
        // 0xD8 - 0xDA : Unassigned
        //

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '[{' key
        /// </summary>
        OEM_4 = 0xDB,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '\|' key
        /// </summary>
        OEM_5 = 0xDC,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the ']}' key
        /// </summary>
        OEM_6 = 0xDD,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the 'single-quote/double-quote' key
        /// </summary>
        OEM_7 = 0xDE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        OEM_8 = 0xDF,

        //
        // 0xE0 : Reserved
        //

        //
        // 0xE1 : OEM Specific
        //

        /// <summary>
        /// Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
        /// </summary>
        OEM_102 = 0xE2,

        //
        // (0xE3-E4) : OEM specific
        //

        /// <summary>
        /// Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        /// </summary>
        PROCESSKEY = 0xE5,

        //
        // 0xE6 : OEM specific
        //

        /// <summary>
        /// Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
        /// </summary>
        PACKET = 0xE7,

        //
        // 0xE8 : Unassigned
        //

        //
        // 0xE9-F5 : OEM specific
        //

        /// <summary>
        /// Attn key
        /// </summary>
        ATTN = 0xF6,

        /// <summary>
        /// CrSel key
        /// </summary>
        CRSEL = 0xF7,

        /// <summary>
        /// ExSel key
        /// </summary>
        EXSEL = 0xF8,

        /// <summary>
        /// Erase EOF key
        /// </summary>
        EREOF = 0xF9,

        /// <summary>
        /// Play key
        /// </summary>
        PLAY = 0xFA,

        /// <summary>
        /// Zoom key
        /// </summary>
        ZOOM = 0xFB,

        /// <summary>
        /// Reserved
        /// </summary>
        NONAME = 0xFC,

        /// <summary>
        /// PA1 key
        /// </summary>
        PA1 = 0xFD,

        /// <summary>
        /// Clear key
        /// </summary>
        OEM_CLEAR = 0xFE,
    }

    [Flags]
    public enum ShutdownFlags
    {
        //Shuts down all processes running in the logon session of the process that called the ExitWindowsEx function. 
        //Then it logs the user off.
        //This flag can be used only by processes running in an interactive user's logon session.
        LogOff = 0x00000000,

        //Shuts down the system to a point at which it is safe to turn off the power. 
        //All file buffers have been flushed to disk, and all running processes have stopped. 
        Shutdown = 0x00000001,

        //Shuts down the system and then restarts the system. 
        Reboot = 0x00000002,

        //This flag has no effect if terminal services is enabled. 
        //Otherwise, the system does not send the WM_QUERYENDSESSION message. 
        //This can cause applications to lose data. 
        //Therefore, you should only use this flag in an emergency.
        Force = 0x00000004,

        //Shuts down the system and turns off the power. The system must support the power-off feature. 
        PowerOff = 0x00000008,

        //Forces processes to terminate if they do not respond to the WM_QUERYENDSESSION or WM_ENDSESSION message within the timeout interval.
        ForceIfHung = 0x00000010,

        //Shuts down the system and then restarts it, 
        //as well as any applications that have been registered for restart 
        //using the RegisterApplicationRestart function.
        //These application receive the WM_QUERYENDSESSION message with lParam 
        //set to the ENDSESSION_CLOSEAPP value.
        RestartApps = 0x00000040,

        //Beginning with Windows 8:  You can prepare the system for a faster startup 
        //by combining the EWX_HYBRID_SHUTDOWN flag with the EWX_SHUTDOWN flag. 
        HybridShutdown = 0x00400000
    }

    public enum GetWindowEnum
    {
        First = 0,
        Last = 1,
        Next = 2,
        Previous = 3,
        Owner = 4,
        Child = 5,
        EnabledPopup = 6
    }

    public enum SetWindowPositionFlags
    {
        NoMove = 0x2,
        NoSize = 1,
        NoZOrder = 0x4,
        ShowWindow = 0x400
    }

    public enum PatBltTypes
    {
        SRCCOPY = 0x00CC0020,
        SRCPAINT = 0x00EE0086,
        SRCAND = 0x008800C6,
        SRCINVERT = 0x00660046,
        SRCERASE = 0x00440328,
        NOTSRCCOPY = 0x00330008,
        NOTSRCERASE = 0x001100A6,
        MERGECOPY = 0x00C000CA,
        MERGEPAINT = 0x00BB0226,
        PATCOPY = 0x00F00021,
        PATPAINT = 0x00FB0A09,
        PATINVERT = 0x005A0049,
        DSTINVERT = 0x00550009,
        BLACKNESS = 0x00000042,
        WHITENESS = 0x00FF0062
    }

    public enum WindowsMessage
    {
        NULL = 0,
        CREATE = 1,
        DESTROY = 2,
        MOVE = 3,
        SIZE = 5,
        ACTIVATE = 6,
        SETFOCUS = 7,
        KILLFOCUS = 8,
        ENABLE = 10,
        SETREDRAW = 11,
        SETTEXT = 12,
        GETTEXT = 13,
        GETTEXTLENGTH = 14,
        PAINT = 15,
        Close = 16,
        QUERYENDSESSION = 17,
        QUIT = 18,
        QUERYOPEN = 19,
        ERASEBKGND = 20,
        SYSCOLORCHANGE = 21,
        ENDSESSION = 22,
        SHOWWINDOW = 24,
        CTLCOLOR = 25,
        SETTINGCHANGE = 26,
        WININICHANGE = 26,
        DEVMODECHANGE = 27,
        ACTIVATEAPP = 28,
        FONTCHANGE = 29,
        TIMECHANGE = 30,
        CANCELMODE = 31,
        SETCURSOR = 32,
        TABLET_MAXOFFSET = 32,
        MOUSEACTIVATE = 33,
        CHILDACTIVATE = 34,
        QUEUESYNC = 35,
        GETMINMAXINFO = 36,
        PAINTICON = 38,
        ICONERASEBKGND = 39,
        NEXTDLGCTL = 40,
        SPOOLERSTATUS = 42,
        DRAWITEM = 43,
        MEASUREITEM = 44,
        DELETEITEM = 45,
        VKEYTOITEM = 46,
        CHARTOITEM = 47,
        SETFONT = 48,
        GETFONT = 49,
        SETHOTKEY = 50,
        GETHOTKEY = 51,
        QUERYDRAGICON = 55,
        COMPAREITEM = 57,
        GETOBJECT = 61,
        COMPACTING = 65,
        COMMNOTIFY = 68,
        WINDOWPOSCHANGING = 70,
        WINDOWPOSCHANGED = 71,
        POWER = 72,
        COPYDATA = 74,
        CANCELJOURNAL = 75,
        NOTIFY = 78,
        INPUTLANGCHANGEREQUEST = 80,
        INPUTLANGCHANGE = 81,
        TCARD = 82,
        HELP = 83,
        USERCHANGED = 84,
        NOTIFYFORMAT = 85,
        CONTEXTMENU = 123,
        STYLECHANGING = 124,
        STYLECHANGED = 125,
        DISPLAYCHANGE = 126,
        GETICON = 127,
        SETICON = 128,
        NCCREATE = 129,
        NCDESTROY = 130,
        NCCALCSIZE = 131,
        NCHITTEST = 132,
        NCPAINT = 133,
        NCACTIVATE = 134,
        GETDLGCODE = 135,
        SYNCPAINT = 136,
        MOUSEQUERY = 155,
        NCMOUSEMOVE = 160,
        NCLBUTTONDOWN = 161,
        NCLBUTTONUP = 162,
        NCLBUTTONDBLCLK = 163,
        NCRBUTTONDOWN = 164,
        NCRBUTTONUP = 165,
        NCRBUTTONDBLCLK = 166,
        NCMBUTTONDOWN = 167,
        NCMBUTTONUP = 168,
        NCMBUTTONDBLCLK = 169,
        NCXBUTTONDOWN = 171,
        NCXBUTTONUP = 172,
        NCXBUTTONDBLCLK = 173,
        INPUT = 255,
        KEYDOWN = 256,
        KEYFIRST = 256,
        KEYUP = 257,
        CHAR = 258,
        DEADCHAR = 259,
        SYSKEYDOWN = 260,
        SYSKEYUP = 261,
        SYSCHAR = 262,
        SYSDEADCHAR = 263,
        KEYLAST = 264,
        IME_STARTCOMPOSITION = 269,
        IME_ENDCOMPOSITION = 270,
        IME_COMPOSITION = 271,
        IME_KEYLAST = 271,
        INITDIALOG = 272,
        COMMAND = 273,
        SYSCOMMAND = 274,
        TIMER = 275,
        HSCROLL = 276,
        VSCROLL = 277,
        INITMENU = 278,
        INITMENUPOPUP = 279,
        MENUSELECT = 287,
        MENUCHAR = 288,
        ENTERIDLE = 289,
        UNINITMENUPOPUP = 293,
        CHANGEUISTATE = 295,
        UPDATEUISTATE = 296,
        QUERYUISTATE = 297,
        CTLCOLORMSGBOX = 306,
        CTLCOLOREDIT = 307,
        CTLCOLORLISTBOX = 308,
        CTLCOLORBTN = 309,
        CTLCOLORDLG = 310,
        CTLCOLORSCROLLBAR = 311,
        CTLCOLORSTATIC = 312,
        MOUSEFIRST = 512,
        MOUSEMOVE = 512,
        LBUTTONDOWN = 513,
        LBUTTONUP = 514,
        LBUTTONDBLCLK = 515,
        RBUTTONDOWN = 516,
        RBUTTONUP = 517,
        RBUTTONDBLCLK = 518,
        MBUTTONDOWN = 519,
        MBUTTONUP = 520,
        MBUTTONDBLCLK = 521,
        MOUSEWHEEL = 522,
        XBUTTONDOWN = 523,
        XBUTTONUP = 524,
        XBUTTONDBLCLK = 525,
        MOUSEHWHEEL = 526,
        MOUSELAST = 526,
        PARENTNOTIFY = 528,
        ENTERMENULOOP = 529,
        EXITMENULOOP = 530,
        NEXTMENU = 531,
        SIZING = 532,
        CAPTURECHANGED = 533,
        MOVING = 534,
        POWERBROADCAST = 536,
        DEVICECHANGE = 537,
        MDICREATE = 544,
        MDIDESTROY = 545,
        MDIACTIVATE = 546,
        MDIRESTORE = 547,
        MDINEXT = 548,
        MDIMAXIMIZE = 549,
        MDITILE = 550,
        MDICASCADE = 551,
        MDIICONARRANGE = 552,
        MDIGETACTIVE = 553,
        MDISETMENU = 560,
        ENTERSIZEMOVE = 561,
        EXITSIZEMOVE = 562,
        DROPFILES = 563,
        MDIREFRESHMENU = 564,
        IME_SETCONTEXT = 641,
        IME_NOTIFY = 642,
        IME_CONTROL = 643,
        IME_COMPOSITIONFULL = 644,
        IME_SELECT = 645,
        IME_CHAR = 646,
        IME_REQUEST = 648,
        IME_KEYDOWN = 656,
        IME_KEYUP = 657,
        MOUSEHOVER = 673,
        NCMOUSELEAVE = 674,
        MOUSELEAVE = 675,
        WTSSESSION_CHANGE = 689,
        TABLET_DEFBASE = 704,
        TABLET_ADDED = 712,
        TABLET_DELETED = 713,
        TABLET_FLICK = 715,
        TABLET_QUERYSYSTEMGESTURESTATUS = 716,
        CUT = 768,
        COPY = 769,
        PASTE = 770,
        CLEAR = 771,
        UNDO = 772,
        RENDERFORMAT = 773,
        RENDERALLFORMATS = 774,
        DESTROYCLIPBOARD = 775,
        DRAWCLIPBOARD = 776,
        PAINTCLIPBOARD = 777,
        VSCROLLCLIPBOARD = 778,
        SIZECLIPBOARD = 779,
        ASKCBFORMATNAME = 780,
        CHANGECBCHAIN = 781,
        HSCROLLCLIPBOARD = 782,
        QUERYNEWPALETTE = 783,
        PALETTEISCHANGING = 784,
        PALETTECHANGED = 785,
        HOTKEY = 786,
        PRINT = 791,
        PRINTCLIENT = 792,
        APPCOMMAND = 793,
        THEMECHANGED = 794,
        DWMCOMPOSITIONCHANGED = 798,
        DWMNCRENDERINGCHANGED = 799,
        DWMCOLORIZATIONCOLORCHANGED = 800,
        DWMWINDOWMAXIMIZEDCHANGE = 801,
        DWMSENDICONICTHUMBNAIL = 803,
        DWMSENDICONICLIVEPREVIEWBITMAP = 806,
        HANDHELDFIRST = 856,
        HANDHELDLAST = 863,
        AFXFIRST = 864,
        AFXLAST = 895,
        PENWINFIRST = 896,
        PENWINLAST = 911,
        USER = 1024,
        APP = 32768
    }
    #endregion

    #region Delegates
    public delegate bool EnumDesktopWindowsProc(IntPtr hDesktop, IntPtr lParam);

    /// <summary>
    /// Callback delegate which is used by the Windows API to
    /// submit window messages.
    /// </summary>
    public delegate IntPtr WindowProcedureHandler(IntPtr hwnd, uint uMsg, IntPtr wparam, IntPtr lparam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate bool EnumDesktopProc(string lpszDesktop, IntPtr lParam);
    #endregion

    public class DeviceContext : IDisposable
    {
        public IntPtr Handle { get; private set; }
        WindowHandler Window;

        DeviceContext(IntPtr hdc) { Handle = hdc; }

        internal DeviceContext(WindowHandler win)
        {
            Window = win;
            Handle = User32.GetWindowDC(win.Handle);
        }

        public static DeviceContext CreateCompatible(DeviceContext dc) { return new DeviceContext(Gdi32.CreateCompatibleDC(dc.Handle)); }

        public static bool BitBlt(DeviceContext Destination, int nXDest, int nYDest, int nWidth, int nHeight, DeviceContext Source, int nXSrc, int nYSrc, PatBltTypes dwRop)
        {
            return Gdi32.BitBlt(Destination.Handle, nXDest, nYDest, nWidth, nHeight, Source.Handle, nXSrc, nYSrc, dwRop);
        }

        /// <returns>Previous Selection</returns>
        public hBitmap SelectObject(hBitmap bitmap) { return new hBitmap(Gdi32.SelectObject(Handle, bitmap.Handle)); }

        public void Dispose()
        {
            if (Window != null) User32.ReleaseDC(Handle, Window.Handle);
            else Gdi32.DeleteDC(Handle);
        }
    }

    public class hBitmap : IDisposable
    {
        public IntPtr Handle { get; private set; }

        internal hBitmap(IntPtr Handle) { this.Handle = Handle; }

        public static hBitmap CreateCompatible(DeviceContext dc, int Width, int Height) { return new hBitmap(Gdi32.CreateCompatibleBitmap(dc.Handle, Width, Height)); }

        public Image Image { get { return Image.FromHbitmap(Handle); } }

        public Bitmap Bitmap { get { return Bitmap.FromHbitmap(Handle); } }

        public void Dispose() { Gdi32.DeleteObject(Handle); }
    }

    public class SystemParams
    {
        public static BootMode BootMode { get { return (BootMode)User32.GetSystemMetrics(SystemMetrics.CLEANBOOT); } }

        public static bool IsNetworkConnected { get { return (User32.GetSystemMetrics(SystemMetrics.NETWORK) & 1) != 0; } }

        public static int ScreenWidth { get { return User32.GetSystemMetrics(SystemMetrics.CXSCREEN); } }

        public static int ScreenHeight { get { return User32.GetSystemMetrics(SystemMetrics.CYSCREEN); } }
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

        [DllImport("shell32.dll", EntryPoint = "#62", SetLastError = true)]
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

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
    }
}
