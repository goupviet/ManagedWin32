using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using ManagedWin32.Api;

namespace ManagedWin32
{
    public sealed class ScreenSaver
    {
        /// <param name="path">The Path to the .src file of the screensaver.</param>
        /// <exception cref="System.IO.FileLoadException">File does not exist or is not a Screensaver.</exception>
        public ScreenSaver(string path)
        {
            if (!File.Exists(path) || !Path.GetExtension(path).ToLower().Equals(".scr")) throw new FileLoadException("File does not exist or is not a Screensaver.");
            this.Source = path;
        }

        /// <summary>
        /// The Path to the .scr file.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Preview the Screensaver.
        /// </summary>
        public void Preview()
        {
            try { Process.Start(Source); }
            catch { }
        }

        /// <summary>
        /// Opens the Configuration Screen.
        /// </summary>
        public void Configure()
        {
            try { Process.Start(new ProcessStartInfo(Source) { Verb = "config" }); }
            catch { }
        }

        /// <summary>
        /// Installs the Screen Saver.
        /// </summary>
        public void Install()
        {
            try { Process.Start(new ProcessStartInfo(Source) { Verb = "install" }); }
            catch { }
        }

        /// <summary>
        /// Checks whether two ScreenSaver objects refer to the same .scr file.
        /// </summary>
        public static bool operator ==(ScreenSaver S1, ScreenSaver S2) { return S1.Source.Equals(S1.Source); }

        /// <summary>
        /// Checks whether two ScreenSaver objects do not refer to the same .scr file.
        /// </summary>
        public static bool operator !=(ScreenSaver S1, ScreenSaver S2) { return !(S1 == S2); }

        /// <summary>
        /// Gets or Sets whether the Screensaver is Active (Enabled, but not necessarily Running).
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                bool isActive = false;
                User32.SystemParametersInfo(SystemInfoParamsAction.GetScreenSaverActive, 0, ref isActive, 0);
                return isActive;
            }
            set
            {
                int nullVar = 0;
                User32.SystemParametersInfo(SystemInfoParamsAction.SetScreenSaverActive, value ? 1 : 0, ref nullVar, User32.SPIF_SENDWININICHANGE);
            }
        }

        /// <summary>
        /// Gets or Sets the Screensaver Timeout Setting in Seconds.
        /// </summary>
        public static int Timeout
        {
            get
            {
                int value = 0;
                User32.SystemParametersInfo(SystemInfoParamsAction.GetScreenSaverTimeout, 0, ref value, 0);
                return value;
            }
            set
            {
                int nullVar = 0;
                User32.SystemParametersInfo(SystemInfoParamsAction.SetScreenSaverTimeout, value, ref nullVar, User32.SPIF_SENDWININICHANGE);
            }
        }

        /// <summary>
        /// Gets or Sets whether the screen saver is actually running.
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                bool isRunning = false;
                User32.SystemParametersInfo(SystemInfoParamsAction.GetScreenSaverRunning, 0, ref isRunning, 0);
                return isRunning;
            }
            set
            {
                if (value && !IsRunning)
                {
                    if (!IsEnabled) IsEnabled = true;

                    const int SysCommand_ScreenSaver = 0xF140;

                    User32.SendMessage(WindowHandler.DesktopWindow.Handle, WindowsMessage.SYSCOMMAND, SysCommand_ScreenSaver, 0);
                }
                else if (!value && IsRunning)
                {
                    using (Desktop hDesktop = new Desktop("Screen-saver"))
                    {
                        if (hDesktop.DesktopHandle != IntPtr.Zero) { foreach (WindowHandler wnd in hDesktop.GetWindows()) if (wnd.IsVisible) wnd.Close(); }
                        else WindowHandler.ForegroundWindow.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Currently active ScreenSaver Object.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">ScreenSaver is not Active or a valid ScreenSaver has not been assigned.</exception>
        /// <exception cref="System.IO.FileLoadException">Registered File does not exist or is not a Screensaver.</exception>
        public static ScreenSaver Current
        {
            get
            {
                string temp = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop").GetValue("SCRNSAVE.EXE", String.Empty).ToString();
                if (!File.Exists(temp) || !Path.GetExtension(temp).ToLower().Equals(".scr"))
                    throw new FileLoadException("Registered File does not exist or is not a Screensaver.");
                if (String.IsNullOrWhiteSpace(temp)) throw new InvalidOperationException("ScreenSaver is not Active or a valid ScreenSaver has not been assigned.");
                return new ScreenSaver(temp);
            }
            set { Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true).SetValue("SCRNSAVE.EXE", value.Source, RegistryValueKind.String); }
        }

        public override bool Equals(object obj) { return obj is ScreenSaver ? this == (ScreenSaver)obj : false; }

        public override string ToString() { return Path.GetFileName(Source); }
    }
}