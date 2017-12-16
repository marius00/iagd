using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arc {
    /**
     * Format of a file part entry in a Grim Dawn .ARC file
     * 
     * Files can consist of multiple entries, as defined by the ToC
     * 
     * @author Michael Hermann
     * @version 1.0
     */
    class ARCFilePart {
        public int offset { get; set; }     // offset in .ARC file
        public int len_comp { get; set; }   // compressed length of file part
        public int len_decomp { get; set; } // decompressed length of file part
    }
}
