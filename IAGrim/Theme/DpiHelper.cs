namespace IAGrim.Theme {
    /// <summary>
    /// Helper class to scale absolute pixel values to units appropriate for the current display DPI/scale factor.
    /// </summary>
    internal class DpiHelper {
        /// <summary>
        /// This is Windows' default DPI. UI elements are designed and laid out based on this DPS.
        /// </summary>
        private const float BASE_DPI = 96.0f;

        private readonly float _scaleFactorX;

        private readonly float _scaleFactorY;

        public DpiHelper(Graphics g) {
            _scaleFactorX = g.DpiX / BASE_DPI;
            _scaleFactorY = g.DpiY / BASE_DPI;
        }

        public int ScaleX(int x) {
            return (int)(x * _scaleFactorX);
        }

        public float ScaleX(float x) {
            return x * _scaleFactorX;
        }

        public int ScaleY(int y) {
            return (int)(y * _scaleFactorY);
        }
        public float ScaleY(float y) {
            return y * _scaleFactorY;
        }

        public Point ScalePoint(Point p) {
            return new Point(ScaleX(p.X), ScaleY(p.Y));
        }

        public PointF ScalePoint(PointF p) {
            return new PointF(ScaleX(p.X), ScaleY(p.Y));
        }

        public Size ScaleSize(Size s) {
            return new Size(ScaleX(s.Width), ScaleY(s.Height));
        }

        public Rectangle ScaleRectangle(Rectangle r) {
            return new Rectangle(ScalePoint(r.Location), ScaleSize(r.Size));
        }
    }
}
