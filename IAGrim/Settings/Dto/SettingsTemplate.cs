using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Settings.Dto {
    class SettingsTemplate {
        public Dictionary<string, object> Local { get; set; }
        public Dictionary<string, object> Persistent { get; set; }
    }
}

