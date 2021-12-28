using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilsoftCommons.DllInjector {
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct COPYDATASTRUCT {
        public int cbData;
        public IntPtr dwData;
        public IntPtr lpData;
    }
}
