using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using log4net;
using System.IO;
using EvilsoftCommons;
using System.Threading.Tasks;

namespace IAGrim.Parser.Arc {
    public class DDSImageReader {
        private static ILog logger = LogManager.GetLogger(typeof(DDSImageReader));
        private const int DXT1 = 1146639409;
        private const int DXT2 = 1146639410;
        private const int DXT3 = 1146639411;
        private const int DXT4 = 1146639412;
        private const int DXT5 = 1146639413;
        private const int A1R5G5B5 = 65538;
        private const int X1R5G5B5 = 131074;
        private const int A4R4G4B4 = 196610;
        private const int X4R4G4B4 = 262146;
        private const int R5G6B5 = 327682;
        private const int R8G8B8 = 65539;
        private const int A8B8G8R8 = 65540;
        private const int X8B8G8R8 = 131076;
        private const int A8R8G8B8 = 196612;
        private const int X8R8G8B8 = 262148;
        private static int[] A1R5G5B5_MASKS = new int[] { 31744, 992, 31, 32768 };
        private static int[] X1R5G5B5_MASKS = new int[] { 31744, 992, 31, 0 };
        private static int[] A4R4G4B4_MASKS = new int[] { 3840, 240, 15, 61440 };
        private static int[] X4R4G4B4_MASKS = new int[] { 3840, 240, 15, 0 };
        private static int[] R5G6B5_MASKS = new int[] { 63488, 2016, 31, 0 };
        private static int[] R8G8B8_MASKS = new int[] { 16711680, 65280, 255, 0 };
        private static int[] A8B8G8R8_MASKS = new int[] { 255, 65280, 16711680, -16777216 };
        private static int[] X8B8G8R8_MASKS = new int[] { 255, 65280, 16711680, 0 };
        private static int[] A8R8G8B8_MASKS = new int[] { 16711680, 65280, 255, -16777216 };
        private static int[] X8R8G8B8_MASKS = new int[] { 16711680, 65280, 255, 0 };
        private static int[] BIT5 = new int[] { 0, 8, 16, 25, 33, 41, 49, 58, 66, 74, 82, 90, 99, 107, 115, 123, 132, 140, 148, 156, 165, 173, 181, 189, 197, 206, 214, 222, 230, 239, 247, 255 };
        private static int[] BIT6 = new int[] { 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 45, 49, 53, 57, 61, 65, 69, 73, 77, 81, 85, 89, 93, 97, 101, 105, 109, 113, 117, 121, 125, 130, 134, 138, 142, 146, 150, 154, 158, 162, 166, 170, 174, 178, 182, 186, 190, 194, 198, 202, 206, 210, 215, 219, 223, 227, 231, 235, 239, 243, 247, 251, 255 };

        /// <summary>
        /// Extract item icons to the target destination
        /// </summary>
        /// <param name="itemsArcFullpath">Full path to items.arc</param>
        /// <param name="destinationFolder"></param>
        public static void ExtractItemIcons(string itemsArcFullpath, string destinationFolder) {
            logger.Debug($"Extracting item icons from {itemsArcFullpath} to {destinationFolder}");
            try {

                if (!File.Exists(itemsArcFullpath)) {
                    logger.Warn($"The file {itemsArcFullpath} does not exist, icon extraction aborted.");

                }
                else if (!Directory.Exists(destinationFolder)) {
                    logger.Warn($"The specified output folder {destinationFolder} does not exist, icon extraction aborted.");
                }
                else {
                    Decompress dc = new Decompress(itemsArcFullpath, true);
                    dc.decompress();

                    foreach (string s in dc.strings) {
                        byte[] b = dc.GetTexture(s);
                        if (b != null && !s.EndsWith("_dif.tex") && !s.EndsWith("_nml.tex")) {
                            try {
                                Image img = ExtractImage(b);
                                img?.Save(Path.Combine(destinationFolder, $@"{Path.GetFileName(s)}.png"), ImageFormat.Png);
                            }
                            catch (Exception ex) {
                                logger.Warn(ex.Message);
                            }
                        }

                    }
                }
            }
            catch (UnauthorizedAccessException) {
                logger.Warn("Tried to parse GD icons while GD was running: Access denied");
                throw;
            }

            logger.Debug("Item icon extraction complete");
        }


