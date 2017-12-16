using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arc {
    class Record {

        public int type { get; set; }        // probably obsolete
        public int offset { get; set; }      // file offset
        public int len_comp { get; set; }    // compressed size
        public int len_decomp { get; set; }  // decompressed size
        public int unknown { get; set; }
        public long fileTime { get; set; }
        public int num_parts { get; set; }  // number of File Parts for this file
        public int index { get; set; }      // first index in File Parts
        public int str_len { get; set; }
        public int str_offset { get; set; }

        public byte[] data { get; set; }     // the decompressed content
        public String text { get; set; }     // the decompressed content as text
    }
}
