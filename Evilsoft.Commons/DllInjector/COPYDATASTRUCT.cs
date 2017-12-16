using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilsoftCommons.DllInjector {
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct COPYDATASTRUCT {
        public int cbData;
        public IntPtr dwData;
        //[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr)]
        //public char[] lpData;
        public IntPtr lpData;
    }
}
