using System.Drawing;
using System.Windows.Forms;

namespace IAGrim.UI {
    static class DarkMode {
        public static void Activate(Control root) {
            It(root.Controls);
        }

        private static void It(Control.ControlCollection collection) {
            foreach (Control control in collection) {
                It(control.Controls);

                TextBox tb = control as TextBox;
                FirefoxCheckBox cb = control as FirefoxCheckBox;
                FirefoxRadioButton rb = control as FirefoxRadioButton;
                GroupBox gb = control as GroupBox;
                FirefoxButton button = control as FirefoxButton;
                PanelBox pb = control as PanelBox;
                Label label = control as Label;

                Color black = Color.Black;
                Color white = Color.White;
                if (tb != null) {
                    tb.BackColor = black;
                    tb.ForeColor = Color.LightGray;
                }

                if (cb != null) {
                    cb.ForeColor = white;
                }

                if (rb != null) {
                }

                if (gb != null) {
                }

                if (button != null) {
                    button.BackColor = black;
                    button.ForeColor = white;
                }

                if (pb != null) {
                    pb.BackColor = black;
                    pb.ForeColor = white;
                }

                if (label != null) {
                    label.BackColor = black;
                    label.ForeColor = Color.LightGray;
                }

                control.BackColor = black;
                control.Invalidate();

            }
        }
    }
}
