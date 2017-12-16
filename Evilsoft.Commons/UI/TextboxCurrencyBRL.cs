using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EvilsoftCommons.UI {
    /// <summary>
    /// Helper class for converting Windows.Forms.TextBox to handle Brazilian currency input.
    /// </summary>
    public class TextboxCurrencyBRL {

        /// <summary>
        /// Restrict input data
        /// </summary>
        /// <param name="tb"></param>
        public TextboxCurrencyBRL(TextBox tb) {
            tb.KeyPress += textbox_KeyPress;
            tb.TextChanged += textBox1_TextChanged;
        }

        /// <summary>
        /// Limit input data (if editable) and add currency formatting
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="dataBinding"></param>
        public TextboxCurrencyBRL(TextBox tb, Binding dataBinding) {
            if (!tb.ReadOnly) {
                tb.KeyPress += textbox_KeyPress;
                tb.TextChanged += textBox1_TextChanged;
            }
            dataBinding.Parse += new ConvertEventHandler(CurrencyStringToDecimal);
            dataBinding.Format += new ConvertEventHandler(DecimalToCurrencyString);
        }

        /// <summary>
        /// Restrict input data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textbox_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 22) {
                double tmp;
                e.Handled = !double.TryParse(Clipboard.GetText().Replace(".", ","), out tmp);
            }
            else {
                TextBox tb = sender as TextBox;
                bool isValidDot = e.KeyChar == ',' && !tb.Text.Contains(',');
                bool isValidPosition = tb.SelectionStart >= 3; // "R$ ".length
                e.Handled = !((char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || isValidDot) && isValidPosition);
            }
        }



        private static void DecimalToCurrencyString(object sender, ConvertEventArgs cevent) {
            if (cevent.DesiredType != typeof(string)) return;
            cevent.Value = ((double)cevent.Value).ToString("c");
        }

        private static void CurrencyStringToDecimal(object sender, ConvertEventArgs cevent) {
            if (cevent.DesiredType != typeof(double)) return;

            cevent.Value = double.Parse(cevent.Value.ToString(), NumberStyles.Currency, null);
        }

        /// <summary>
        /// Handle the decimal shifting right->left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e) {
            TextBox tb = sender as TextBox;

            //Remove previous formatting, or the decimal check will fail including leading zeros
            //decimal value = decimal.Parse(textBox1.Text.ToString(), NumberStyles.Currency, null);
            decimal ul;
            //Check we are indeed handling a number
            string tmp = tb.Text.ToString().Replace(".", "").Replace(",", "");
            if (decimal.TryParse(tmp, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("pt-BR"), out ul)) {
                ul /= 100;

                tb.TextChanged -= textBox1_TextChanged;
                //Format the text as currency
                tb.Text = string.Format(CultureInfo.CreateSpecificCulture("pt-BR"), "{0:C2}", ul);
                tb.TextChanged += textBox1_TextChanged;
                tb.Select(tb.Text.Length, 0);
            }
        }

    }
}
