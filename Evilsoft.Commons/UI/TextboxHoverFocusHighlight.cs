using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EvilsoftCommons.UI {
    public class TextboxHoverFocusHighlight {
        private TextBox textbox;

        public TextboxHoverFocusHighlight(TextBox textbox) {
            this.textbox = textbox;
            textbox.MouseHover += textboxSearch_MouseHover;
            textbox.GotFocus += textboxSearch_GotFocus;
            textbox.LostFocus += textboxSearch_LostFocus;
            textbox.MouseEnter += textboxSearch_MouseEnter;
            textbox.MouseLeave += textboxSearch_MouseLeave;

        }
        void textboxSearch_MouseLeave(object sender, EventArgs e) {
            if (!this.textbox.Focused)
                this.textbox.BackColor = System.Drawing.SystemColors.Window;
        }

        void textboxSearch_MouseEnter(object sender, EventArgs e) {
            this.textbox.BackColor = System.Drawing.Color.AliceBlue;
        }

        void textboxSearch_MouseHover(object sender, EventArgs e) {
            this.textbox.BackColor = System.Drawing.Color.AliceBlue;
        }

        void textboxSearch_LostFocus(object sender, EventArgs e) {
            this.textbox.BackColor = System.Drawing.SystemColors.Window;
        }

        void textboxSearch_GotFocus(object sender, EventArgs e) {
            this.textbox.BackColor = System.Drawing.Color.AliceBlue;
        }

    }
}
