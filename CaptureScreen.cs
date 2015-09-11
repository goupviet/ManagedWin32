using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ManagedWin32.Api;

namespace ManagedWin32
{
    public static class Screen
    {
        public static int Width { get { return User32.GetSystemMetrics(SystemMetrics.CXSCREEN); } }

        public static int Height { get { return User32.GetSystemMetrics(SystemMetrics.CYSCREEN); } }
    }

    public static class ScreenCapture
    {
        public static Bitmap CaptureDesktop()
        {
            hBitmap hBitmap;
            using (DeviceContext hDC = WindowHandler.DesktopWindow.DeviceContext)
            {
                using (var hMemDC = DeviceContext.CreateCompatible(hDC))
                {
                    using (hBitmap = hBitmap.CreateCompatible(hDC, Screen.Width, Screen.Height))
                    {
                        if (hBitmap.Handle != IntPtr.Zero)
                        {
                            hBitmap hOld = hMemDC.SelectObject(hBitmap);

                            DeviceContext.BitBlt(hMemDC, 0, 0, Screen.Width, Screen.Height, hDC, 0, 0, PatBltTypes.SRCCOPY);

                            hMemDC.SelectObject(hOld);
                            return hBitmap.Bitmap;
                        }
                        return null;
                    }
                }
            }
        }

        public static Bitmap CaptureCursor(ref int x, ref int y)
        {
            Bitmap bmp;
            IntPtr hicon;
            CursorInfo ci = new CursorInfo() { cbSize = Marshal.SizeOf(typeof(CursorInfo)) };

            IconInfo icInfo;

            if (User32.GetCursorInfo(out ci))
            {
                if (ci.flags == User32.CURSOR_SHOWING)
                {
                    hicon = User32.CopyIcon(ci.hCursor);
                    if (User32.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.X - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.Y - ((int)icInfo.yHotspot);

                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();
                        return bmp;
                    }
                }
            }

            return null;
        }

        public static Bitmap CaptureDesktopWithCursor()
        {
            int cursorX = 0, cursorY = 0;
            Bitmap desktopBMP, cursorBMP;
            Graphics g;
            Rectangle r;

            desktopBMP = CaptureDesktop();
            cursorBMP = CaptureCursor(ref cursorX, ref cursorY);
            if (desktopBMP != null)
            {
                if (cursorBMP != null)
                {
                    r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
                    g = Graphics.FromImage(desktopBMP);
                    g.DrawImage(cursorBMP, r);
                    g.Flush();

                    return desktopBMP;
                }
                else return desktopBMP;
            }

            return null;
        }

        public static Bitmap Capture(this WindowHandler Window)
        {
            using (var SourceDC = Window.DeviceContext)
            {
                // create a device context we can copy to
                using (var MemoryDC = DeviceContext.CreateCompatible(SourceDC))
                {
                    var size = Window.Size;
                    int Width = size.Width, Height = size.Height;

                    // create a bitmap we can copy it to,
                    // using GetDeviceCaps to get the width/height
                    using (var hBmp = hBitmap.CreateCompatible(SourceDC, Width, Height))
                    {
                        // select the bitmap object
                        hBitmap hOld = MemoryDC.SelectObject(hBmp);

                        // bitblt over
                        DeviceContext.BitBlt(MemoryDC, 0, 0, Width, Height, SourceDC, 0, 0, PatBltTypes.SRCCOPY);

                        // restore selection
                        MemoryDC.SelectObject(hOld);

                        // get a .NET image object for it
                        return hBmp.Bitmap;
                    }
                }
            }
        }

        public static Bitmap CaptureScreen()
        {
            var bitmap = new Bitmap(Screen.Width, Screen.Height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(Screen.Width, Screen.Height));
                return bitmap;
            }
        }
    }
}
