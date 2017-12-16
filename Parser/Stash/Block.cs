using System;
namespace IAGrim.StashFile {
    public class Block {
        public const int DEFAULT_STASHTAB_RESULT = 0;

        public uint Result;

        public uint Length;

        public uint End;

        public static bool ReadStart(out Block pBlock, GDCryptoDataBuffer pCrypto) {
            pBlock = new Block();
            bool flag = !pCrypto.ReadCryptoUInt(out pBlock.Result) || !pCrypto.ReadNextCryptoUInt(out pBlock.Length);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                pBlock.End = (uint)(pCrypto.Cursor + (int)pBlock.Length);
                result = true;
            }
            return result;
        }

        public bool ReadEnd(GDCryptoDataBuffer pCrypto) {
            bool flag = (long)pCrypto.Cursor != (long)((ulong)this.End);
            bool result;
            if (flag) {
                result = false;
            }
            else {
                uint num;
                bool flag2 = !pCrypto.ReadNextCryptoUInt(out num) || num > 0u;
                result = !flag2;
            }
            return result;
        }

        public void WriteStart(uint pResult, DataBuffer pBuffer) {
            pBuffer.WriteUInt(pResult);
            pBuffer.WriteUInt(0);
            this.End = (uint)pBuffer.Length;
        }

        public void WriteEnd(DataBuffer pBuffer) {
            int length = pBuffer.Length;
            pBuffer.Length = ((int)this.End) - 4;
            pBuffer.WriteUInt(((uint)length) - this.End);
            pBuffer.Length = length;
            pBuffer.WriteInt(0);
        }


    }

}