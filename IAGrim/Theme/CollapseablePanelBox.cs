using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.Theme {
    class CollapseablePanelBox : PanelBox {
        private const int margin = 3;
        private const int size = 24;
        private Image _arrow;
        private readonly Image _arrowImageUp = global::IAGrim.Properties.Resources.arrow;
        private readonly Image _arrowImageDown = global::IAGrim.Properties.Resources.arrow;
        private PanelState _panelState = PanelState.Expanded;
        private int _originalHeight;

        public enum PanelState {
            Collapsed, Expanded
        }

        public CollapseablePanelBox() {
            _arrowImageDown.RotateFlip(RotateFlipType.Rotate180FlipNone);

            // Position the arrow correctly in the top right corner
            _arrow = _arrowImageDown;
        }


        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (this.Width != 0 && this.Height != 0) {
                e.Graphics.DrawImage(this._arrow, Width - margin - size, margin, size, size);
            }
        }

        public void SetState(PanelState state) {
            _panelState = state;
            _arrow = _panelState == PanelState.Collapsed ? _arrowImageUp : _arrowImageDown;

            if (_panelState == PanelState.Collapsed) {
                _originalHeight = Height;
                Height = 32;
            }
            else {
                Height = _originalHeight;
            }

            OnSizeChanged(EventArgs.Empty);
        }

        public void ToggleState() {
            SetState(_panelState == PanelState.Expanded ? PanelState.Collapsed : PanelState.Expanded);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (Enabled) {
                if (e.Y < size) {
                    ToggleState();
                }
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (e.Y < size) {
                Cursor.Current = Cursors.Hand;
            }
        }
    }
}