        public static Image ExtractImage(byte[] bytes) {

            int size = IOHelper.GetInt(bytes, 8);
            byte[] rawPixels = new byte[size];
            Array.Copy(bytes, 12, rawPixels, 0, rawPixels.Length);


            DDSHeader ddsHdr = getDDSHeader(rawPixels);
            if (ddsHdr == null)
                return null;
            fixDDSHeader(rawPixels, ddsHdr);

            int w = GetWidth(rawPixels);
            int h = GetHeight(rawPixels);
            if (w > 96 || w > 96)
                return null;

            int[] decodedPixels = read(rawPixels, 0);
            if (decodedPixels == null)
                return null;

            {
                Bitmap bitmap1 = bitmapFromBytes(GetWidth(rawPixels), GetHeight(rawPixels), decodedPixels);
                return bitmap1;
            }
        }

        private static Bitmap bitmapFromBytes(int width, int height, int[] arr) {
            var b = new Bitmap(width, height, PixelFormat.Format32bppArgb);


            /*
                        ColorPalette ncp = b.Palette;
                        for (int i = 0; i < 256; i++)
                            ncp.Entries[i] = Color.FromArgb(255, 255 - i, 255 - i, 255 - i);
                        b.Palette = ncp;*/

            var BoundsRect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Width * b.Height;

            // fill in rgbValues, e.g. with a for loop over an input array
            System.Runtime.InteropServices.Marshal.Copy(arr, 0, ptr, bytes);
            b.UnlockBits(bmpData);

            return b;
        }

        private static Bitmap bitmapFromBytes(int width, int height, byte[] arr) {
            var b = new Bitmap(width, height, PixelFormat.Format32bppPArgb);


            /*
                        ColorPalette ncp = b.Palette;
                        for (int i = 0; i < 256; i++)
                            ncp.Entries[i] = Color.FromArgb(255, 255 - i, 255 - i, 255 - i);
                        b.Palette = ncp;*/

            var BoundsRect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * b.Height;

            // fill in rgbValues, e.g. with a for loop over an input array
            System.Runtime.InteropServices.Marshal.Copy(arr, 0, ptr, bytes);
            b.UnlockBits(bmpData);

            return b;
        }
        /*
        private static byte[] convert(int[] data) {
            byte[] result = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++) {
                byte[] bytes = BitConverter.GetBytes(data[i]);
                if (!BitConverter.IsLittleEndian) {
                    //Array.Reverse(bytes);
                }

                result[i * 4 + 0] = (byte)bytes[0];
                result[i * 4 + 1] = (byte)bytes[1];
                result[i * 4 + 2] = (byte)bytes[2];
                result[i * 4 + 3] = (byte)bytes[3];
            }

            return result;
        }*/
        #region Fix

