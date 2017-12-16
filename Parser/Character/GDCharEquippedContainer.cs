using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    public class GDCharEquippedContainer {
        public GDItem[] Equipment = new GDItem[12]; // Equipped items, except for weapon slots
        public GDItem[] Weapon1 = new GDItem[2];   // Equipped items in weapon slots 1
        public GDItem[] Weapon2 = new GDItem[2];   // Equipped items in weapon slots 2

        public void Read(GDCryptoDataBuffer reader) {
            byte useAlternate = reader.ReadCryptoByteUnchecked();

            for (int i = 0; i < Equipment.Length; i = i + 1) {
                GDItem item = new GDItem(GDItem.GDItemType.EQUIPPED);
                item.Read(reader);
                Equipment[i] = item;
            }

            byte alternate1 = reader.ReadCryptoByteUnchecked();

            for (int i = 0; i < Weapon1.Length; i = i + 1) {
                GDItem item = new GDItem(GDItem.GDItemType.EQUIPPED);
                item.Read(reader);
                Weapon1[i] = item;
            }

            byte alternate2 = reader.ReadCryptoByteUnchecked();

            for (int i = 0; i < Weapon2.Length; i = i + 1) {
                GDItem item = new GDItem(GDItem.GDItemType.EQUIPPED);
                item.Read(reader);

                Weapon2[i] = item;
            }

        }
    }
}
