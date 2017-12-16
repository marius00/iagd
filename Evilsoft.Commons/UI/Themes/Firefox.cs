// Firefox Theme.
// Made by AeroRev9.
// 25/07/2015.
// Updated : 29/07/2015 [2].
// Credits : Mavaamarten, Xertz.
// Converted by LaPanthere

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
namespace EvilsoftCommons.UI.Themes.Firefox {
    static class Theme {

        public static Font GlobalFont(FontStyle B, int S) {
            return new Font("Segoe UI", S, B);
        }

        public static string GetCheckMark() {
            return "iVBORw0KGgoAAAANSUhEUgAAABMAAAAQCAYAAAD0xERiAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEySURBVDhPY/hPRUBdw/79+/efVHz77bf/X37+wRAn2bDff/7+91l+83/YmtsYBpJs2ITjz/8rTbrwP2Dlrf9XXn5FkSPJsD13P/y3nHsVbNjyy28w5Ik27NWXX//TNt8DG1S19zFWNRiGvfzy8//ccy9RxEB4wvFnYIMMZl7+//brLwx5EEYx7MP33/9dF18Ha1py8RVcHBR7mlMvgsVXX8X0Hgwz/P379z8yLtz5AKxJdcpFcBj9+v3nf/CqW2Cx5E13UdSiYwzDvv36/d9/BUSzzvRL/0t2PQSzQd57+vEHilp0jGEYCJ9+8hnuGhiee+4Vhjp0jNUwEN566/1/m/mQZJC/48H/zz9+YVWHjHEaBsKgwAZ59eH771jl0TFew0D48osvWMWxYYKGEY///gcAqiuA6kEmfEMAAAAASUVORK5CYII=";
        }

    }

    static class Helpers {


        public static void FillRoundRectBetter(Graphics G, Rectangle R, int Curve, Color C) {
            using (SolidBrush B = new SolidBrush(C)) {
                G.FillPie(B, R.X, R.Y, Curve, Curve, 180, 90);
                G.FillPie(B, R.X + R.Width - Curve, R.Y, Curve, Curve, 270, 90);
                G.FillPie(B, R.X, R.Y + R.Height - Curve, Curve, Curve, 90, 90);
                G.FillPie(B, R.X + R.Width - Curve, R.Y + R.Height - Curve, Curve, Curve, 0, 90);
                //G.FillRectangle(B, Convert.ToInt32(R.X + Curve / 2), R.Y, R.Width - Curve, Convert.ToInt32(Curve / 2));
                G.FillRectangle(B, R.X, Convert.ToInt32(R.Y), R.Width, R.Height);
                //G.FillRectangle(B, Convert.ToInt32(R.X + Curve / 2), Convert.ToInt32(R.Y + R.Height - Curve / 2), R.Width - Curve, Convert.ToInt32(Curve / 2));
            }

        }
        public enum MouseState : byte {
            None = 0,
            Over = 1,
            Down = 2
        }

        public static Rectangle FullRectangle(Size S, bool Subtract) {
            if (Subtract) {
                return new Rectangle(0, 0, S.Width - 1, S.Height - 1);
            }
            else {
                return new Rectangle(0, 0, S.Width, S.Height);
            }
        }

        public static Color GreyColor(int G) {
            return Color.FromArgb(G, G, G);
        }

        public static void CenterString(Graphics G, string T, Font F, Color C, Rectangle R) {
            SizeF TS = G.MeasureString(T, F);

            using (SolidBrush B = new SolidBrush(C)) {
                G.DrawString(T, F, B, new Point((int)(R.Width / 2 - (TS.Width / 2)), (int)(R.Height / 2 - (TS.Height / 2))));
            }
        }


        public static void FillRoundRect(Graphics G, Rectangle R, int Curve, Color C) {
            using (SolidBrush B = new SolidBrush(C)) {
                G.FillPie(B, R.X, R.Y, Curve, Curve, 180, 90);
                G.FillPie(B, R.X + R.Width - Curve, R.Y, Curve, Curve, 270, 90);
                G.FillPie(B, R.X, R.Y + R.Height - Curve, Curve, Curve, 90, 90);
                G.FillPie(B, R.X + R.Width - Curve, R.Y + R.Height - Curve, Curve, Curve, 0, 90);
                G.FillRectangle(B, Convert.ToInt32(R.X + Curve / 2), R.Y, R.Width - Curve, Convert.ToInt32(Curve / 2));
                G.FillRectangle(B, R.X, Convert.ToInt32(R.Y + Curve / 2), R.Width, R.Height - Curve);
                G.FillRectangle(B, Convert.ToInt32(R.X + Curve / 2), Convert.ToInt32(R.Y + R.Height - Curve / 2), R.Width - Curve, Convert.ToInt32(Curve / 2));
            }

        }


