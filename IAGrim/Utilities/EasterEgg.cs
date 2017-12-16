using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IAGrim.Utilities {
    /// <summary>
    /// Easter-prank class, all UI text will be reversed on the 1st of April.
    /// Restarting the program will fix it.
    /// </summary>
    class EasterEgg {


        public static void Activate(Control root) {
            if (IsAprilFools()) {
                if ((bool)Properties.Settings.Default.EasterPrank) {
                    It(root.Controls);

                    // Only one prank per year
                    Properties.Settings.Default.EasterPrank = false;
                    Properties.Settings.Default.Save();

                }
            }
            else {
                // Activate for next year..
                Properties.Settings.Default.EasterPrank = true;
                Properties.Settings.Default.Save();
            }

                
        }

        public static bool IsAprilFools() {
            return DateTime.Now.Month == 4 && DateTime.Now.Day == 1;
        }


        private static void It(System.Windows.Forms.Control.ControlCollection collection) {
            foreach (Control control in collection) {
                It(control.Controls);

                TextBox tb = control as TextBox;
                FirefoxCheckBox cb = control as FirefoxCheckBox;
                FirefoxRadioButton rb = control as FirefoxRadioButton;
                GroupBox gb = control as GroupBox;
                FirefoxButton button = control as FirefoxButton;
                PanelBox pb = control as PanelBox;
                Label label = control as Label;

                if (tb != null)
                    tb.Text = Reverse(tb.Text);
                if (cb != null)
                    cb.Text = Reverse(cb.Text);
                if (rb != null)
                    rb.Text = Reverse(rb.Text);
                if (gb != null)
                    gb.Text = Reverse(gb.Text);
                if (button != null)
                    button.Text = Reverse(button.Text);
                if (pb != null)
                    pb.Text = Reverse(pb.Text);
                if (label != null)
                    label.Text = Reverse(label.Text);
            }
        }

        public static string Reverse(string s) {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
