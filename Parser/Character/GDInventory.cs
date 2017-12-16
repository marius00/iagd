using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    public class GDInventory {
        private const int VERSION = 4;
        private const int BLOCK = 3;
        private GDCharEquippedContainer equipment = new GDCharEquippedContainer();


        public GDItem[] Equipment {
            get {
                return equipment.Equipment;
            }
        }
        public GDItem[] Weapon1 {
            get {
                return equipment.Weapon1;
            }
        }
        public GDItem[] Weapon2 {
            get {
                return equipment.Weapon2;
            }
        }

        public void Read(GDCryptoDataBuffer reader) {
            reader.ReadBlockStart(BLOCK);

            int version = reader.ReadCryptoIntUnchecked();
            if (version != VERSION)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");

            bool allGood;
            reader.ReadCryptoBool(out allGood);
            if (allGood) {
                int numSacks = reader.ReadCryptoIntUnchecked(true);

                int focused = reader.ReadCryptoIntUnchecked(true);
                int selected = reader.ReadCryptoIntUnchecked(true);

                for (int i = 0; i < numSacks; i = i + 1) {
                    GDCharInventorySack sack = new GDCharInventorySack();
                    sack.Read(reader);

                    //sacks.add(sack);
                }

                equipment.Read(reader);
            }

            // end marker
            reader.ReadBlockEnd();
        }
    }
}