        private static DDSHeader getDDSHeader(byte[] bytes) {
            DDSHeader header = new DDSHeader();
            int offset = 0;

            if (bytes.Length < 4)
                return null;

            Array.Copy(bytes, offset, header.version, 0, 4);
            //header.version = GDReader.getBytes4(bytes, offset);
            header.size = IOHelper.GetInt(bytes, offset += 4);
            offset += 4;
            if (header.size != 124) {
                //logger.Warn("ERR_UNSUPPORTED_HD_SIZE");
                return null;
            }

            header.flags = IOHelper.GetInt(bytes, offset);
            header.height = IOHelper.GetInt(bytes, offset += 4);
            header.width = IOHelper.GetInt(bytes, offset += 4);
            header.linearSize = IOHelper.GetInt(bytes, offset += 4);
            header.depth = IOHelper.GetInt(bytes, offset += 4);
            header.num_mipmap = IOHelper.GetInt(bytes, offset += 4);
            offset += 4;
            for (int i = 0; i < header.reserved1.Length; ++i) {
                header.reserved1[i] = IOHelper.GetInt(bytes, offset);
                offset += 4;
            }
            header.pixelFormat.size = IOHelper.GetInt(bytes, offset);
            offset += 4;
            if (header.pixelFormat.size != 32) {
                throw new Exception("ERR_UNSUPPORTED_PIX_SIZE");
            }
            header.pixelFormat.flags = IOHelper.GetInt(bytes, offset);
            header.pixelFormat.fourCC = IOHelper.GetInt(bytes, offset += 4);
            header.pixelFormat.rgbBitCount = IOHelper.GetInt(bytes, offset += 4);
            header.pixelFormat.rBitMask = IOHelper.GetInt(bytes, offset += 4);
            header.pixelFormat.gBitMask = IOHelper.GetInt(bytes, offset += 4);
            header.pixelFormat.bBitMask = IOHelper.GetInt(bytes, offset += 4);
            header.pixelFormat.aBitMask = IOHelper.GetInt(bytes, offset += 4);
            header.caps = IOHelper.GetInt(bytes, offset += 4);
            header.caps2 = IOHelper.GetInt(bytes, offset += 4);
            header.caps3 = IOHelper.GetInt(bytes, offset += 4);
            header.caps4 = IOHelper.GetInt(bytes, offset += 4);
            header.reserved2 = IOHelper.GetInt(bytes, offset += 4);
            offset += 4;
            return header;
        }

        private static void fixDDSHeader(byte[] bytes, DDSHeader header) {
            if (header.version[3] == 82) {
                bytes[3] = 32;
            }
            int flags = header.flags | 1 | 2 | 4 | 4096;
            int caps = header.caps | 4096;
            if (header.num_mipmap > 1) {
                header.flags |= 131072;
                header.caps = header.caps | 8 | 4194304;
            }
            if ((header.caps2 & 512) != 0) {
                caps |= 8;
            }
            if (header.depth > 1) {
                flags |= 8388608;
                caps |= 8;
            }
            if ((header.pixelFormat.flags & 4) != 0 && header.linearSize != 0) {
                flags |= 524288;
            }
            if (((header.pixelFormat.flags & 64) == 64 || (header.pixelFormat.flags & 512) == 512 || (header.pixelFormat.flags & 131072) == 131072 || (header.pixelFormat.flags & 2) == 2) && header.linearSize != 0) {
                flags |= 8;
            }

            //Array.Copy(flags, 0, bytes, 8, flags.)

            IOHelper.SetInt(bytes, 8, flags);
            IOHelper.SetInt(bytes, 80, header.pixelFormat.flags);
            IOHelper.SetInt(bytes, 92, 16711680);
            IOHelper.SetInt(bytes, 96, 65280);
            IOHelper.SetInt(bytes, 100, 255);
            IOHelper.SetInt(bytes, 104, 255);
            IOHelper.SetInt(bytes, 108, caps);
        }
        #endregion

        #region helpers
        private static int GetHeight(byte[] buffer) {
            return buffer[12] & 255 | (buffer[13] & 255) << 8 | (buffer[14] & 255) << 16 | (buffer[15] & 255) << 24;
        }

        private static int GetWidth(byte[] buffer) {
            return buffer[16] & 255 | (buffer[17] & 255) << 8 | (buffer[18] & 255) << 16 | (buffer[19] & 255) << 24;
        }

        private static int GetMipmap(byte[] buffer) {
            return buffer[28] & 255 | (buffer[29] & 255) << 8 | (buffer[30] & 255) << 16 | (buffer[31] & 255) << 24;
        }
        public static int getBitCount(byte[] buffer) {
            return buffer[88] & 255 | (buffer[89] & 255) << 8 | (buffer[90] & 255) << 16 | (buffer[91] & 255) << 24;
        }

        public static int getRedMask(byte[] buffer) {
            return buffer[92] & 255 | (buffer[93] & 255) << 8 | (buffer[94] & 255) << 16 | (buffer[95] & 255) << 24;
        }

        public static int getGreenMask(byte[] buffer) {
            return buffer[96] & 255 | (buffer[97] & 255) << 8 | (buffer[98] & 255) << 16 | (buffer[99] & 255) << 24;
        }

