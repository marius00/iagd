using System;
using System.IO;
using System.Text;

namespace IAGrim.StashFile {
    public class DataBuffer {
        private byte[] mData = new byte[256];
        private int mWriteCursor = 0;
        private int mReadCursor = 0;
        private const int DEFAULT_SIZE = 256;


        public byte[] Data {
            get {
                return this.mData;
            }
            set {
                this.mData = value;
            }
        }

        public int Length {
            get {
                return this.mWriteCursor;
            }
            set {
                this.mWriteCursor = value;
            }
        }

        public int Cursor {
            get {
                return this.mReadCursor;
            }
            set {
                this.mReadCursor = value;
            }
        }

        public int Remaining {
            get {
                return this.mWriteCursor - this.mReadCursor;
            }
        }
        public static void WriteBytesToDisk(string pFullPath, byte[] pBytes) {
            if ((pFullPath == null) || (pFullPath.Length < 1)) {
                Console.WriteLine("Path cannot be null");
            }
            else {
                if (File.Exists(pFullPath)) {
                    File.Delete(pFullPath);
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open(pFullPath, FileMode.OpenOrCreate))) {
                    writer.Write(pBytes, 0, pBytes.Length);
                }
            }
        }
/*
 * // DOES NOT WORK
 * WILL CORRUPT FILES
 * LOOK AT COMPLEXITY OF WRITE UINT
        public void WriteFloat(float f) {
            byte[] b = BitConverter.GetBytes(f);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);

            WriteUInt(BitConverter.ToUInt32(b, 0));
        }
*/

        public void WriteUInt(uint pValue) {
            this.Prepare(4);
            int mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)(pValue & 0xff);
            mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)((pValue >> 8) & 0xff);
            mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)((pValue >> 0x10) & 0xff);
            mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)((pValue >> 0x18) & 0xff);
        }

        public void WriteString(string pValue) {
            if ((pValue == null) || (pValue.Length < 1)) {
                this.WriteInt(0);
            }
            else {
                byte[] bytes = Encoding.ASCII.GetBytes(pValue);
                this.WriteInt(bytes.Length);
                this.WriteRawBytes(bytes, 0, bytes.Length);
            }
        }
        public void WriteInt(int pValue) {
            this.Prepare(4);
            int mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)(pValue & 0xff);
            mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)((pValue >> 8) & 0xff);
            mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)((pValue >> 0x10) & 0xff);
            mWriteCursor = this.mWriteCursor;
            this.mWriteCursor = mWriteCursor + 1;
            this.mData[mWriteCursor] = (byte)((pValue >> 0x18) & 0xff);
        }


        public void WriteRawBytes(byte[] pBytes) {
            this.WriteRawBytes(pBytes, 0, pBytes.Length);
        }

        public void WriteBoolean(bool value) {
            byte[] b = { value ? (byte)1 : (byte)0 };
            this.WriteRawBytes(b, 0, b.Length);
        }

        public void WriteRawBytes(byte[] pBytes, int pStart, int pLength) {
            if (pLength < 1)
                return;

            this.Prepare(pLength);
            Buffer.BlockCopy((Array)pBytes, pStart, (Array)this.mData, this.mWriteCursor, pLength);
            this.mWriteCursor = this.mWriteCursor + pLength;
        }

        public static byte[] ReadBytesFromDisk(string pPath) {
            if ((pPath == null) || (pPath.Length < 1)) {
                return null;
            }
            if (!File.Exists(pPath)) {
                return null;
            }
            using (FileStream fs = File.OpenRead(pPath)) {
                using (BinaryReader reader = new BinaryReader(fs)) {
                    int length = (int)reader.BaseStream.Length;
                    int index = 0;
                    byte[] buffer = new byte[length];
                    while (length > 0) {
                        int count = (length < 0x7fff) ? length : 0x7fff;
                        while (count > 0) {
                            int num4 = reader.Read(buffer, index, count);
                            if (num4 == 0) {
                                break;
                            }
                            index += num4;
                            count -= num4;
                            length -= num4;
                        }
                    }
                    return buffer;
                }
            }
        }

        public bool ReadRawBytes(out byte[] pBytes, int pLength) {
            pBytes = null;
            if (pLength >= 1) {
                if ((this.mReadCursor + pLength) > this.mWriteCursor) {
                    return false;
                }
                pBytes = new byte[pLength];
                Buffer.BlockCopy(this.mData, this.mReadCursor, pBytes, 0, pLength);
                this.mReadCursor += pLength;
            }
            return true;
        }


        public bool ReadUInt(out uint pValue) {
            pValue = 0;
            if ((this.mReadCursor + 4) > this.mWriteCursor) {
                return false;
            }
            
            pValue = this.mData[mReadCursor++];
            pValue |= (uint)this.mData[mReadCursor++] << 8;
            pValue |= (uint)this.mData[mReadCursor++] << 0x10;
            pValue |= (uint)this.mData[mReadCursor++] << 0x18;

            return true;
        }

        public bool ReadBytes(out byte[] pValue, int length) {
            pValue = new byte[length];
            if ((this.mReadCursor + length) > this.mWriteCursor) {
                return false;
            }
            
            for (int i = 0; i < length; i++) {
                pValue[i] = this.mData[mReadCursor + i];
            }
            mReadCursor += length;

            return true;
        }


        private void Prepare(int pLength) {
            if (this.mData.Length - this.mWriteCursor >= pLength)
                return;
            int newSize = this.mData.Length + 256;
            while (newSize < this.mWriteCursor + pLength)
                newSize += 256;
            Array.Resize<byte>(ref this.mData, newSize);
        }
    }
}