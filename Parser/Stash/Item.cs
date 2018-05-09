using System;

namespace IAGrim.StashFile {
    public class Item : IComparable<Item> {
        public override string ToString() {
            return $"Item[{BaseRecord},{PrefixRecord},{SuffixRecord},{ModifierRecord},{TransmuteRecord},{MateriaRecord},{Seed},{RelicCompletionBonusRecord},{RelicSeed},{EnchantmentRecord},{EnchantmentSeed},{MateriaCombines},{StackCount}]";
        }

        private static Random _random = new Random();

        public string BaseRecord = "";

        public string PrefixRecord = "";

        public string SuffixRecord = "";

        public string ModifierRecord = "";

        public string TransmuteRecord = "";

        public uint Seed = 0u;

        public string MateriaRecord = "";

        public string RelicCompletionBonusRecord = "";

        public uint RelicSeed = 0u;

        public string EnchantmentRecord = "";

        public uint UNKNOWN = 0u;

        public uint EnchantmentSeed = 0u;

        public uint MateriaCombines = 0u;

        public uint StackCount = 1u;

        public uint XOffset = 0;

        public uint YOffset = 0;

        // Default values chosen to minimize overlap while still allowing 16 items into the tab
        public int Height = 4;
        public int Width = 2;

        public Item() {
            this.RandomizeSeed();
            this.RandomizeRelicSeed();
        }

        public uint RandomizeSeed() {
            return this.Seed = (uint)Item._random.Next();
        }

        public uint RandomizeRelicSeed() {
            return this.RelicSeed = (uint)Item._random.Next();
        }

        public bool Read(GDCryptoDataBuffer pCrypto) {
            bool flag = !pCrypto.ReadCryptoString(out this.BaseRecord) || !pCrypto.ReadCryptoString(out this.PrefixRecord) 
                || !pCrypto.ReadCryptoString(out this.SuffixRecord) || !pCrypto.ReadCryptoString(out this.ModifierRecord) 
                || !pCrypto.ReadCryptoString(out this.TransmuteRecord) || !pCrypto.ReadCryptoUInt(out this.Seed) 
                || !pCrypto.ReadCryptoString(out this.MateriaRecord) || !pCrypto.ReadCryptoString(out this.RelicCompletionBonusRecord) 
                || !pCrypto.ReadCryptoUInt(out this.RelicSeed) || !pCrypto.ReadCryptoString(out this.EnchantmentRecord) 
                || !pCrypto.ReadCryptoUInt(out this.UNKNOWN) || !pCrypto.ReadCryptoUInt(out this.EnchantmentSeed) 
                || !pCrypto.ReadCryptoUInt(out this.MateriaCombines) || !pCrypto.ReadCryptoUInt(out this.StackCount);

            flag = flag || !pCrypto.ReadCryptoUInt(out this.XOffset) || !pCrypto.ReadCryptoUInt(out this.YOffset);
            return !flag;
        }

        public void Write(DataBuffer pBuffer) {
            pBuffer.WriteString(this.BaseRecord);
            pBuffer.WriteString(this.PrefixRecord);
            pBuffer.WriteString(this.SuffixRecord);
            pBuffer.WriteString(this.ModifierRecord);
            pBuffer.WriteString(this.TransmuteRecord);
            pBuffer.WriteUInt(this.Seed);
            pBuffer.WriteString(this.MateriaRecord);
            pBuffer.WriteString(this.RelicCompletionBonusRecord);
            pBuffer.WriteUInt(this.RelicSeed);
            pBuffer.WriteString(this.EnchantmentRecord);
            pBuffer.WriteUInt(this.UNKNOWN);
            pBuffer.WriteUInt(this.EnchantmentSeed);
            pBuffer.WriteUInt(this.MateriaCombines);
            pBuffer.WriteUInt(this.StackCount);
            pBuffer.WriteUInt(this.XOffset);
            pBuffer.WriteUInt(this.YOffset);
        }

        public int CompareTo(Item other) {
            return (Height * Width) - (other.Height * other.Width);
        }


        public override bool Equals(Object obj) {
            Item that = obj as Item;
            if (that == null)
                return base.Equals(obj);

            if (!Equals(this.BaseRecord, that.BaseRecord)) return false;
            if (!Equals(this.PrefixRecord, that.PrefixRecord)) return false;
            if (!Equals(this.SuffixRecord, that.SuffixRecord)) return false;
            if (!Equals(this.ModifierRecord, that.ModifierRecord)) return false;
            if (!Equals(this.TransmuteRecord, that.TransmuteRecord)) return false;
            if (!Equals(this.Seed, that.Seed)) return false;
            if (!Equals(this.MateriaRecord, that.MateriaRecord)) return false;
            if (!Equals(this.RelicCompletionBonusRecord, that.RelicCompletionBonusRecord)) return false;
            if (!Equals(this.RelicSeed, that.RelicSeed)) return false;
            if (!Equals(this.EnchantmentRecord, that.EnchantmentRecord)) return false;
            if (!Equals(this.EnchantmentSeed, that.EnchantmentSeed)) return false;
            if (!Equals(this.MateriaCombines, that.MateriaCombines)) return false;

            return true;
        }

    }
}