        public static int getBlueMask(byte[] buffer) {
            return buffer[100] & 255 | (buffer[101] & 255) << 8 | (buffer[102] & 255) << 16 | (buffer[103] & 255) << 24;
        }

        public static int getAlphaMask(byte[] buffer) {
            return buffer[104] & 255 | (buffer[105] & 255) << 8 | (buffer[106] & 255) << 16 | (buffer[107] & 255) << 24;
        }
        public static int getPixelFormatFlags(byte[] buffer) {
            return buffer[80] & 255 | (buffer[81] & 255) << 8 | (buffer[82] & 255) << 16 | (buffer[83] & 255) << 24;
        }

        public static int getFourCC(byte[] buffer) {
            return (buffer[84] & 255) << 24 | (buffer[85] & 255) << 16 | (buffer[86] & 255) << 8 | buffer[87] & 255;
        }

        #endregion

        private static int getType(byte[] buffer) {
            int type = 0;
            int flags = getPixelFormatFlags(buffer);
            if ((flags & 4) != 0) {
                type = getFourCC(buffer);
            }
            else if ((flags & 64) != 0) {
                int alphaMask;
                int bitCount = getBitCount(buffer);
                int redMask = getRedMask(buffer);
                int greenMask = getGreenMask(buffer);
                int blueMask = getBlueMask(buffer);
                int n = alphaMask = (flags & 1) != 0 ? getAlphaMask(buffer) : 0;
                if (bitCount == 16) {
                    if (redMask == A1R5G5B5_MASKS[0] && greenMask == A1R5G5B5_MASKS[1] && blueMask == A1R5G5B5_MASKS[2] && alphaMask == A1R5G5B5_MASKS[3]) {
                        type = 65538;
                    }
                    else if (redMask == X1R5G5B5_MASKS[0] && greenMask == X1R5G5B5_MASKS[1] && blueMask == X1R5G5B5_MASKS[2] && alphaMask == X1R5G5B5_MASKS[3]) {
                        type = 131074;
                    }
                    else if (redMask == A4R4G4B4_MASKS[0] && greenMask == A4R4G4B4_MASKS[1] && blueMask == A4R4G4B4_MASKS[2] && alphaMask == A4R4G4B4_MASKS[3]) {
                        type = 196610;
                    }
                    else if (redMask == X4R4G4B4_MASKS[0] && greenMask == X4R4G4B4_MASKS[1] && blueMask == X4R4G4B4_MASKS[2] && alphaMask == X4R4G4B4_MASKS[3]) {
                        type = 262146;
                    }
                    else if (redMask == R5G6B5_MASKS[0] && greenMask == R5G6B5_MASKS[1] && blueMask == R5G6B5_MASKS[2] && alphaMask == R5G6B5_MASKS[3]) {
                        type = 327682;
                    }
                }
                else if (bitCount == 24) {
                    if (redMask == R8G8B8_MASKS[0] && greenMask == R8G8B8_MASKS[1] && blueMask == R8G8B8_MASKS[2] && alphaMask == R8G8B8_MASKS[3]) {
                        type = 65539;
                    }
                }
                else if (bitCount == 32) {
                    if (redMask == A8B8G8R8_MASKS[0] && greenMask == A8B8G8R8_MASKS[1] && blueMask == A8B8G8R8_MASKS[2] && alphaMask == A8B8G8R8_MASKS[3]) {
                        type = 65540;
                    }
                    else if (redMask == X8B8G8R8_MASKS[0] && greenMask == X8B8G8R8_MASKS[1] && blueMask == X8B8G8R8_MASKS[2] && alphaMask == X8B8G8R8_MASKS[3]) {
                        type = 131076;
                    }
                    else if (redMask == A8R8G8B8_MASKS[0] && greenMask == A8R8G8B8_MASKS[1] && blueMask == A8R8G8B8_MASKS[2] && alphaMask == A8R8G8B8_MASKS[3]) {
                        type = 196612;
                    }
                    else if (redMask == X8R8G8B8_MASKS[0] && greenMask == X8R8G8B8_MASKS[1] && blueMask == X8R8G8B8_MASKS[2] && alphaMask == X8R8G8B8_MASKS[3]) {
                        type = 262148;
                    }
                }
            }
            return type;
        }


