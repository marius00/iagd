using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace IAGrim.Theme {
    internal class ScrollPanelMessageFilter : IMessageFilter, IDisposable {
        int WM_MOUSEWHEEL = 0x20A;
        Control control;
        bool panelHasFocus = false;

        [DllImport("User32.dll")]
        static extern Int32 SendMessage(int hWnd, int Msg, int wParam, int lParam);

        public ScrollPanelMessageFilter(Control form) {
            this.control = form;
            //Go through each control on the panel and add an event handler.
            //We need to know if a control on the panel has focus to prevent sending
            //the scroll message a second time
            AddFocusEvent(form);
        }

        private void AddFocusEvent(Control parentControl) {
            foreach (Control control in parentControl.Controls) {
                if (control.Controls.Count == 0) {
                    control.GotFocus += new EventHandler(control_GotFocus);
                    control.LostFocus += new EventHandler(control_LostFocus);
                }
                else {
                    AddFocusEvent(control);
                }
            }
        }

        void control_GotFocus(object sender, EventArgs e) {
            panelHasFocus = true;
        }

        void control_LostFocus(object sender, EventArgs e) {
            panelHasFocus = false;
        }

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m) {
            //filter out all other messages except than mouse wheel
            //also only proceed with processing if the panel is focusable, no controls on the panel have focus 
            //and the vertical scroll bar is visible

            bool verticalScroll = false;
            Form asForm = control as Form;
            Panel asPanel = control as Panel;
            
            if (asForm != null)
                verticalScroll = asForm.VerticalScroll.Visible;

            else if (asPanel != null)
                verticalScroll = asPanel.VerticalScroll.Visible;

            if (m.Msg == WM_MOUSEWHEEL && control.CanFocus && !panelHasFocus && verticalScroll) {
                //is mouse coordinates over the panel display rectangle?
                System.Drawing.Rectangle rect = control.RectangleToScreen(control.ClientRectangle);
                System.Drawing.Point cursorPoint = System.Windows.Forms.Cursor.Position;//new Point();
                //GetCursorPos(ref cursorPoint);
                if ((cursorPoint.X > rect.X && cursorPoint.X < rect.X + rect.Width) &&
                    (cursorPoint.Y > rect.Y && cursorPoint.Y < rect.Y + rect.Height)) {
                    //send the mouse wheel message to the panel.
                    SendMessage((int)control.Handle, m.Msg, (Int32)m.WParam, (Int32)m.LParam);
                    return true;
                }
            }
            return false;
        }
        #endregion

        public void Dispose() {
            Application.RemoveMessageFilter(this);
        }
    }
}
