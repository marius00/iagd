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

    private PointF _TextLocation = new PointF(8, 5);
    public String TextLocation {
        get {
            return string.Format("{0}; {1}", _TextLocation.X, _TextLocation.Y);
        }
        set {
            var values = value.Split(';');
            float x, y;
            if (values.Length == 2 && float.TryParse(values[0], out x) && float.TryParse(values[1], out y)) {
                _TextLocation.X = x;
                _TextLocation.Y = y;
            }
            else {
                _TextLocation.X = 0;
                _TextLocation.Y = 0;
            }

            Invalidate();
        }
    }

    private int _HeaderHeight = 29;
    public int HeaderHeight {
        get {
            return _HeaderHeight;
        }
        set {
            _HeaderHeight = value;
            Invalidate();
        }
    }

    public void PaintHook() {
        //this.Font = new Font("Tahoma", 10f);
        //this.ForeColor = Color.FromArgb(40, 40, 40);
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
                    G.DrawString(this.Text, this.Font, brush, _TextLocation);
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


    protected sealed override void OnPaint(PaintEventArgs e) {
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

    protected void DrawCorners(Color c, Rectangle rect) {
        if (this._NoRounding) {
            return;
        }
        this.B.SetPixel(rect.X, rect.Y, c);
        checked {
            this.B.SetPixel(rect.X + (rect.Width - 1), rect.Y, c);
            this.B.SetPixel(rect.X, rect.Y + (rect.Height - 1), c);
            this.B.SetPixel(rect.X + (rect.Width - 1), rect.Y + (rect.Height - 1), c);
        }
    }

    protected void DrawBorders(Pen p1, Pen p2, Rectangle rect) {
        checked {
            this.G.DrawRectangle(p1, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            this.G.DrawRectangle(p2, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
        }
    }

    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle) {
        this._Rectangle = new Rectangle(x, y, width, height);
        this._Gradient = new LinearGradientBrush(this._Rectangle, c1, c2, angle);
        this.G.FillRectangle(this._Gradient, this._Rectangle);
    }
}