        public static int[] read(byte[] buffer, int mipmapLevel) {
            int width = GetWidth(buffer);
            int height = GetHeight(buffer);
            int mipmap = GetMipmap(buffer);
            int type = getType(buffer);
            if (type == 0) {
                return null;
            }
            int offset = 128;
            if (mipmapLevel > 0 && mipmapLevel < mipmap) {
                for (int i = 0; i < mipmapLevel; ++i) {
                    switch (type) {
                        case 1146639409: {
                                offset += 8 * ((width + 3) / 4) * ((height + 3) / 4);
                                break;
                            }
                        case 1146639410:
                        case 1146639411:
                        case 1146639412:
                        case 1146639413: {
                                offset += 16 * ((width + 3) / 4) * ((height + 3) / 4);
                                break;
                            }
                        case 65538:
                        case 65539:
                        case 65540:
                        case 131074:
                        case 131076:
                        case 196610:
                        case 196612:
                        case 262146:
                        case 262148:
                        case 327682: {
                                offset += (type & 255) * width * height;
                            } break;
                    }
                    width /= 2;
                    height /= 2;
                }
                if (width <= 0) {
                    width = 1;
                }
                if (height <= 0) {
                    height = 1;
                }
            }
            int[] pixels = null;
            switch (type) {
                case 1146639409: {
                        pixels = decodeDXT1(width, height, offset, buffer);
                        break;
                    }
                case 1146639410: {
                        pixels = decodeDXT2(width, height, offset, buffer);
                        break;
                    }
                case 1146639411: {
                        pixels = decodeDXT3(width, height, offset, buffer);
                        break;
                    }
                case 1146639412: {
                        pixels = decodeDXT4(width, height, offset, buffer);
                        break;
                    }
                case 1146639413: {
                        pixels = decodeDXT5(width, height, offset, buffer);
                        break;
                    }
                case 65538: {
                        pixels = readA1R5G5B5(width, height, offset, buffer);
                        break;
                    }
                case 131074: {
                        pixels = readX1R5G5B5(width, height, offset, buffer);
                        break;
                    }
                case 196610: {
                        pixels = readA4R4G4B4(width, height, offset, buffer);
                        break;
                    }
                case 262146: {
                        pixels = readX4R4G4B4(width, height, offset, buffer);
                        break;
                    }
                case 327682: {
                        pixels = readR5G6B5(width, height, offset, buffer);
                        break;
                    }
                case 65539: {
                        pixels = readR8G8B8(width, height, offset, buffer);
                        break;
                    }
                case 65540: {
                        pixels = readA8B8G8R8(width, height, offset, buffer);
                        break;
                    }
                case 131076: {
                        pixels = readX8B8G8R8(width, height, offset, buffer);
                        break;
                    }
                case 196612: {
                        pixels = readA8R8G8B8(width, height, offset, buffer);
                        break;
                    }
                case 262148: {
                        pixels = readA8R8G8B8(width, height, offset, buffer);
                    } break;
            }
            return pixels;
        }


        #region Decoders

        private static int[] decodeDXT1(int width, int height, int offset, byte[] buffer) {
            int[] pixels = new int[width * height];
            int index = offset;
            int w = (width + 3) / 4;
            int h = (height + 3) / 4;
            for (int i = 0; i < h; ++i) {
                for (int j = 0; j < w; ++j) {
                    int c0 = buffer[index] & 255 | (buffer[index + 1] & 255) << 8;
                    int c1 = buffer[index] & 255 | (buffer[(index += 2) + 1] & 255) << 8;
                    index += 2;
                    for (int k = 0; k < 4 && 4 * i + k < height; ++k) {
                        int t0 = buffer[index] & 3;
                        int t1 = (buffer[index] & 12) >> 2;
                        int t2 = (buffer[index] & 48) >> 4;
                        int t3 = (buffer[index++] & 192) >> 6;
                        pixels[4 * width * i + 4 * j + width * k + 0] = getDXTColor(c0, c1, 255, t0);
                        if (4 * j + 1 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k + 1] = getDXTColor(c0, c1, 255, t1);
                        if (4 * j + 2 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k + 2] = getDXTColor(c0, c1, 255, t2);
                        if (4 * j + 3 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k + 3] = getDXTColor(c0, c1, 255, t3);
                    }
                }
            }
            return pixels;
        }

