using IAGrim.Theme;
using IAGrim.Services.ItemStats;
using IAGrim.UI.Popups;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
    // public Color TextColor;
    public bool IsDarkMode { get; set; }
    private byte[] checkMarkBytes => Convert.FromBase64String(IsDarkMode ? Theme.GetCheckMark() :  Theme.GetLightCheckMark());

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

    /// <summary>Raised when the numeric filter is set or removed via the filter dialog (not on cancel).</summary>
    public event EventHandler FilterChanged;

    /// <summary>
    /// When true, a small filter (funnel) button is drawn on the right edge of the checkbox while it is
    /// checked and hovered, letting the user attach a numeric "stat &gt;= n" comparison to this stat.
    /// Opt-in per panel; defaults to false so non-stat checkboxes (e.g. Classes) are unaffected.
    /// </summary>
    public bool SupportsNumericFilter { get; set; } = false;

    /// <summary>The comparison operator the user picked, or null when no numeric filter is set.</summary>
    public StatValueFilter.Op? FilterOperator { get; private set; }

    /// <summary>The threshold the user typed, valid only when <see cref="FilterOperator"/> is set.</summary>
    public double FilterThreshold { get; private set; }

    /// <summary>True once a numeric filter has been configured for this checkbox.</summary>
    public bool HasFilter => FilterOperator != null;

    public void ClearFilter() {
        FilterOperator = null;
        FilterThreshold = 0;
        Invalidate();
    }

    /// <summary>Turns on the numeric-filter button for every FirefoxCheckBox under <paramref name="root"/>.</summary>
    public static void EnableNumericFilters(Control root) {
        foreach (Control c in root.Controls) {
            if (c is FirefoxCheckBox cb) {
                cb.SupportsNumericFilter = true;
            }

            if (c.HasChildren) {
                EnableNumericFilters(c);
            }
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
        // Unchecking the stat removes any numeric filter: "stat >= n" makes no sense without the stat.
        if (!Checked && HasFilter) {
            ClearFilter();
        }

        Invalidate();
    }

    // Right-edge hit-rect of the filter glyph, recomputed on every paint (empty when not drawn).
    private Rectangle _filterIconRect = Rectangle.Empty;
    private bool _overIcon;

    /// <summary>The filter glyph is shown whenever the stat is selected; filtering an unselected stat is meaningless.</summary>
    private bool ShouldShowFilterIcon =>
        SupportsNumericFilter && Enabled && Checked;

    private void FirefoxCheckBox_EnabledChanged(object sender, EventArgs e) {
        Invalidate();
    }


    protected override void OnPaint(PaintEventArgs e) {
        G = e.Graphics;
        G.SmoothingMode = SmoothingMode.HighQuality;
        G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        base.OnPaint(e);

        G.Clear(Parent.BackColor);

        DpiHelper dpiHelper = new DpiHelper(G);
        Rectangle checkboxRect = dpiHelper.ScaleRectangle(new Rectangle(3, 3, 20, 20));
        Point checkmarkPosition = dpiHelper.ScalePoint(new Point(4, 5));
        Point textPosition = dpiHelper.ScalePoint(new Point(32, 4));

        if (Enabled) {
            ETC = ForeColor;

            switch (State) {

                case Helpers.MouseState.Over:
                case Helpers.MouseState.Down:
                    Helpers.DrawRoundRect(G, checkboxRect, 3, Color.FromArgb(44, 156, 218));

                    break;
                default:
                    Helpers.DrawRoundRect(G, checkboxRect, 3, Helpers.GreyColor(200));

                    break;
            }


            if (Checked) {
                using (Image I = Image.FromStream(new System.IO.MemoryStream(checkMarkBytes))) {
                    G.DrawImage(I, checkmarkPosition);
                }

            }


        }
        else {
            ETC = Helpers.GreyColor(170);
            Helpers.DrawRoundRect(G, checkboxRect, 3, Helpers.GreyColor(220));


            if (Checked) {
                using (Image I = Image.FromStream(new System.IO.MemoryStream(checkMarkBytes))) {
                    G.DrawImage(I, checkmarkPosition);
                }

            }

        }


        using (SolidBrush B = new SolidBrush(ETC)) {

            if (Bold) {
                G.DrawString(Text, Theme.GlobalFont(FontStyle.Bold, 10), B, textPosition);
            }
            else {
                G.DrawString(Text, Theme.GlobalFont(FontStyle.Regular, 10), B, textPosition);
            }

        }

        DrawFilterIcon(G, dpiHelper);
    }

    /// <summary>
    /// Draws the small funnel-shaped filter button on the right edge and records its hit-rect. An
    /// active filter is tinted accent-blue; a hover-only button is grey.
    /// </summary>
    private void DrawFilterIcon(Graphics g, DpiHelper dpiHelper) {
        if (!ShouldShowFilterIcon) {
            _filterIconRect = Rectangle.Empty;
            return;
        }

        Size box = dpiHelper.ScaleSize(new Size(20, 20));
        int right = dpiHelper.ScalePoint(new Point(6, 0)).X;
        _filterIconRect = new Rectangle(Width - box.Width - right, (Height - box.Height) / 2, box.Width, box.Height);

        Color accent = Color.FromArgb(44, 156, 218);
        Color color = HasFilter ? accent : (_overIcon ? accent : Helpers.GreyColor(150));

        // Funnel: wide top, narrow spout, drawn inside a small inset of the hit-rect.
        Rectangle r = Rectangle.Inflate(_filterIconRect, -dpiHelper.ScalePoint(new Point(4, 0)).X, -dpiHelper.ScalePoint(new Point(0, 3)).Y);
        int midX = r.Left + r.Width / 2;
        int neck = r.Top + (int)(r.Height * 0.45f);
        PointF[] funnel = {
            new PointF(r.Left, r.Top),
            new PointF(r.Right, r.Top),
            new PointF(midX + r.Width * 0.12f, neck),
            new PointF(midX + r.Width * 0.12f, r.Bottom),
            new PointF(midX - r.Width * 0.12f, r.Bottom - r.Height * 0.15f),
            new PointF(midX - r.Width * 0.12f, neck),
        };

        if (_overIcon && !HasFilter) {
            using (SolidBrush hb = new SolidBrush(Helpers.GreyColor(235))) {
                Helpers.FillRoundRectBetter(g, _filterIconRect, 3, Helpers.GreyColor(235));
            }
        }

        using (SolidBrush fb = new SolidBrush(color)) {
            g.FillPolygon(fb, funnel);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e) {
        base.OnMouseMove(e);
        bool over = _filterIconRect != Rectangle.Empty && _filterIconRect.Contains(e.Location);
        if (over != _overIcon) {
            _overIcon = over;
            Invalidate();
        }
        Cursor = over ? Cursors.Hand : Cursors.Default;
    }

    protected override void OnClick(EventArgs e) {
        // A click on the filter glyph opens the numeric-filter dialog instead of toggling the checkbox.
        if (ShouldShowFilterIcon && _filterIconRect.Contains(PointToClient(MousePosition))) {
            OpenFilterDialog();
            return;
        }

        base.OnClick(e);
    }

    private void OpenFilterDialog() {
        var result = NumericFilterDialog.Show(this, FilterOperator, FilterThreshold);
        if (result == null) {
            return; // cancelled
        }

        if (result.Value.cleared) {
            ClearFilter();
        }
        else {
            FilterOperator = result.Value.op;
            FilterThreshold = result.Value.threshold;
            Invalidate();
        }

        // Re-run the search so the newly set/removed threshold takes effect immediately.
        FilterChanged?.Invoke(this, EventArgs.Empty);
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
        _overIcon = false;
        Invalidate();
    }

    #endregion

}
