using System;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool MaskBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth,
           int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, IntPtr hbmMask, int xMask,
           int yMask, uint dwRop);

        [DllImport("gdi32.dll")]
        public static extern int SetBkColor(IntPtr hDC, uint crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, PatBltTypes dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitmapInfo pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
