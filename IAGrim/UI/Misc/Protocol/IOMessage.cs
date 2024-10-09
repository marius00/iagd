using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc.Protocol {
    internal class IOMessage {
        public IOMessageType Type { get; set; }
        public object Data { get; set; }
    }
}
