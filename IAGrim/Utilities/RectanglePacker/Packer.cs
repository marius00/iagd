using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities.RectanglePacker {
    class Packer {
        private bool[,] Map;
        public uint Height { private set; get; }
        public uint Width { private set; get; }
        
        private PackerMode _mode;
        public PackerMode Mode {
            get {
                return _mode;
            }
            set {
                _mode = value;
                if (value == PackerMode.Horizontal_First)
                    SelectedInsertFunc = InsertHorizontalFirst;
                else
                    SelectedInsertFunc = InsertVerticalFirst;
            }
        }
        private Func<bool[,], Shape, Position> SelectedInsertFunc;

        public class Position {
            public float X;
            public float Y;

            public uint UX {
                get {
                    byte[] b = BitConverter.GetBytes(X);
                    return BitConverter.ToUInt32(b, 0);
                }
            }

            public uint UY {
                get {
                    byte[] b = BitConverter.GetBytes(Y);
                    return BitConverter.ToUInt32(b, 0);
                }
            }
        }

        public Packer(uint height, uint width, PackerMode mode = PackerMode.Horizontal_First) {
            Map = new bool[height, width];
            this.Height = height;
            this.Width = width;
            this.Mode = mode;
        }

        /// <summary>
        /// Attempt to determine the best 'mode' of insertion
        /// </summary>
        /// <param name="shapes"></param>
        /// <returns></returns>
        public PackerMode GetPreferredMode(List<Shape> shapes) {
            shapes.Sort();

            int vertical = CanInsertCountVertical(shapes);
            int horizontal = CanInsertCountHorizontal(shapes);

            if (vertical > horizontal)
                return PackerMode.Vertical_First;
            else
                return PackerMode.Horizontal_First;
        }

        private int CanInsertCountVertical(List<Shape> shapes) {
            int n = 0;
            bool[,] map = new bool[Height, Width];
            foreach (Shape s in shapes) {
                if (InsertVerticalFirst(map, s) != null)
                    n++;
            }

            return n;
        }

        private int CanInsertCountHorizontal(List<Shape> shapes) {
            int n = 0;
            bool[,] map = new bool[Height, Width];
            foreach (Shape s in shapes) {
                if (InsertVerticalFirst(map, s) != null)
                    n++;
            }

            return n;
        }

        /// <summary>
        /// Attempt to insert the given item/shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public Position Insert(Shape shape) {
            return SelectedInsertFunc(Map, shape);
        }

        /// <summary>
        /// Insert the given item/shape at a specific position
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Insert(Shape shape, uint x, uint y) {
            return Fits(Map, shape, x, y);
        }

        private Position InsertVerticalFirst(bool[,] map, Shape shape) {
            for (uint dy = 0; dy < Height - shape.Height; dy++) {
                for (uint dx = 0; dx < Width - shape.Width; dx++) {
                    if (Fits(map, shape, dx, dy)) {
                        return new Position { X = dx, Y = dy };
                    }
                }
            }

            return null;
        }

        private Position InsertHorizontalFirst(bool[,] map, Shape shape) {
            for (uint dx = 0; dx <= Width - shape.Width; dx++) {
                for (uint dy = 0; dy <= Height - shape.Height; dy++) {
                    if (Fits(map, shape, dx, dy)) {
                        return new Position { X = dx, Y = dy };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to fit an item into the InventorySack
        /// The method returns true if the item has been successfully reserved
        /// </summary>
        /// <param name="map"></param>
        /// <param name="s"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool Fits(bool[,] map, Shape s, uint x, uint y) {
            // Out of bounds
            if (x + s.Width > Width)
                return false;
            else if (y + s.Height > Height)
                return false;

            // Check if the location is available
            for (uint dx = 0; dx < s.Width; dx++) {
                for (uint dy = 0; dy < s.Height; dy++) {
                    if (map[y + dy, x + dx])
                        return false;
                }
            }


            // Mark location as unavailable
            for (uint dx = 0; dx < s.Width; dx++) {
                for (uint dy = 0; dy < s.Height; dy++) {
                    map[y + dy, x + dx] = true;
                }
            }

            return true;
        }
    }
}
