using System;
using System.Drawing;
using System.Windows.Forms;
using IAGrim.Services.ItemStats;

namespace IAGrim.UI.Popups {

    /// <summary>
    /// A small modal for attaching a numeric comparison (operator + threshold) to a stat checkbox.
    /// Prototype UI only: the result is handed back to the caller, which stores it on the checkbox.
    /// </summary>
    public partial class NumericFilterDialog : Form {

        /// <summary>Outcome of the dialog. <c>cleared</c> means the user removed the filter.</summary>
        public readonly struct Result {
            public readonly StatValueFilter.Op op;
            public readonly double threshold;
            public readonly bool cleared;

            public Result(StatValueFilter.Op op, double threshold, bool cleared) {
                this.op = op;
                this.threshold = threshold;
                this.cleared = cleared;
            }
        }

        private static readonly (string label, StatValueFilter.Op op)[] Operators = {
            (">=", StatValueFilter.Op.GreaterOrEqual),
            (">", StatValueFilter.Op.GreaterThan),
            ("<", StatValueFilter.Op.LessThan),
            ("<=", StatValueFilter.Op.LessOrEqual),
            ("=", StatValueFilter.Op.Equal),
        };

        private readonly ComboBox _operator = new ComboBox();
        private readonly NumericUpDown _value = new NumericUpDown();
        private Result? _result;

        private NumericFilterDialog(StatValueFilter.Op? op, double threshold, bool hasExisting) {
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            Text = "Value filter";
            ClientSize = new Size(240, 96);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.75f);

            var caption = new Label {
                Text = "Match items where this stat is:",
                Location = new Point(12, 10),
                AutoSize = true
            };

            _operator.DropDownStyle = ComboBoxStyle.DropDownList;
            _operator.Location = new Point(12, 34);
            _operator.Size = new Size(56, 26);
            foreach (var o in Operators) {
                _operator.Items.Add(o.label);
            }
            _operator.SelectedIndex = op == null ? 0 : Array.FindIndex(Operators, x => x.op == op.Value);

            _value.Location = new Point(76, 34);
            _value.Size = new Size(152, 26);
            _value.DecimalPlaces = 0;
            _value.Minimum = -100000;
            _value.Maximum = 100000;
            _value.Increment = 1;
            _value.Value = (decimal)Math.Max((double)_value.Minimum, Math.Min((double)_value.Maximum, threshold));

            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Size = new Size(64, 26), Location = new Point(164, 66) };
            ok.Click += (s, e) => {
                _result = new Result(Operators[_operator.SelectedIndex].op, (double)_value.Value, false);
                Close();
            };

            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Size = new Size(64, 26), Location = new Point(96, 66) };
            cancel.Click += (s, e) => Close();

            Controls.Add(caption);
            Controls.Add(_operator);
            Controls.Add(_value);
            Controls.Add(ok);
            Controls.Add(cancel);

            if (hasExisting) {
                var clear = new Button { Text = "Remove", Size = new Size(64, 26), Location = new Point(12, 66) };
                clear.Click += (s, e) => {
                    _result = new Result(default, 0, true);
                    Close();
                };
                Controls.Add(clear);
            }

            AcceptButton = ok;
            CancelButton = cancel;
        }

        /// <summary>
        /// Shows the dialog anchored near the invoking control. Returns the chosen filter, or null when
        /// the user cancelled (leaving any existing filter untouched).
        /// </summary>
        public static Result? Show(Control owner, StatValueFilter.Op? currentOp, double currentThreshold) {
            using (var dialog = new NumericFilterDialog(currentOp, currentThreshold, currentOp != null)) {
                var anchor = owner.PointToScreen(new Point(owner.Width, 0));
                dialog.Location = new Point(anchor.X - dialog.Width + 20, anchor.Y + 24);
                dialog.ShowDialog(owner);
                return dialog._result;
            }
        }
    }
}
