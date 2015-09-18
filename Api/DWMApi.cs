using System;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    #region Structures
    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_ThumbnailProperties
    {
        public DWM_ThumbnailFlags Flags;
        public DWM_Rect Destination;
        public DWM_Rect Source;
        public Byte Opacity;
        public bool Visible;
        public bool SourceClientAreaOnly;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_Rect { public int Left, Top, Right, Bottom; }

    public struct DWM_Size { public int Width, Height; }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_Blurbehind
    {
        /// <summary>Flags that indicates the given parameters</summary>
        public DWM_BlurbehindFlags dwFlags;
        /// <summary>True if the transparency is enabled</summary>
        public bool fEnable;
        /// <summary>Region</summary>
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }
    #endregion

    #region Enumerations
    [Flags]
    public enum DWM_ThumbnailFlags : uint
    {
        RectDestination = 1,
        RectSource = 2,
        Opacity = 4,
        Visible = 8,
        SourceClientAreaOnly = 0x10
    }

    [Flags]
    public enum DWM_BlurbehindFlags : uint
    {
        /// <summary>Flag Transparency Enabled</summary>
        Enable = 1,
        /// <summary>Flag Region</summary>
        BlurRegion = 2,
        /// <summary>Flag Transition on maximized</summary>
        TransitionOnMaximized = 4
    }
    #endregion

    public static class DWMApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(this IntPtr HThumbnail, ref DWM_ThumbnailProperties props);

        [DllImport("dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(this IntPtr HThumbnail, out DWM_Size size);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(this IntPtr hwnd, ref DWM_Rect margins);

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(this IntPtr dest, IntPtr source, out IntPtr HThumbnail);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(this IntPtr HThumbnail);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(this IntPtr hwnd, ref DWM_Blurbehind blurBehind);
    }
}
