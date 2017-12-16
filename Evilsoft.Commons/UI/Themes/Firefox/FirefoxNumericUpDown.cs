
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace EvilsoftCommons.UI.Themes.Firefox {

    class FirefoxNumericUpDown : Control {

        #region " Private "
        private Graphics G;
        private int _Value;
        private int _Min;
        private int _Max;
        private Point Loc;
        private bool Down;
        #endregion
        private Color ETC = Color.Blue;

        #region " Properties "



        public int Value {
            get { return _Value; }

            set {
                if (value <= _Max & value >= Minimum) {
                    _Value = value;
                    Text = value.ToString(); // Trigger textchanged
                }

                Invalidate();

            }
        }

        public int Minimum {
            get { return _Min; }

            set {
                if (value < Maximum) {
                    _Min = value;
                }

                if (value < Minimum) {
                    value = Minimum;
                }

                Invalidate();
            }
        }

        public int Maximum {
            get { return _Max; }

            set {
                if (value > Minimum) {
                    _Max = value;
                }

                if (value > Maximum) {
                    value = Maximum;
                }

                Invalidate();
            }
        }

        #endregion

        #region " Control "

        public FirefoxNumericUpDown() {
            this.EnabledChanged += FirefoxNumericUpDown_EnabledChanged;
            this.MouseWheel += FirefoxNumericUpDown_MouseWheel;
            DoubleBuffered = true;
            Value = 0;
            Minimum = 0;
            Maximum = 100;
            Cursor = Cursors.IBeam;
            BackColor = Color.White;
            ForeColor = Color.FromArgb(66, 78, 90);
            Font = Theme.GlobalFont(FontStyle.Regular, 10);
            Enabled = true;
        }

        private void FirefoxNumericUpDown_MouseWheel(object sender, MouseEventArgs e) {
            if (e.Delta > 0)
                Increase();
            else
                Decrease();
        }

        private void FirefoxNumericUpDown_EnabledChanged(object sender, EventArgs e) {
            Invalidate();
        }

        private bool _hoverUp;
        private bool _hoverDown;
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            Loc.X = e.X;
            Loc.Y = e.Y;
            Invalidate();

            if (Loc.X < Width - 23) {
                Cursor = Cursors.IBeam;
            } else {
                Cursor = Cursors.Default;
            }


            if (Enabled) {
                bool isHover = Loc.X > Width - 21 && Loc.X < Width - 3;
                bool isUp = isHover && Loc.Y < 14 && Loc.Y >= 3;
                bool isDown = isHover && Loc.Y > 14 && Loc.Y < 25;

                _hoverUp = isUp;
                _hoverDown = isDown;

            }

        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Height = 30;
        }

        private void Increase() {

            if (Value == 0)
                Value = 10;
            else if ((Value + 5) <= Maximum) {
                Value += 5;
            }
        }

        private void Decrease() {

            if (Value == 10)
                Value = 0;
            else if ((Value - 5) >= Minimum) {
                Value -= 5;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseClick(e);


            if (Enabled) {
                if (Loc.X > Width - 21 && Loc.X < Width - 3) {
                    if (Loc.Y < 15) {
                        Increase();
                    } else {
                        Decrease();
                    }
                } else {
                    Down = !Down;
                    Focus();
                }

            }

            Invalidate();
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e) {
            base.OnKeyPress(e);
            try {
                if (Down) {
                    Value = Convert.ToInt32(Value.ToString() + e.KeyChar.ToString());
                }

                if (Value > Maximum) {
                    Value = Maximum;
                }

            } catch {
            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) {
            base.OnKeyUp(e);


            if (e.KeyCode == Keys.Up) {
                Increase();

                Invalidate();


            } else if (e.KeyCode == Keys.Down) {
                Decrease();

            } else if (e.KeyCode == Keys.Back) {
                string BC = Value.ToString();
                BC = BC.Remove(Convert.ToInt32(BC.Length - 1));

                if ((BC.Length == 0)) {
                    BC = "0";
                }

                Value = Convert.ToInt32(BC);

            }

            Invalidate();

        }

        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);


            if (Enabled) {
                ETC = Color.FromArgb(66, 78, 90);


                using (Pen P = new Pen(Helpers.GreyColor(190))) {
                    // Area for the number
                    Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(190));

                    // Divisor for up/down
                    G.DrawLine(P, new Point(Width - 24, (int)14.5f), new Point(Width - 5, (int)14.5f));

                }

                // Rect around up/down
                Helpers.DrawRoundRect(G, new Rectangle(Width - 24, 4, 19, 21), 3, Helpers.GreyColor(200));

                if (_hoverUp) {
                    if (Value < Maximum)
                        Helpers.FillRoundRectBetter(G, new Rectangle(Width - 24, 4, 19, 10), 3, Color.FromArgb(0, 162, 232));
                    else
                        Helpers.FillRoundRectBetter(G, new Rectangle(Width - 24, 4, 19, 10), 3, Helpers.GreyColor(200));
                } else if (_hoverDown) {
                    if (Value > Minimum)
                        Helpers.FillRoundRectBetter(G, new Rectangle(Width - 24, 14, 19, 11), 3, Color.FromArgb(0, 162, 232));
                    else
                        Helpers.FillRoundRectBetter(G, new Rectangle(Width - 24, 14, 19, 11), 3, Helpers.GreyColor(200));
                }


            } else {
                ETC = Helpers.GreyColor(170);

                using (Pen P = new Pen(Helpers.GreyColor(230))) {
                    Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(190));
                    G.DrawLine(P, new Point(Width - 24, (int)13.5f), new Point(Width - 5, (int)13.5f));
                }

                Helpers.DrawRoundRect(G, new Rectangle(Width - 24, 4, 19, 21), 3, Helpers.GreyColor(220));

            }

            using (SolidBrush B = new SolidBrush(ETC)) {
                G.DrawString("t", new Font("Marlett", 8, FontStyle.Bold), B, new Point(Width - 22, 5));
                G.DrawString("u", new Font("Marlett", 8, FontStyle.Bold), B, new Point(Width - 22, 13));
                Helpers.CenterString(G, Value.ToString(), new Font("Segoe UI", 10), ETC, new Rectangle(Width / 2 - 10, 0, Width - 15, Height));
            }

        }

        #endregion

    }
}