        public static void DrawRoundRect(Graphics G, Rectangle R, int Curve, Color C) {
            using (Pen P = new Pen(C)) {
                G.DrawArc(P, R.X, R.Y, Curve, Curve, 180, 90);
                G.DrawLine(P, Convert.ToInt32(R.X + Curve / 2), R.Y, Convert.ToInt32(R.X + R.Width - Curve / 2), R.Y);
                G.DrawArc(P, R.X + R.Width - Curve, R.Y, Curve, Curve, 270, 90);
                G.DrawLine(P, R.X, Convert.ToInt32(R.Y + Curve / 2), R.X, Convert.ToInt32(R.Y + R.Height - Curve / 2));
                G.DrawLine(P, Convert.ToInt32(R.X + R.Width), Convert.ToInt32(R.Y + Curve / 2), Convert.ToInt32(R.X + R.Width), Convert.ToInt32(R.Y + R.Height - Curve / 2));
                G.DrawLine(P, Convert.ToInt32(R.X + Curve / 2), Convert.ToInt32(R.Y + R.Height), Convert.ToInt32(R.X + R.Width - Curve / 2), Convert.ToInt32(R.Y + R.Height));
                G.DrawArc(P, R.X, R.Y + R.Height - Curve, Curve, Curve, 90, 90);
                G.DrawArc(P, R.X + R.Width - Curve, R.Y + R.Height - Curve, Curve, Curve, 0, 90);
            }

        }


        public static void CenterStringTab(Graphics G, string text, Font font, Brush brush, Rectangle rect, bool shadow = false, int yOffset = 0) {
            SizeF textSize = G.MeasureString(text, font);
            int textX = (int)(rect.X + (rect.Width / 2) - (textSize.Width / 2));
            int textY = (int)(rect.Y + (rect.Height / 2) - (textSize.Height / 2) + yOffset);

            if (shadow)
                G.DrawString(text, font, Brushes.Black, textX + 1, textY + 1);
            G.DrawString(text, font, brush, textX, textY + 1);

        }

    }


    [DefaultEvent("CheckedChanged")]
    class FirefoxRadioButton : Control {

        #region " Public "
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region " Private "
        private Helpers.MouseState State;
        private Color ETC = Color.Blue;

        private Graphics G;
        private bool _EnabledCalc;
        private bool _Checked;
        #endregion
        private bool _Bold;

        #region " Properties "

        public bool Checked {
            get { return _Checked; }
            set {
                _Checked = value;
                Invalidate();
            }
        }

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

        public bool Bold {
            get { return _Bold; }
            set {
                _Bold = value;
                Invalidate();
            }
        }

        #endregion

        #region " Control "

        public FirefoxRadioButton() {
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(66, 78, 90);
            Font = Theme.GlobalFont(FontStyle.Regular, 10);
            Size = new Size(160, 27);
            Enabled = true;
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

                        using (Pen P = new Pen(Color.FromArgb(34, 146, 208))) {
                            G.DrawEllipse(P, new Rectangle(2, 2, 22, 22));
                        }


                        break;
                    default:

                        using (Pen P = new Pen(Helpers.GreyColor(190))) {
                            G.DrawEllipse(P, new Rectangle(2, 2, 22, 22));
                        }


                        break;
                }


