using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arc {
    class DDSPixelFormat {
        public static int HEADER_SIZE = 32;
        public static int ALPHA_PIXELS = 1;
        public static int ALPHA = 2;
        public static int FOUR_CC = 4;
        public static int PALETTE_IDX = 32;
        public static int RGB = 64;
        public static int YUV = 512;
        public static int LUMINANCE = 131072;
        public static int R_BITMASK = 16711680;
        public static int G_BITMASK = 65280;
        public static int B_BITMASK = 255;
        public int size;
        public int flags;
        public int fourCC;
        public int rgbBitCount;
        public int rBitMask;
        public int gBitMask;
        public int bBitMask;
        public int aBitMask;
    }
}
