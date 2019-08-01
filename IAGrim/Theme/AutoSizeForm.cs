using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.Theme {
    class AutoSizeForm : Form {
        private readonly int _margin = 3;
        readonly List<Control> _controls = new List<Control>();

        protected AutoSizeForm() {
            this.Load += new System.EventHandler(this.AutoSizeForm_Load);
        }


        class CustomComparer : IComparer<Control> {
            public int Compare(Control x, Control y) {
                return x.Top.CompareTo(y.Top);
            }
        }


        private void AutoSizeForm_Load(object sender, EventArgs e) {
            // When a child resizes, we need to recalculate.

            for (int i = 0; i < Controls.Count; i++) {
                _controls.Add(Controls[i]);
                Controls[i].SizeChanged += (_, __) => ResizeChildren();
            }

            _controls.Sort(new CustomComparer());
        }

        private void ResizeChildren() {
            // All positions are relative to the current scroll position.
            // Alternatively, subtract by this value?
            this.VerticalScroll.Value = 0;

            // Set the correct width for each child
            for (int i = 0; i < Controls.Count; i++) {
                Controls[i].Width = Width - _margin * 3 - SystemInformation.VerticalScrollBarWidth - System.Windows.Forms.SystemInformation.VerticalResizeBorderThickness;
            }

            // Set the correct Y position for each child
            int y = _margin;
            foreach (var c in _controls) {
                c.Top = y;
                y += c.Height + _margin;
            }

            Top = _margin;
        }

        protected override void OnSizeChanged(EventArgs e) {
            ResizeChildren();

            base.OnSizeChanged(e);
        }
    }
}
