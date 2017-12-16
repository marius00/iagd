using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EvilsoftCommons.UI {
    /// <summary>
    /// Helper class for tying 3 input fields into a single DateTime
    /// </summary>
    class DateFieldVerifier {
        private TextBox day;
        private TextBox month;
        private TextBox year;
        private Control lastFocus;

        public DateTime? Result {
            get {
                int d;
                int m;
                int y;
                if (int.TryParse(day.Text, out d) && int.TryParse(month.Text, out m) && int.TryParse(year.Text, out y)) {
                    bool isD = d > 0 && d <= 31;
                    bool isM = m > 0 && m <= 12;
                    bool isY = y > 1890 && y < 3000;

                    if (isM && isY && isD && DateTime.DaysInMonth(y, m) <= d)
                        return new DateTime(y, m, d);
                }
                
                return null;
            }
            set {
                day.Text = value.Value.Day.ToString();
                month.Text = value.Value.Month.ToString();
                year.Text = value.Value.Year.ToString();
            }
        }

        public DateFieldVerifier(TextBox day, TextBox month, TextBox year, Control lastFocus = null) {
            this.day = day;
            this.month = month;
            this.year = year;
            this.lastFocus = lastFocus;

            day.KeyPress += tbDob_KeyPress;
            month.KeyPress += tbDob_KeyPress;
            year.KeyPress += tbDob_KeyPress;

            day.TextChanged += day_TextChanged;
            month.TextChanged += month_TextChanged;
            year.TextChanged += year_TextChanged;

            day.MaxLength = 2;
            month.MaxLength = 2;
            year.MaxLength = 4;
        }

        void day_TextChanged(object sender, EventArgs e) {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length >= 2) {
                int value = int.Parse(tb.Text);
                if (value > 31 || value < 0) {
                    tb.SelectionStart = 0;
                    tb.SelectionLength = tb.Text.Length;
                }
                else {
                    month.Focus();
                }
            }
        }

        void month_TextChanged(object sender, EventArgs e) {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length >= 2) {
                int value = int.Parse(tb.Text);
                if (value > 12 || value < 0) {
                    tb.SelectionStart = 0;
                    tb.SelectionLength = tb.Text.Length;
                }
                else {
                    year.Focus();
                }
            }
        }

        void year_TextChanged(object sender, EventArgs e) {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length >= 4) {
                int value = int.Parse(tb.Text);
                if (value > 3000 || value < 1890) {
                    tb.SelectionStart = 0;
                    tb.SelectionLength = tb.Text.Length;
                }
                // Verify that we don't exceed the days (only verifiable once we got both month and year)
                else {
                    int d;
                    int m;

                    if (int.TryParse(day.Text, out d) && d <= 31) {
                        if (int.TryParse(month.Text, out m) && m <= 12) {
                            if (DateTime.DaysInMonth(value, m) < d) {
                                day.Focus();
                                day.SelectionStart = 0;
                                day.SelectionLength = day.Text.Length;
                            }
                            else if (lastFocus != null) {
                                lastFocus.Focus();
                            }
                        }
                        else {
                            month.Focus();
                        }
                    }
                    else {
                        day.Focus();
                    }
                }
            }
        }


        /// <summary>
        /// Numeric restriction for DoB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbDob_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 22) {
                int tmp;
                e.Handled = !int.TryParse(Clipboard.GetText(), out tmp);
            }
            else {
                TextBox tb = sender as TextBox;
                e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
            }
        }
    }
}
