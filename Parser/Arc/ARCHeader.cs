using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arc {
    class ARCHeader {
        public const int HEADER_SIZE = 28;  // 7 * int (4 bytes)

        public int unknown { get; set; }
        public int version { get; set; }
        public int num_files { get; set; }   // number of files in .ARC
        public int rec_num { get; set; }     // number of file parts, each record is 12 bytes long
        public int rec_size { get; set; }    // size of file part table (= rec_num * 12)
        public int str_size { get; set; }    // size of string table
        public int rec_offset { get; set; }  // offset for file parts

    }
}
