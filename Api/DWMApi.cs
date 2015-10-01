using System;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    public static class DWMApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(this IntPtr HThumbnail, ref DWMThumbnailProperties props);

        [DllImport("dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(this IntPtr HThumbnail, out Size size);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(this IntPtr hwnd, ref RECT margins);

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(this IntPtr dest, IntPtr source, out IntPtr HThumbnail);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(this IntPtr HThumbnail);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(this IntPtr hwnd, ref DWMBlurbehind blurBehind);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, DwmWindowAttribute dWAttribute, ref RECT pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetColorizationColor(ref int color, [MarshalAs(UnmanagedType.Bool)] ref bool opaque);

        public static bool IsTransparency
        {
            get
            {
                int color = 0;
                bool opaque = true;

                DwmGetColorizationColor(ref color, ref opaque);

                return !opaque;
            }
        }
    }
}