                if (Checked) {
                    using (SolidBrush B = new SolidBrush(Color.FromArgb(34, 146, 208))) {
                        G.FillEllipse(B, new Rectangle(7, 7, 12, 12));
                    }

                }

            }
            else {
                ETC = Helpers.GreyColor(170);

                using (Pen P = new Pen(Helpers.GreyColor(210))) {
                    G.DrawEllipse(P, new Rectangle(2, 2, 22, 22));
                }


                if (Checked) {
                    using (SolidBrush B = new SolidBrush(Color.FromArgb(34, 146, 208))) {
                        G.FillEllipse(B, new Rectangle(7, 7, 12, 12));
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

                if (!Checked) {
                    foreach (Control C in Parent.Controls) {
                        if (C is FirefoxRadioButton) {
                            ((FirefoxRadioButton)C).Checked = false;
                        }
                    }

                }

                Checked = true;
                if (CheckedChanged != null) {
                    CheckedChanged(this, e);
                }
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

    [DefaultEvent("CheckedChanged")]
    class FirefoxCheckBox : Control {

        #region " Public "
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region " Private "
        private Helpers.MouseState State;
        private Color ETC = Color.Blue;

        private Graphics G;
        private bool _EnabledCalc;
        private bool _Checked;
        #endregion
        private bool _Bold;

        #region " Properties "

        public bool Checked {
            get { return _Checked; }
            set {
                _Checked = value;
                Invalidate();
            }
        }

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
                Checked = !Checked;
                if (CheckedChanged != null) {
                    CheckedChanged(this, e);
                }
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

    class FirefoxH1 : Label {

        #region " Private "
        #endregion
        private Graphics G;

        #region " Control "

        public FirefoxH1() {
            DoubleBuffered = true;
            AutoSize = false;
            Font = new Font("Segoe UI Semibold", 20);
            ForeColor = Color.FromArgb(76, 88, 100);
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            using (Pen P = new Pen(Helpers.GreyColor(200))) {
                G.DrawLine(P, new Point(0, 50), new Point(Width, 50));
            }

        }

        #endregion

    }

    class FirefoxH2 : Label {

        #region " Control "

        public FirefoxH2() {
            Font = Theme.GlobalFont(FontStyle.Bold, 10);
            ForeColor = Color.FromArgb(76, 88, 100);
            BackColor = Color.White;
        }

        #endregion

    }

    class FirefoxButton : Control {

        #region " Private "
        private Helpers.MouseState State;
        private Color ETC = Color.Blue;

        private Graphics G;
        #endregion
        private bool _EnabledCalc;

        #region " Properties "

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
            Font = Theme.GlobalFont(FontStyle.Regular, 10);
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);

            if (Enabled) {
                ETC = Color.FromArgb(56, 68, 80);

                switch (State) {

                    case Helpers.MouseState.None:

                        using (SolidBrush B = new SolidBrush(Helpers.GreyColor(245))) {
                            G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                        }


                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(193));

                        break;
                    case Helpers.MouseState.Over:

                        using (SolidBrush B = new SolidBrush(Helpers.GreyColor(232))) {
                            G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                        }


                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(193));

                        break;
                    default:

                        using (SolidBrush B = new SolidBrush(Helpers.GreyColor(212))) {
                            G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                        }


                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(193));

                        break;
                }

            }
            else {
                ETC = Helpers.GreyColor(170);

                using (SolidBrush B = new SolidBrush(Helpers.GreyColor(245))) {
                    G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                }

                Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(223));

            }

            Helpers.CenterString(G, Text, Theme.GlobalFont(FontStyle.Regular, 10), ETC, Helpers.FullRectangle(Size, false));

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

    class FirefoxRedirect : Control {

        #region " Private "
        private Helpers.MouseState State;

        private Graphics G;
        private Color FC = Color.Blue;
        #endregion
        private Font FF = null;

        #region " Control "

