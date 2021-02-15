using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {
    class RequestCharacterDownloadUrlEventArg : EventArgs {
        public string Character { get; set; }
        public string Url { get; set; }
    }
}
