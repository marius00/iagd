using System;
using System.Collections.Generic;
using IAGrim.StashFile;

namespace IAGrim.Parser.Stash {
    public class Item : IComparable<Item> {
        public override string ToString() {
            return $"Item[{BaseRecord},{PrefixRecord},{SuffixRecord},{ModifierRecord},{TransmuteRecord},{MateriaRecord},{Seed},{RelicCompletionBonusRecord},{RelicSeed},{EnchantmentRecord},{EnchantmentSeed},{MateriaCombines},{StackCount}]";
        }

        private static readonly Random Random = new Random();

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
        public string AscendantRecord = "";
        public string AscendantRecord2H = "";

        public uint UNKNOWN = 0u;

        public uint EnchantmentSeed = 0u;

        public uint MateriaCombines = 0u;

        public uint StackCount = 1u;

        public uint Rerolls = 0u;

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
            return this.Seed = (uint)Item.Random.Next();
        }

        public uint RandomizeRelicSeed() {
            return this.RelicSeed = (uint)Item.Random.Next();
        }

        public bool Read(GDCryptoDataBuffer pCrypto) {
            bool flag = !pCrypto.ReadCryptoString(out this.BaseRecord) || !pCrypto.ReadCryptoString(out this.PrefixRecord) 
                || !pCrypto.ReadCryptoString(out this.SuffixRecord) || !pCrypto.ReadCryptoString(out this.ModifierRecord) 
                || !pCrypto.ReadCryptoString(out this.TransmuteRecord) || !pCrypto.ReadCryptoUInt(out this.Seed) 
                || !pCrypto.ReadCryptoString(out this.MateriaRecord) || !pCrypto.ReadCryptoString(out this.RelicCompletionBonusRecord) 
                || !pCrypto.ReadCryptoUInt(out this.RelicSeed) || !pCrypto.ReadCryptoString(out this.EnchantmentRecord)
                || !pCrypto.ReadCryptoString(out this.AscendantRecord) || !pCrypto.ReadCryptoString(out this.AscendantRecord2H)
                || !pCrypto.ReadCryptoUInt(out this.UNKNOWN) || !pCrypto.ReadCryptoUInt(out this.EnchantmentSeed) 
                || !pCrypto.ReadCryptoUInt(out this.MateriaCombines) || !pCrypto.ReadCryptoUInt(out this.StackCount) || !pCrypto.ReadCryptoUInt(out this.Rerolls);

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
            if (!Equals(this.AscendantRecord, that.AscendantRecord)) return false;
            if (!Equals(this.AscendantRecord2H, that.AscendantRecord2H)) return false;

            return true;
        }

        public override int GetHashCode() {
            var hashCode = -2107434431;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaseRecord);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PrefixRecord);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SuffixRecord);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModifierRecord);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TransmuteRecord);
            hashCode = hashCode * -1521134295 + Seed.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MateriaRecord);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RelicCompletionBonusRecord);
            hashCode = hashCode * -1521134295 + RelicSeed.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EnchantmentRecord);
            hashCode = hashCode * -1521134295 + EnchantmentSeed.GetHashCode();
            hashCode = hashCode * -1521134295 + MateriaCombines.GetHashCode();
            hashCode = hashCode * -1521134295 + StackCount.GetHashCode();
            return hashCode;
        }
    }
}