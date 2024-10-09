using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc.Protocol {
    internal class IOMessageStateChange {
        public IOMessageStateChangeType Type { get; set; }
        public bool Value { get; set; }
    }
}