        private static int[] decodeDXT2(int width, int height, int offset, byte[] buffer) {
            return decodeDXT3(width, height, offset, buffer);
        }

        private static int[] decodeDXT3(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int w = (width + 3) / 4;
            int h = (height + 3) / 4;
            int[] pixels = new int[width * height];
            int[] alphaTable = new int[16];
            for (int i = 0; i < h; ++i) {
                for (int j = 0; j < w; ++j) {
                    for (int k = 0; k < 4; ++k) {
                        int a0 = buffer[index++] & 255;
                        int a1 = buffer[index++] & 255;
                        alphaTable[4 * k + 0] = 17 * ((a0 & 240) >> 4);
                        alphaTable[4 * k + 1] = 17 * (a0 & 15);
                        alphaTable[4 * k + 2] = 17 * ((a1 & 240) >> 4);
                        alphaTable[4 * k + 3] = 17 * (a1 & 15);
                    }
                    int c0 = buffer[index] & 255 | (buffer[index + 1] & 255) << 8;
                    int c1 = buffer[index] & 255 | (buffer[(index += 2) + 1] & 255) << 8;
                    index += 2;
                    for (int k2 = 0; k2 < 4 && 4 * i + k2 < height; ++k2) {
                        int t0 = buffer[index] & 3;
                        int t1 = (buffer[index] & 12) >> 2;
                        int t2 = (buffer[index] & 48) >> 4;
                        int t3 = (buffer[index++] & 192) >> 6;
                        pixels[4 * width * i + 4 * j + width * k2 + 0] = getDXTColor(c0, c1, alphaTable[4 * k2 + 0], t0);
                        if (4 * j + 1 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k2 + 1] = getDXTColor(c0, c1, alphaTable[4 * k2 + 1], t1);
                        if (4 * j + 2 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k2 + 2] = getDXTColor(c0, c1, alphaTable[4 * k2 + 2], t2);
                        if (4 * j + 3 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k2 + 3] = getDXTColor(c0, c1, alphaTable[4 * k2 + 3], t3);
                    }
                }
            }
            return pixels;
        }

        private static int[] decodeDXT4(int width, int height, int offset, byte[] buffer) {
            return decodeDXT5(width, height, offset, buffer);
        }

