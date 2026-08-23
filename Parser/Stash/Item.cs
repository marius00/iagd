using System;
using System.Collections.Generic;

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