using System;
using System.IO;
using System.Text;

namespace IAGrim.StashFile {

    public class GDCryptoDataBuffer : DataBuffer {
        public const int XOR_KEY = 1431655765;

        public const int TABLE_SIZE = 256;

        public const int PRIME = 39916801;

        private uint m_Key = 0u;

        private uint[] m_Table = new uint[256];

        public GDCryptoDataBuffer() {
            base.Data = new byte[256];
        }

        public GDCryptoDataBuffer(byte[] pData) {
            base.Data = new byte[pData.Length];
            base.WriteRawBytes(pData, 0, base.Data.Length);
        }

        public void GenerateTable(uint pKey) {
            this.m_Table = new uint[256];
            for (int i = 0; i < this.m_Table.Length; i++) {
                pKey = (pKey >> 1 | pKey << 31);
                pKey *= 39916801u;
                this.m_Table[i] = pKey;
            }
        }

        public void UpdateKey(byte[] bytes) {
            bool flag = !BitConverter.IsLittleEndian;
            if (flag) {
                Array.Reverse(bytes);
            }
            uint num = 0u;
            while ((ulong)num < (ulong)((long)bytes.Length)) {
                this.m_Key ^= this.m_Table[(int)bytes[(int)num]];
                num += 1u;
            }
        }

        [Obsolete("Not supported", true)]
        public void UpdateKey(short pKey) { }

        [Obsolete("Not supported", true)]
        public void UpdateKey(byte pKey) { }

        public void UpdateKey(uint pKey) {
            byte[] bytes = BitConverter.GetBytes(pKey);
            UpdateKey(bytes);
        }

        public bool ReadCryptoKey() {
            uint num;
            bool flag = !base.ReadUInt(out num);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                num ^= 1431655765u;
                this.m_Key = num;
                this.GenerateTable(num);
                result = true;
            }
            return result;
        }


        public bool ReadCryptoUInt(out uint pValue) {
            pValue = 0u;
            uint num;
            bool flag = !base.ReadUInt(out num);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                pValue = (num ^ this.m_Key);
                this.UpdateKey(num);
                result = true;
            }
            return result;
        }

        public bool ReadCryptoBool(out bool pValue) {
            byte b;

            var r = ReadCryptoByte(out b);
            pValue = b == 1;
            return r;
        }
        public bool ReadCryptoByte(out byte pValue) {

            byte[] result;
            bool flag = !base.ReadBytes(out result, 1);
            pValue = result[0];


            if (flag) {
                return false;
            }
            else {
                pValue = (byte)(pValue ^ this.m_Key);
                this.UpdateKey(result);
                return true;
            }
        }
        public byte ReadCryptoByteUnchecked() {
            byte b;
            if (ReadCryptoByte(out b))
                return b;
            else
                return 0;
        }

        public void ReadAndDiscardUID() {
            byte b;
            for (int i = 0; i < 16; i++) {
                ReadCryptoByte(out b);
            }
        }

        public bool ReadCryptoInt(out int pValue, bool updateKey = true) {
            pValue = 0;
            uint num;
            bool flag = !base.ReadUInt(out num);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                pValue = unchecked((int)(num ^ this.m_Key));
                if (updateKey)
                    this.UpdateKey(num);
                result = true;
            }
            return result;
        }
        public int ReadCryptoIntUnchecked(bool updateKey = true) {
            int pValue = 0;
            uint num;
            bool flag = !base.ReadUInt(out num);
            if (flag) {
                
            }
            else {
                pValue = unchecked((int)(num ^ this.m_Key));
                if (updateKey)
                    this.UpdateKey(num);
            }
            return pValue;
        }

        public bool ReadNextCryptoUInt(out uint pValue) {
            pValue = 0u;
            bool flag = !base.ReadUInt(out pValue);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                pValue ^= this.m_Key;
                result = true;
            }
            return result;
        }

        public bool ReadCryptoFloat(out float pValue) {
            pValue = 0f;
            uint num;
            bool flag = !this.ReadCryptoUInt(out num);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                byte[] b = BitConverter.GetBytes(num);                
                pValue = BitConverter.ToSingle(b, 0);
                result = true;
            }
            return result;
        }
        public float ReadCryptoFloatUnchecked() {
            float f;
            
            if (ReadCryptoFloat(out f))
                return f;
            else
                return float.NaN;
        }

        public string ReadCryptoStringUnchecked() {
            string s;
            if (ReadCryptoString(out s))
                return s;
            else
                return string.Empty;
        }
        public bool ReadCryptoString(out string pValue) {
            pValue = "";
            uint len;
            bool flag = !this.ReadCryptoUInt(out len);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                bool flag2 = len > 0u;
                if (flag2) {
                    bool flag3 = (long)base.Cursor + (long)((ulong)len) > (long)base.Length;
                    if (flag3) {
                        result = false;
                        return result;
                    }
                    byte[] array;
                    bool flag4 = !base.ReadRawBytes(out array, (int)len);
                    if (flag4) {
                        result = false;
                        return result;
                    }
                    for (int i = 0; i < array.Length; i++) {
                        uint num2 = (uint)array[i];
                        num2 ^= this.m_Key;
                        this.m_Key ^= this.m_Table[(int)array[i]];
                        array[i] = (byte)num2;
                    }
                    pValue = Encoding.ASCII.GetString(array, 0, array.Length);
                }
                result = true;
            }
            return result;
        }
        public bool ReadCryptoWString(out string pValue) {
            pValue = "";
            uint len;
            bool flag = !this.ReadCryptoUInt(out len);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                bool flag2 = len > 0u;
                if (flag2) {
                    bool flag3 = (long)base.Cursor + (long)((ulong)len) > (long)base.Length;
                    if (flag3) {
                        result = false;
                        return result;
                    }
                    byte[] array;
                    bool flag4 = !base.ReadRawBytes(out array, (int)len * 2);
                    if (flag4) {
                        result = false;
                        return result;
                    }
                    for (int i = 0; i < array.Length; i++) {
                        uint num2 = (uint)array[i];
                        num2 ^= this.m_Key;
                        this.m_Key ^= this.m_Table[(int)array[i]];
                        array[i] = (byte)num2;
                    }
                    pValue = Encoding.UTF8.GetString(array, 0, array.Length);
                }
                result = true;
            }
            return result;
        }



        public void ReadBlockStart(int block) {

            int blockVersion = ReadCryptoIntUnchecked();
            int blockLength = ReadCryptoIntUnchecked(false);

            if (blockVersion != block)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");
        }
        public void ReadBlockEnd() {
            // end marker
            bool blockEnd = ReadCryptoIntUnchecked(false) == 0;
            if (!blockEnd)
                throw new Exception("Error reading block");
        }
    }
}