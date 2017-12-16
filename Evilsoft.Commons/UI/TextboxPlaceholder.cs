using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvilsoftCommons.UI {
    public class TextboxPlaceholder {
        private TextBoxBase textbox;
        private string placeholder;
        private Color originalColor;

        public TextboxPlaceholder(TextBoxBase textbox, string placeholder) {
            textbox.GotFocus += textbox_GotFocus;
            textbox.LostFocus += textbox_LostFocus;
            this.textbox = textbox;
            this.placeholder = placeholder;
            originalColor = textbox.ForeColor;

            textbox_LostFocus(null, null);
        }

        void textbox_LostFocus(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(textbox.Text)) {
                textbox.Text = placeholder;
                textbox.ForeColor = Color.LightGray;
            }
        }

        void textbox_GotFocus(object sender, EventArgs e) {
            if (textbox.Text.Equals(placeholder)) {
                textbox.Text = "";
                textbox.ForeColor = originalColor;
            }
        }
    }
}
