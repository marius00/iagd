using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace EvilsoftCommons {
    public class TooltipHelper : IDisposable {
        private ToolTip _tooltip;
        private ToolTip Tooltip {
            get {
                if (_tooltip == null)
                    _tooltip = new ToolTip();

                return _tooltip;
            }
        }


        ~TooltipHelper() {
            Dispose();
        }

        /// <summary>
        /// Show a tooltip at a given control
        /// </summary>
        /// <param name="control"></param>
        public void ShowTooltipForControl(string text, Control control, bool focus = true) {
            Point p = new Point(0, -(control.Height + 20));

            Debug.Assert(control as Form == null);

            Tooltip.IsBalloon = true;
            Tooltip.Show(text, control, p, 2000);
            if (focus)
                control.Focus();
        }

        public enum TooltipLocation {
            LEFT, TOP, RIGHT, BOTTOM
        }

        public void ShowTooltipForControl(string text, Control control, TooltipLocation location) {
            Point p;
            switch (location) {
                case TooltipLocation.TOP:
                    p = new Point(0, -(control.Height + 20));
                    break;
                case TooltipLocation.LEFT:
                    p = new Point(-20, 0);
                    break;
                case TooltipLocation.RIGHT:
                    p = new Point(control.Width + 5, 0);
                    break;
                case TooltipLocation.BOTTOM:
                default:
                    p = new Point(0, (control.Height + 20));
                    break;
            }
            Debug.Assert(control as Form == null);

            Tooltip.IsBalloon = location == TooltipLocation.TOP;
            Tooltip.Show(text, control, p, 2000);
        }



        public void ShowTooltipAtMouse(string text, Control control) {
            Point p = control.PointToClient(Cursor.Position);
            p.Y = p.Y - 12 * 2;

            Debug.Assert(control as Form == null);

            //tooltip.IsBalloon = true;
            Tooltip.Show(text, control, p, 2000);
        }

        public void Dispose() {
            if (_tooltip != null) {
                _tooltip.RemoveAll();
                _tooltip.Dispose();
                _tooltip = null;
            }
        }
    }

}