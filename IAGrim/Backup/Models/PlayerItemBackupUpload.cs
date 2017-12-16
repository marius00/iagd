using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Models {
    class PlayerItemBackupUpload {

        public virtual long? Seed { get; set; }
        public virtual long RelicSeed { get; set; }
        public virtual string BaseRecord { get; set; }
        public virtual string PrefixRecord { get; set; }
        public virtual string SuffixRecord { get; set; }
        public virtual string ModifierRecord { get; set; }
        public virtual string MateriaRecord { get; set; }
        public virtual string RelicCompletionRecord { get; set; }

        public virtual UInt16? StackCount { get; set; }
        public virtual UInt16 MateriaCombines { get; set; }

        public virtual string Mod { get; set; }
        public virtual bool? IsHardcore { get; set; }
    }
}