        private static int[] decodeDXT5(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int w = (width + 3) / 4;
            int h = (height + 3) / 4;
            int[] pixels = new int[width * height];
            int[] alphaTable = new int[16];
            for (int i = 0; i < h; ++i) {
                for (int j = 0; j < w; ++j) {
                    int a0 = buffer[index++] & 255;
                    int a1 = buffer[index++] & 255;
                    int b0 = buffer[index] & 255 | (buffer[index + 1] & 255) << 8 | (buffer[index + 2] & 255) << 16;
                    int b1 = buffer[index] & 255 | (buffer[index + 1] & 255) << 8 | (buffer[(index += 3) + 2] & 255) << 16;
                    alphaTable[0] = b0 & 7;
                    alphaTable[1] = b0 >> 3 & 7;
                    alphaTable[2] = b0 >> 6 & 7;
                    alphaTable[3] = b0 >> 9 & 7;
                    alphaTable[4] = b0 >> 12 & 7;
                    alphaTable[5] = b0 >> 15 & 7;
                    alphaTable[6] = b0 >> 18 & 7;
                    alphaTable[7] = b0 >> 21 & 7;
                    alphaTable[8] = b1 & 7;
                    alphaTable[9] = b1 >> 3 & 7;
                    alphaTable[10] = b1 >> 6 & 7;
                    alphaTable[11] = b1 >> 9 & 7;
                    alphaTable[12] = b1 >> 12 & 7;
                    alphaTable[13] = b1 >> 15 & 7;
                    alphaTable[14] = b1 >> 18 & 7;
                    alphaTable[15] = b1 >> 21 & 7;
                    int c0 = buffer[index] & 255 | (buffer[(index += 3) + 1] & 255) << 8;
                    int c1 = buffer[index] & 255 | (buffer[(index += 2) + 1] & 255) << 8;
                    index += 2;
                    for (int k = 0; k < 4 && 4 * i + k < height; ++k) {
                        int t0 = buffer[index] & 3;
                        int t1 = (buffer[index] & 12) >> 2;
                        int t2 = (buffer[index] & 48) >> 4;
                        int t3 = (buffer[index++] & 192) >> 6;
                        pixels[4 * width * i + 4 * j + width * k + 0] = getDXTColor(c0, c1, getDXT5Alpha(a0, a1, alphaTable[4 * k + 0]), t0);
                        if (4 * j + 1 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k + 1] = getDXTColor(c0, c1, getDXT5Alpha(a0, a1, alphaTable[4 * k + 1]), t1);
                        if (4 * j + 2 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k + 2] = getDXTColor(c0, c1, getDXT5Alpha(a0, a1, alphaTable[4 * k + 2]), t2);
                        if (4 * j + 3 >= width) continue;
                        pixels[4 * width * i + 4 * j + width * k + 3] = getDXTColor(c0, c1, getDXT5Alpha(a0, a1, alphaTable[4 * k + 3]), t3);
                    }
                }
            }
            return pixels;
        }
        #endregion


        #region Readers

