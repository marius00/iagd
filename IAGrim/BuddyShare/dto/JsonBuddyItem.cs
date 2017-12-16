using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.BuddyShare.dto {
    public class JsonBuddyItem {
        public long Id { get; set; }
        public string BaseRecord { get; set; }
        public string PrefixRecord { get; set; }
        public string SuffixRecord { get; set; }
        public string ModifierRecord { get; set; }
        public string TransmuteRecord { get; set; }
        public string MateriaRecord { get; set; }
        public long StackCount { get; set; }
        public string Mod { get; set; }
    }
}