        public FirefoxRedirect() {
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            BackColor = Color.White;
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            switch (State) {

                case Helpers.MouseState.Over:
                    FC = Color.FromArgb(23, 140, 229);
                    FF = Theme.GlobalFont(FontStyle.Underline, 10);

                    break;
                case Helpers.MouseState.Down:
                    FC = Color.FromArgb(255, 149, 0);
                    FF = Theme.GlobalFont(FontStyle.Regular, 10);

                    break;
                default:
                    FC = Color.FromArgb(0, 149, 221);
                    FF = Theme.GlobalFont(FontStyle.Regular, 10);

                    break;
            }

            using (SolidBrush B = new SolidBrush(FC)) {
                G.DrawString(Text, FF, B, new Point(0, 0));
            }

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

    class FirefoxSubTabControl : TabControl {

        #region " Private "
        private Graphics G;
        #endregion
        private Rectangle TabRect;

        #region " Control "

        public FirefoxSubTabControl() {
            DoubleBuffered = true;
            Alignment = TabAlignment.Top;
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
            SetStyle(ControlStyles.UserPaint, true);
            ItemSize = new Size(100, 40);
            SizeMode = TabSizeMode.Fixed;
        }

        protected override void OnControlAdded(ControlEventArgs e) {
            base.OnControlAdded(e);
            try {
                for (int i = 0; i <= TabPages.Count - 1; i++) {
                    TabPages[i].BackColor = Color.White;
                    TabPages[i].ForeColor = Color.FromArgb(66, 79, 90);
                    TabPages[i].Font = Theme.GlobalFont(FontStyle.Regular, 10);
                }
            }
            catch {
            }
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);


            for (int i = 0; i <= TabPages.Count - 1; i++) {
                TabRect = GetTabRect(i);


                if (GetTabRect(i).Contains(this.PointToClient(Cursor.Position)) & !(SelectedIndex == i)) {
                    using (SolidBrush B = new SolidBrush(Helpers.GreyColor(240))) {
                        G.FillRectangle(B, new Rectangle(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2, GetTabRect(i).Width, GetTabRect(i).Height + 1));
                    }


                }
                else if (SelectedIndex == i) {
                    using (SolidBrush B = new SolidBrush(Helpers.GreyColor(240))) {
                        G.FillRectangle(B, new Rectangle(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2, GetTabRect(i).Width, GetTabRect(i).Height + 1));
                    }

                    using (Pen P = new Pen(Color.FromArgb(255, 149, 0), 4)) {
                        G.DrawLine(P, new Point(TabRect.X - 2, TabRect.Y + ItemSize.Height - 2), new Point(TabRect.X + TabRect.Width - 2, TabRect.Y + ItemSize.Height - 2));
                    }

                }
                else if (!(SelectedIndex == i)) {
                    G.FillRectangle(Brushes.White, GetTabRect(i));
                }

                using (SolidBrush B = new SolidBrush(Color.FromArgb(56, 69, 80))) {
                    Helpers.CenterStringTab(G, TabPages[i].Text, Theme.GlobalFont(FontStyle.Regular, 10), B, GetTabRect(i));
                }

            }

            using (Pen P = new Pen(Helpers.GreyColor(200))) {
                G.DrawLine(P, new Point(0, ItemSize.Height + 2), new Point(Width, ItemSize.Height + 2));
            }

        }

        #endregion

    }

    class FirefoxMainTabControl : TabControl {

        #region " Private "
        private Graphics G;
        private Rectangle TabRect;
        #endregion
        private Color FC = Color.Blue;

        #region " Control "

        public FirefoxMainTabControl() {
            DoubleBuffered = true;
            ItemSize = new Size(43, 152);
            Alignment = TabAlignment.Left;
            SizeMode = TabSizeMode.Fixed;
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnControlAdded(ControlEventArgs e) {
            base.OnControlAdded(e);
            try {
                for (int i = 0; i <= TabPages.Count - 1; i++) {
                    TabPages[i].BackColor = Color.White;
                    TabPages[i].ForeColor = Color.FromArgb(66, 79, 90);
                    TabPages[i].Font = Theme.GlobalFont(FontStyle.Regular, 10);
                }
            }
            catch {
            }
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Color.FromArgb(66, 79, 90));


            for (int i = 0; i <= TabPages.Count - 1; i++) {
                TabRect = GetTabRect(i);


                if (SelectedIndex == i) {
                    using (SolidBrush B = new SolidBrush(Color.FromArgb(52, 63, 72))) {
                        G.FillRectangle(B, TabRect);
                    }

                    FC = Helpers.GreyColor(245);

                    using (SolidBrush B = new SolidBrush(Color.FromArgb(255, 175, 54))) {
                        G.FillRectangle(B, new Rectangle(TabRect.Location.X - 3, TabRect.Location.Y + 1, 5, TabRect.Height - 2));
                    }

                }
                else {
                    FC = Helpers.GreyColor(192);

                    using (SolidBrush B = new SolidBrush(Color.FromArgb(66, 79, 90))) {
                        G.FillRectangle(B, TabRect);
                    }

                }

                using (SolidBrush B = new SolidBrush(FC)) {
                    G.DrawString(TabPages[i].Text, Theme.GlobalFont(FontStyle.Regular, 10), B, new Point(TabRect.X + 50, TabRect.Y + 12));
                }

                if ((ImageList != null)) {
                    if (!(TabPages[i].ImageIndex < 0)) {
                        G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Rectangle(TabRect.X + 19, TabRect.Y + ((TabRect.Height / 2) - 10), 18, 18));
                    }
                }

            }

        }

        #endregion

    }

}