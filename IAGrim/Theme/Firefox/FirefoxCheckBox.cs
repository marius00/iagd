using IAGrim.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

[DefaultEvent("CheckedChanged")]
class FirefoxCheckBox : CheckBox {

    #region " Public "
    //public event CheckedChangedEventHandler CheckedChanged;
    public delegate void CheckedChangedEventHandler(object sender, EventArgs e);
    #endregion

    #region " Private "
    private Helpers.MouseState State;
    private Color ETC = Color.Blue;

    private Graphics G;


    #endregion
    private bool _Bold;

    #region " Properties "

/*
    public bool Checked {
        get { return base.Checked; }
        set {
            base.Checked = value;
            Invalidate();
        }
    }*/

    public new bool Enabled {
        get { return EnabledCalc; }
        set {
            base.Enabled = value;
            Invalidate();
        }
    }

    [DisplayName("Enabled")]
    public bool EnabledCalc {
        get { return base.Enabled; }
        set {
            Enabled = value;
            Invalidate();
        }
    }

    public bool Bold {
        get { return _Bold; }
        set {
            _Bold = value;
            Invalidate();
        }
    }

    #endregion

    #region " Control "

    public FirefoxCheckBox() {
        DoubleBuffered = true;
        ForeColor = Color.FromArgb(66, 78, 90);
        Font = Theme.GlobalFont(FontStyle.Regular, 10);
        Size = new Size(160, 27);
        Enabled = true;
        this.EnabledChanged += FirefoxCheckBox_EnabledChanged;
        this.CheckedChanged += FirefoxCheckBox_CheckedChanged;
    }

    private void FirefoxCheckBox_CheckedChanged(object sender, EventArgs e) {
        Invalidate();
    }

    private void FirefoxCheckBox_EnabledChanged(object sender, EventArgs e) {
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

            switch (State) {

                case Helpers.MouseState.Over:
                case Helpers.MouseState.Down:
                    Helpers.DrawRoundRect(G, new Rectangle(3, 3, 20, 20), 3, Color.FromArgb(44, 156, 218));

                    break;
                default:
                    Helpers.DrawRoundRect(G, new Rectangle(3, 3, 20, 20), 3, Helpers.GreyColor(200));

                    break;
            }


            if (Checked) {
                using (Image I = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(Theme.GetCheckMark())))) {
                    G.DrawImage(I, new Point(4, 5));
                }

            }


        }
        else {
            ETC = Helpers.GreyColor(170);
            Helpers.DrawRoundRect(G, new Rectangle(3, 3, 20, 20), 3, Helpers.GreyColor(220));


            if (Checked) {
                using (Image I = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(Theme.GetCheckMark())))) {
                    G.DrawImage(I, new Point(4, 5));
                }

            }

        }


        using (SolidBrush B = new SolidBrush(ETC)) {

            if (Bold) {
                G.DrawString(Text, Theme.GlobalFont(FontStyle.Bold, 10), B, new Point(32, 4));
            }
            else {
                G.DrawString(Text, Theme.GlobalFont(FontStyle.Regular, 10), B, new Point(32, 4));
            }

        }


    }

    protected override void OnMouseDown(MouseEventArgs e) {
        base.OnMouseDown(e);
        State = Helpers.MouseState.Down;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e) {
        base.OnMouseUp(e);

        if (Enabled) {
            //Checked = !Checked;
/*
            if (CheckedChanged != null) {
                CheckedChanged(this, e);
            }*/
        }

        State = Helpers.MouseState.Over;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e) {
        base.OnMouseEnter(e);
        State = Helpers.MouseState.Over;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e) {
        base.OnMouseLeave(e);
        State = Helpers.MouseState.None;
        Invalidate();
    }

    #endregion

}
