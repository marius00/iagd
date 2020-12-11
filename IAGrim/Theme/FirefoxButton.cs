using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


sealed class FirefoxButton : Control {
    #region " Private "

    private Helpers.MouseState State;

    private Graphics G;

    #endregion

    private bool _EnabledCalc;

    #region " Properties "

    public Color HoverColor { get; set; } = Helpers.GreyColor(232);
    public Color BorderColor { get; set; } = Helpers.GreyColor(193);
    public Color HoverForeColor { get; set; } = Helpers.GreyColor(193);
    public Color BackColorDefault { get; set; } = Helpers.GreyColor(212);
    public new Color BackColorOverride { get; set; } = Helpers.GreyColor(245);


    public new bool Enabled {
        get { return EnabledCalc; }
        set {
            _EnabledCalc = value;
            Invalidate();
        }
    }

    [DisplayName("Enabled")]
    public bool EnabledCalc {
        get { return _EnabledCalc; }
        set {
            Enabled = value;
            Invalidate();
        }
    }

    #endregion

    #region " Control "

    public FirefoxButton() {
        DoubleBuffered = true;
        Enabled = true;
        ForeColor = Color.FromArgb(56, 68, 80);
        Font = global::Theme.GlobalFont(FontStyle.Regular, 10);
    }


    protected override void OnPaint(PaintEventArgs e) {
        G = e.Graphics;
        G.SmoothingMode = SmoothingMode.HighQuality;
        G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        base.OnPaint(e);

        G.Clear(Parent.BackColor);

        var fore = ForeColor;
        if (Enabled) {
            switch (State) {
                case Helpers.MouseState.None:

                    using (SolidBrush B = new SolidBrush(BackColorOverride)) {
                        // TODO: This is not 245 -_-
                        G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                    }


                    Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, BorderColor);

                    break;
                case Helpers.MouseState.Over:

                    using (SolidBrush B = new SolidBrush(HoverColor)) {
                        G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                    }

                    fore = HoverForeColor;
                    Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, BorderColor);

                    break;
                default:

                    using (SolidBrush B = new SolidBrush(BackColorDefault)) {
                        G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                    }


                    Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, BorderColor);

                    break;
            }
        }
        else {
            fore = Helpers.GreyColor(170);

            using (SolidBrush B = new SolidBrush(Helpers.GreyColor(245))) {
                G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
            }

            Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(223));
        }

        Helpers.CenterString(G, Text, global::Theme.GlobalFont(FontStyle.Regular, 10), fore, Helpers.FullRectangle(Size, false));
    }

    protected override void OnMouseUp(MouseEventArgs e) {
        base.OnMouseUp(e);
        State = Helpers.MouseState.Over;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e) {
        base.OnMouseUp(e);
        State = Helpers.MouseState.Down;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e) {
        base.OnMouseEnter(e);
        State = Helpers.MouseState.Over;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e) {
        base.OnMouseEnter(e);
        State = Helpers.MouseState.None;
        Invalidate();
    }

    #endregion
}