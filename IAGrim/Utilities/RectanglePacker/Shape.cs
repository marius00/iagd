using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities.RectanglePacker {
    class Shape : IComparable<Shape> {
        private int _w;
        private int _h;
        public int Width {
            get {
                return Math.Min(6, Math.Max(0, _w));
            }
            set {
                _w = value;
            }
        }
        public int Height {
            get {
                return Math.Min(6, Math.Max(0, _h));
            }
            set {
                _h = value;
            }
        }

        public int Size {
            get {
                return Width * Height;
            }
        }

        public int CompareTo(Shape other) {
            return Size - other.Size;
        }
    }
}
