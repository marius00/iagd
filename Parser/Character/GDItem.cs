using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    public class GDItem {
        public enum GDItemType {
            STASH_SHARED, STASH_CHAR, EQUIPPED, INVENTORY
        };

        public string BaseRecord { get; set; }
        public string PrefixRecord { get; set; }
        public string SuffixRecord { get; set; }
        public string ModifierRecord { get; set; }
        public string TransmuteRecord { get; set; }
        public int Seed { get; set; }
        public string ComponentRecord { get; set; }
        public string CompletionBonus { get; set; }
        public int ComponentSeed { get; set; }
        public string AugmentRecord { get; set; }
        public int unknown { get; set; }
        public int AugmentSeed { get; set; }
        public int unknown2 { get; set; }

        public int StackCount { get; set; }

        private readonly GDItemType containerType;

        public GDItem(GDItemType containerType) {
            this.containerType = containerType;
        }

        public void Read(GDCryptoDataBuffer reader) {
            ReadInternalV4(reader);

            if ((containerType == GDItemType.STASH_SHARED) || (containerType == GDItemType.STASH_CHAR)) {
                float xPos = reader.ReadCryptoIntUnchecked();
                float yPos = reader.ReadCryptoIntUnchecked();
            }

            if (containerType == GDItemType.INVENTORY) {

                int x = reader.ReadCryptoIntUnchecked();
                int y = reader.ReadCryptoIntUnchecked();
            }

            if (containerType == GDItemType.EQUIPPED) {
                int attached = reader.ReadCryptoByteUnchecked();
            }

        }


        private void ReadInternalV4(GDCryptoDataBuffer reader) {
            BaseRecord = reader.ReadCryptoStringUnchecked();
            PrefixRecord = reader.ReadCryptoStringUnchecked();
            SuffixRecord = reader.ReadCryptoStringUnchecked();
            ModifierRecord = reader.ReadCryptoStringUnchecked();
            TransmuteRecord = reader.ReadCryptoStringUnchecked();
            Seed = reader.ReadCryptoIntUnchecked();

            ComponentRecord = reader.ReadCryptoStringUnchecked();
            CompletionBonus = reader.ReadCryptoStringUnchecked();
            ComponentSeed = reader.ReadCryptoIntUnchecked();
            AugmentRecord = reader.ReadCryptoStringUnchecked();
            unknown = reader.ReadCryptoIntUnchecked();
            AugmentSeed = reader.ReadCryptoIntUnchecked();
            unknown2 = reader.ReadCryptoIntUnchecked();

            StackCount = reader.ReadCryptoIntUnchecked();
        }
    }
}
