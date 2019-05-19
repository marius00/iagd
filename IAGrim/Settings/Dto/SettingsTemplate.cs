using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Settings.Dto {
    class SettingsTemplate {
        public LocalSettings Local { get; set; }
        public PersistentSettings Persistent { get; set; }
    }
}

