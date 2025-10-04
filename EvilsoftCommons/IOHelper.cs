using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EvilsoftCommons {

    public class IOHelper {

        private static ILog logger = LogManager.GetLogger(typeof(IOHelper));


        public static bool IsFileLocked(FileInfo file) {
            FileStream stream = null;

            try {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }


        public static int ReadInteger(FileStream fs, bool endian = false) {
            byte[] array = new byte[4];
            if (fs.Read(array, 0, 4) != 4) {
                logger.WarnFormat("ReadInteger called with only {0} bytes remaining!", fs.Length - fs.Position);
                return 0;
            }
            else {
                if (endian && BitConverter.IsLittleEndian) {
                    Array.Reverse(array);
                }
                return BitConverter.ToInt32(array, 0);
            }
        }
        public static string GetPrefixString(byte[] data, int pos) {
            List<char> str = new List<char>();

            int length = GetInt(data, pos);
            pos += 4;
            int max = pos + length;

            while (pos < data.Length && pos < max) {
                str.Add(Convert.ToChar(data[pos++]));
            }

            return new string(str.ToArray<char>());
        }


        public static int ReadInteger(FileStream fs) {
            byte[] array = new byte[4];
            if (fs.Read(array, 0, 4) != 4) {
                logger.WarnFormat("ReadInteger called with only {0} bytes remaining!", fs.Length - fs.Position);
                return 0;
            }
            else {
                if (!BitConverter.IsLittleEndian) {
                    Array.Reverse(array);
                }
                return BitConverter.ToInt32(array, 0);
            }
        }

        public static long ReadLong(FileStream fs) {
            byte[] array = new byte[8];
            if (fs.Read(array, 0, 8) != 8) {
                logger.WarnFormat("ReadLong called with only {0} bytes remaining!", fs.Length - fs.Position);
                return 0;
            }
            else {
                if (!BitConverter.IsLittleEndian) {
                    Array.Reverse(array);
                }
                return BitConverter.ToInt64(array, 0);
            }
        }


        public static uint ReadUInteger(FileStream fs, bool endian = false) {
            byte[] array = new byte[4];
            if (fs.Read(array, 0, 4) != 4) {
                logger.WarnFormat("ReadUInteger called with only {0} bytes remaining!", fs.Length - fs.Position);
                return 0;
            }
            else {
                if (endian && BitConverter.IsLittleEndian) {
                    Array.Reverse(array);
                }
                return BitConverter.ToUInt32(array, 0);
            }
        }


        public static ushort ReadUShort(FileStream fs, bool endian = false) {
            byte[] array = new byte[2];
            if (fs.Read(array, 0, 2) != 2) {
                logger.WarnFormat("ReadUShort called with only {0} bytes remaining!", fs.Length - fs.Position);
                return 0;
            }
            else {
                if (endian && BitConverter.IsLittleEndian) {
                    Array.Reverse(array);
                }
                return BitConverter.ToUInt16(array, 0);
            }
        }


        public static string ReadString(FileStream fs) {
            uint length = ReadUInteger(fs);
            long remaining = fs.Length - fs.Position;
            if (length > remaining || length > 1024 * 10 || length <= 0) {
                //throw new ArgumentOutOfRangeException(String.Format("Could not parse string of length {0}", length));
                return string.Empty;
            }

            char[] sub = new char[length];
            byte[] buf = new byte[length];
            if (fs.Read(buf, 0, (int)length) != length) {
                logger.WarnFormat("ReadString requested {0} bytes but only got ... bytes", length);
            }
            for (int i = 0; i < length; i++) {
                sub[i] = Convert.ToChar(buf[i]);
            }

            return new String(sub);
        }


        public static string ReadString(FileStream fs, uint length) {
            long remaining = fs.Length - fs.Position;
            if (length > remaining || length > 1024 * 10 || length <= 0) {
                //throw new ArgumentOutOfRangeException(String.Format("Could not parse string of length {0}", length));
                return string.Empty;
            }

            char[] sub = new char[length];
            byte[] buf = new byte[length];
            if (fs.Read(buf, 0, (int)length) != length) {
                logger.WarnFormat("ReadString requested {0} bytes but only got ... bytes", length);
            }
            for (int i = 0; i < length; i++) {
                sub[i] = Convert.ToChar(buf[i]);
            }

            return new String(sub);
        }



        public static ushort GetShort(byte[] data, uint pos) {
            byte[] bytes = new byte[] { (byte)data[pos], (byte)data[pos + 1] };
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static ushort GetShort(byte[] data, int pos) {
            byte[] bytes = new byte[] { (byte)data[pos], (byte)data[pos + 1] };
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }


        public static uint GetUInt(byte[] data, uint pos) {
            byte[] bytes = new byte[] { (byte)data[pos], (byte)data[pos + 1], (byte)data[pos + 2], (byte)data[pos + 3] };
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
        public static uint GetUInt(byte[] data, int pos) {
            byte[] bytes = new byte[] { (byte)data[pos], (byte)data[pos + 1], (byte)data[pos + 2], (byte)data[pos + 3] };
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }


        public static float GetFloat(byte[] data, uint pos) {
            byte[] bytes = new byte[] { (byte)data[pos], (byte)data[pos + 1], (byte)data[pos + 2], (byte)data[pos + 3] };
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }


        public static int GetInt(byte[] data, int pos) {
            byte[] bytes = new byte[] { (byte)data[pos], (byte)data[pos + 1], (byte)data[pos + 2], (byte)data[pos + 3] };
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static void SetInt(byte[] data, int pos, int value) {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            Array.Copy(bytes, 0, data, pos, bytes.Length);
        }

        public static long GetLong(byte[] data, int pos) {
            byte[] bytes = new byte[] { (byte)data[pos], (byte)data[pos + 1], (byte)data[pos + 2], (byte)data[pos + 3], (byte)data[pos + 4], (byte)data[pos + 5], (byte)data[pos + 6], (byte)data[pos + 7] };
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }


        /// <summary>
        /// Return a null terminated string (not including the null)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static string GetNullString(byte[] data, int pos) {
            List<char> str = new List<char>();

            while (pos < data.Length && data[pos] != 0) {
                str.Add(Convert.ToChar(data[pos++]));
            }

            return new string(str.ToArray<char>());
        }

        public static ushort ReadShort(FileStream fs) {
            byte[] array = new byte[2];
            if (fs.Read(array, 0, 2) != 2) {
                logger.WarnFormat("ReadShort called with only {0} bytes remaining!", fs.Length - fs.Position);
                return 0;
            }
            else {
                return BitConverter.ToUInt16(array, 0);
            }
        }


        public static void WriteBytePrefixed(FileStream fs, string value) {
            List<char> str = new List<char>();
            if (string.IsNullOrEmpty(value)) {
                fs.WriteByte((byte)0);
            }
            else {
                byte[] data = Encoding.ASCII.GetBytes(value);
                System.Diagnostics.Debug.Assert(data.Length == value.Length);
                fs.WriteByte((byte)data.Length);
                Write(fs, data);
            }
        }

        public static void Write(FileStream fs, bool value) {
            fs.WriteByte((byte)(value ? 1 : 0));
        }
        public static void Write(FileStream fs, Int32 value) {
            Write(fs, BitConverter.GetBytes(value));
        }
        public static void Write(FileStream fs, Int16 value) {
            Write(fs, BitConverter.GetBytes(value));
        }
        public static void Write(FileStream fs, byte value) {
            fs.WriteByte(value);
        }
        public static void Write(FileStream fs, Int64 value) {
            Write(fs, BitConverter.GetBytes(value));
        }
        public static void Write(FileStream fs, uint value) {
            Write(fs, BitConverter.GetBytes(value));
        }
        public static void Write(FileStream fs, byte[] value) {
            fs.Write(value, 0, value.Length);
        }

        public static string GetBytePrefixedString(byte[] data, int pos) {
            List<char> str = new List<char>();

            int length = data[pos];
            pos += 1;
            int max = pos + length;

            while (pos < data.Length && pos < max) {
                str.Add(Convert.ToChar(data[pos++]));
            }

            return new string(str.ToArray<char>());
        }
    }
}