        private static int[] readA1R5G5B5(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int rgba = buffer[index] & 255 | (buffer[index + 1] & 255) << 8;
                index += 2;
                int r = BIT5[(rgba & A1R5G5B5_MASKS[0]) >> 10];
                int g = BIT5[(rgba & A1R5G5B5_MASKS[1]) >> 5];
                int b = BIT5[rgba & A1R5G5B5_MASKS[2]];
                int a = 255 * ((rgba & A1R5G5B5_MASKS[3]) >> 15);
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readX1R5G5B5(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int rgba = buffer[index] & 255 | (buffer[index + 1] & 255) << 8;
                index += 2;
                int r = BIT5[(rgba & X1R5G5B5_MASKS[0]) >> 10];
                int g = BIT5[(rgba & X1R5G5B5_MASKS[1]) >> 5];
                int b = BIT5[rgba & X1R5G5B5_MASKS[2]];
                int a = 255;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readA4R4G4B4(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int rgba = buffer[index] & 255 | (buffer[index + 1] & 255) << 8;
                index += 2;
                int r = 17 * ((rgba & A4R4G4B4_MASKS[0]) >> 8);
                int g = 17 * ((rgba & A4R4G4B4_MASKS[1]) >> 4);
                int b = 17 * (rgba & A4R4G4B4_MASKS[2]);
                int a = 17 * ((rgba & A4R4G4B4_MASKS[3]) >> 12);
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readX4R4G4B4(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int rgba = buffer[index] & 255 | (buffer[index + 1] & 255) << 8;
                index += 2;
                int r = 17 * ((rgba & A4R4G4B4_MASKS[0]) >> 8);
                int g = 17 * ((rgba & A4R4G4B4_MASKS[1]) >> 4);
                int b = 17 * (rgba & A4R4G4B4_MASKS[2]);
                int a = 255;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readR5G6B5(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int rgba = buffer[index] & 255 | (buffer[index + 1] & 255) << 8;
                index += 2;
                int r = BIT5[(rgba & R5G6B5_MASKS[0]) >> 11];
                int g = BIT6[(rgba & R5G6B5_MASKS[1]) >> 5];
                int b = BIT5[rgba & R5G6B5_MASKS[2]];
                int a = 255;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readR8G8B8(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int b = buffer[index++] & 255;
                int g = buffer[index++] & 255;
                int r = buffer[index++] & 255;
                int a = 255;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readA8B8G8R8(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int r = buffer[index++] & 255;
                int g = buffer[index++] & 255;
                int b = buffer[index++] & 255;
                int a = buffer[index++] & 255;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readX8B8G8R8(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int r = buffer[index++] & 255;
                int g = buffer[index++] & 255;
                int b = buffer[index++] & 255;
                int a = 255;
                ++index;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readA8R8G8B8(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int b = buffer[index++] & 255;
                int g = buffer[index++] & 255;
                int r = buffer[index++] & 255;
                int a = buffer[index++] & 255;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }

        private static int[] readX8R8G8B8(int width, int height, int offset, byte[] buffer) {
            int index = offset;
            int[] pixels = new int[width * height];
            for (int i = 0; i < height * width; ++i) {
                int b = buffer[index++] & 255;
                int g = buffer[index++] & 255;
                int r = buffer[index++] & 255;
                int a = 255;
                ++index;
                //pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
                pixels[i] = a << 24 | r << 16 | g << 8 | b << 0;
            }
            return pixels;
        }
        #endregion



        #region Colors
        private static int getDXTColor(int c0, int c1, int a, int t) {
            switch (t) {
                case 0: {
                        return getDXTColor1(c0, a);
                    }
                case 1: {
                        return getDXTColor1(c1, a);
                    }
                case 2: {
                        return c0 > c1 ? getDXTColor2_1(c0, c1, a) : getDXTColor1_1(c0, c1, a);
                    }
                case 3: {
                        return c0 > c1 ? getDXTColor2_1(c1, c0, a) : 0;
                    }
            }
            return 0;
        }

        private static int getDXTColor2_1(int c0, int c1, int a) {
            int r = (2 * BIT5[(c0 & 64512) >> 11] + BIT5[(c1 & 64512) >> 11]) / 3;
            int g = (2 * BIT6[(c0 & 2016) >> 5] + BIT6[(c1 & 2016) >> 5]) / 3;
            int b = (2 * BIT5[c0 & 31] + BIT5[c1 & 31]) / 3;
            return a << 16 | r << 8 | g << 0 | b << 24;
        }

        private static int getDXTColor1_1(int c0, int c1, int a) {
            int r = (BIT5[(c0 & 64512) >> 11] + BIT5[(c1 & 64512) >> 11]) / 2;
            int g = (BIT6[(c0 & 2016) >> 5] + BIT6[(c1 & 2016) >> 5]) / 2;
            int b = (BIT5[c0 & 31] + BIT5[c1 & 31]) / 2;
            return a << 16 | r << 8 | g << 0 | b << 24;
        }

        private static int getDXTColor1(int c, int a) {
            int r = BIT5[(c & 64512) >> 11];
            int g = BIT6[(c & 2016) >> 5];
            int b = BIT5[c & 31];
            return a << 16 | r << 8 | g << 0 | b << 24;
        }

        private static int getDXT5Alpha(int a0, int a1, int t) {
            if (a0 > a1) {
                switch (t) {
                    case 0: {
                            return a0;
                        }
                    case 1: {
                            return a1;
                        }
                    case 2: {
                            return (6 * a0 + a1) / 7;
                        }
                    case 3: {
                            return (5 * a0 + 2 * a1) / 7;
                        }
                    case 4: {
                            return (4 * a0 + 3 * a1) / 7;
                        }
                    case 5: {
                            return (3 * a0 + 4 * a1) / 7;
                        }
                    case 6: {
                            return (2 * a0 + 5 * a1) / 7;
                        }
                    case 7: {
                            return (a0 + 6 * a1) / 7;
                        }
                }
            }
            else {
                switch (t) {
                    case 0: {
                            return a0;
                        }
                    case 1: {
                            return a1;
                        }
                    case 2: {
                            return (4 * a0 + a1) / 5;
                        }
                    case 3: {
                            return (3 * a0 + 2 * a1) / 5;
                        }
                    case 4: {
                            return (2 * a0 + 3 * a1) / 5;
                        }
                    case 5: {
                            return (a0 + 4 * a1) / 5;
                        }
                    case 6: {
                            return 0;
                        }
                    case 7: {
                            return 255;
                        }
                }
            }
            return 0;
        }

        #endregion
    }
}
