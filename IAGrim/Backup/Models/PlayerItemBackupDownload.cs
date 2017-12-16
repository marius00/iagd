using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Models {

#pragma warning disable 0649
    class PlayerItemBackupDownload {

        public IList<PlayerItemBackupDownloadItem> Items;
        public IList<long> Deleted;
        public bool Success;
        public int ErrorCode;
    }
#pragma warning restore 0649



    class PlayerItemBackupDownloadItem {
        public virtual long OnlineId { get; set; }
        public virtual long ModifiedDate { get; set; }

        public virtual long Seed { get; set; }
        public virtual long RelicSeed { get; set; }
        public virtual string BaseRecord { get; set; }
        public virtual string PrefixRecord { get; set; }
        public virtual string SuffixRecord { get; set; }
        public virtual string ModifierRecord { get; set; }
        public virtual string MateriaRecord { get; set; }
        public virtual string RelicCompletionRecord { get; set; }

        public virtual int StackCount { get; set; }
        public virtual int MateriaCombines { get; set; }

        public virtual string Mod { get; set; }
        public virtual bool IsHardcore { get; set; }
    }
}
