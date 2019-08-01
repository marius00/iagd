using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

internal class PanelBox : ScrollableControl {
    //private bool _Checked;

    public PanelBox() {
        this.SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        this.B = new Bitmap(1, 1);
        this.G = Graphics.FromImage(this.B);
        this.AllowTransparent();
    }

    private PointF _textLocation = new PointF(8, 5);
    public String TextLocation {
        get => $"{_textLocation.X}; {_textLocation.Y}";
        set {
            var values = value.Split(';');
            float x, y;
            if (values.Length == 2 && float.TryParse(values[0], out x) && float.TryParse(values[1], out y)) {
                _textLocation.X = x;
                _textLocation.Y = y;
            }
            else {
                _textLocation.X = 0;
                _textLocation.Y = 0;
            }

            Invalidate();
        }
    }

    private int _headerHeight = 29;
    public int HeaderHeight {
        get => _headerHeight;
        set {
            _headerHeight = value;
            Invalidate();
        }
    }

    public void PaintHook() {
        G.SmoothingMode = SmoothingMode.AntiAlias;


        Brush colorHeader = new SolidBrush(Color.FromArgb(231, 231, 231));
        Brush colorContent = new SolidBrush(Color.FromArgb(240, 240, 240));
        Pen colorBorder = new Pen(Color.FromArgb(214, 214, 214));
        SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);
        try {

            int headerHeight = Math.Min(HeaderHeight, this.Height);
            Rectangle rect;

            checked {
                // Panel (content)
                rect = new Rectangle(1, 0, this.Width - 3, this.Height - 4);
                G.FillRectangle(colorContent, rect);

                // Header
                rect = new Rectangle(0, 1, this.Width, headerHeight);
                G.FillRectangle(colorHeader, rect);

                // Main frame
                G.DrawRectangle(colorBorder, 0, 0, this.Width - 2, this.Height - 3);

                // Header frame
                G.DrawRectangle(colorBorder, 0, 0, this.Width - 2, headerHeight + 1);

                if (!string.IsNullOrEmpty(Text)) {
                    G.SmoothingMode = SmoothingMode.HighQuality;
                    //G.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    G.DrawString(this.Text, this.Font, brush, _textLocation);
                }

            }
        } finally {
            colorHeader.Dispose();
            colorContent.Dispose();
            colorBorder.Dispose();
            brush.Dispose();
        }
    }


    protected Graphics G;

    protected Bitmap B;

    private bool _NoRounding;

    private Rectangle _Rectangle;

    private LinearGradientBrush _Gradient;

    public bool NoRounding {
        get {
            return this._NoRounding;
        }
        set {
            this._NoRounding = value;
            this.Invalidate();
        }
    }

    public void AllowTransparent() {
        this.SetStyle(ControlStyles.Opaque, false);
        this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    }


    protected override void OnPaint(PaintEventArgs e) {
        if (this.Width != 0) {
            if (this.Height == 0) {
                return;
            }
            this.PaintHook();
            e.Graphics.DrawImage(this.B, 0, 0);
        }
    }

    protected override void OnSizeChanged(EventArgs e) {
        if (this.Width != 0 && this.Height != 0) {
            this.B = new Bitmap(this.Width, this.Height);
            this.G = Graphics.FromImage(this.B);
            this.Invalidate();
        }
        base.OnSizeChanged(e);
    }
}

