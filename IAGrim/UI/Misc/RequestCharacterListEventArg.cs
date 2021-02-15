using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc {
    class RequestCharacterListEventArg : EventArgs {
        public List<string> Characters { get; set; }
    }
}
