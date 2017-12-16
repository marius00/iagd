using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IAGrim.UI {

    interface ComboBoxItemToggle {
        bool Enabled { get; }
    }
    class ComboBoxItemDisabler {

        private int LastSelection = 0;

        public ComboBoxItemDisabler(ComboBox cb) {
            cb.Enter += cb_Enter;
            cb.SelectionChangeCommitted += cb_SelectionChangeCommitted;
            cb.DrawItem += comboBox_DrawItem;
            cb.BackColor = SystemColors.Control;
        }

        void cb_SelectionChangeCommitted(object sender, EventArgs e) {
            ComboBox cb = sender as ComboBox;
            ComboBoxItemToggle item = cb.SelectedItem as ComboBoxItemToggle;
            if (item != null) {
                if (!item.Enabled)
                    cb.SelectedIndex = LastSelection;
            }
        }

        void cb_Enter(object sender, EventArgs e) {
            LastSelection = (sender as ComboBox).SelectedIndex;
        }


        private void comboBox_DrawItem(object sender, DrawItemEventArgs e) {
            ComboBox comboBox = (ComboBox)sender;
            if (e.Index >= 0) {
                ComboBoxItemToggle item = comboBox.Items[e.Index] as ComboBoxItemToggle;
                if (!item.Enabled) {
                    e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
                    e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), comboBox.Font, Brushes.LightSlateGray, e.Bounds);
                }
                else {
                    e.DrawBackground();

                    // Set the brush according to whether the item is selected or not
                    Brush br = ((e.State & DrawItemState.Selected) > 0) ? SystemBrushes.HighlightText : SystemBrushes.ControlText;

                    e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), comboBox.Font, br, e.Bounds);
                    e.DrawFocusRectangle();
                }
            }
        }
    }
}
