using System;
using System.ComponentModel;
using System.Text;
using ManagedWin32.Api;
using Microsoft.Win32;

namespace ManagedWin32
{
    public class IconPickerDialog : CommonDialog
    {
        const int  MAX_PATH = 260;

        [DefaultValue(default(string))]
        public string FileName { get; set; }

        [DefaultValue(0)]
        public int IconIndex { get; set; }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            var buf = new StringBuilder(FileName, MAX_PATH);
            int index;

            bool ok = User32.SHPickIconDialog(hwndOwner, buf, MAX_PATH, out index);
            if (ok)
            {
                FileName = Environment.ExpandEnvironmentVariables(buf.ToString());
                IconIndex = index;
            }

            return ok;
        }

        public override void Reset()
        {
            FileName = null;
            IconIndex = 0;
        }
    }
}