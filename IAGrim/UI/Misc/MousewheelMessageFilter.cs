using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.UI.Misc {
    /// <summary>
    /// Pass along mousewheel messages to the hover control (eg control under the mouse pointer)
    /// </summary>
    class MousewheelMessageFilter : IMessageFilter {
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(Point p);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public bool PreFilterMessage(ref Message m) {
            switch (m.Msg) {
                case 0x020A:   // WM_MOUSEWHEEL
                case 0x020E:  //  WM_MOUSEHWHEEL
                    IntPtr hControlUnderMouse = WindowFromPoint(new Point((int)m.LParam));
                    if (hControlUnderMouse != m.HWnd) {
                        SendMessage(hControlUnderMouse, Convert.ToUInt32(m.Msg), m.WParam, m.LParam);
                        return true;
                    }

                    break;
            }

            return false;
            
        }
    }
}
