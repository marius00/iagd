using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arc {
    class DDSHeader {
        public static int HEADER_SIZE = 124;
        public static int VERSION_DEFAULT = 32;
        public static int VERSION_REVERSED = 82;
        public static int HEADER_CAPS = 1;
        public static int HEADER_HEIGHT = 2;
        public static int HEADER_WIDTH = 4;
        public static int HEADER_PITCH = 8;
        public static int HEADER_PIXELFORMAT = 4096;
        public static int HEADER_MIPMAP_COUNT = 131072;
        public static int HEADER_LINEAR_SIZE = 524288;
        public static int HEADER_DEPTH = 8388608;
        public static int CAPS_COMPLEX = 8;
        public static int CAPS_TEXTURE = 4096;
        public static int CAPS_MIPMAP = 4194304;
        public static int CAPS2_CUBEMAP = 512;
        public static int CAPS2_CUBE_POS_X = 1024;
        public static int CAPS2_CUBE_NEG_X = 2048;
        public static int CAPS2_CUBE_POS_Y = 4096;
        public static int CAPS2_CUBE_NEG_Y = 8192;
        public static int CAPS2_CUBE_POS_Z = 16384;
        public static int CAPS2_CUBE_NEG_Z = 32768;
        public static int CAPS2_VOLUME = 2097152;
        public byte[] version = new byte[4];
        public int size;
        public int flags;
        public int height;
        public int width;
        public int linearSize;
        public int depth;
        public int num_mipmap;
        public int[] reserved1 = new int[11];
        public DDSPixelFormat pixelFormat = new DDSPixelFormat();
        public int caps;
        public int caps2;
        public int caps3;
        public int caps4;
        public int reserved2;
    }
}
