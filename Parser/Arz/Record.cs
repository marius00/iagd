using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Parser.Arz {

    public class Record {
        public uint StringIndex { get; set; }
        public string Type { get; set; }

        public uint Offset { get; set; }

        public uint SizeUncompressed {
            get {
                return _SizeUncompressed;
            }
            set {
                _SizeUncompressed = value;
                Uncompressed = new byte[value];
            }
        }

        public byte[] Compressed { get; set; }
        public byte[] Uncompressed { get; set; }


        private uint _SizeUncompressed;

    }
}
