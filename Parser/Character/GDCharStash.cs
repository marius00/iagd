using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    class GDCharStash {
        private const int VERSION = 5;
        private const int BLOCK = 4;


        public void Read(GDCryptoDataBuffer reader) {
            reader.ReadBlockStart(BLOCK);

            int version = reader.ReadCryptoIntUnchecked();
            if (version != VERSION)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");

            int width = reader.ReadCryptoIntUnchecked();
            int height = reader.ReadCryptoIntUnchecked();
            int numItems = reader.ReadCryptoIntUnchecked();
            for (int i = 0; i < numItems; i++) {
                var item = new GDItem(GDItem.GDItemType.STASH_CHAR);
                item.Read(reader);
            }
            reader.ReadBlockEnd();
        }

    }
}
