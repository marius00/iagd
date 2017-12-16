using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    class GDCharInventorySack {
        private const int BLOCK = 0;
        public void Read(GDCryptoDataBuffer reader) {

            reader.ReadBlockStart(BLOCK);

            reader.ReadCryptoByteUnchecked();


            int numItems = reader.ReadCryptoIntUnchecked();

            for (int i = 0; i < numItems; i = i + 1) {
                GDItem item = new GDItem(GDItem.GDItemType.INVENTORY);
                item.Read(reader);
            }

            reader.ReadBlockEnd();
        }
    }
}
