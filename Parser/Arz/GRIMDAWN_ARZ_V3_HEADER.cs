using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arz {
    public class GRIMDAWN_ARZ_V3_HEADER {
        public ushort Unknown { get; set; } // Assumed version check.
        public ushort Version { get; set; }
        public uint RecordTableStart { get; set; }
        public uint RecordTableSize { protected get; set; }
        public uint RecordTableEntryCount { get; set; }
        public uint StringTableStart { get; set; }
        public uint StringTableSize { get; set; }
    }
}
