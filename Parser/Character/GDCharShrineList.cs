using IAGrim.StashFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Parser.Character {
    class GDCharShrineList {
        private const int VERSION = 2;
        private const int BLOCK = 17;


        public void Read(GDCryptoDataBuffer reader) {
            reader.ReadBlockStart(BLOCK);

            int version = reader.ReadCryptoIntUnchecked();
            if (version != VERSION)
                throw new FormatException("ERR_UNSUPPORTED_VERSION");

            int numStates = 6;// 0 = Normal, found, 1 = Normal, restored, 2 = Epic, found and so forth
            for (int i = 0; i < numStates; i++) {
                int length = reader.ReadCryptoIntUnchecked();
                for (int m = 0; m < length; m++) {
                    reader.ReadAndDiscardUID();
                }
            }

            reader.ReadBlockEnd();
        }
    }
}
