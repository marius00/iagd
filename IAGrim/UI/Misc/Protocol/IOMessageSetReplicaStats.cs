using IAGrim.Services.ItemReplica;
using System.Collections.Generic;

namespace IAGrim.UI.Misc.Protocol {
    internal class IOMessageSetReplicaStats {
        public long Id { get; set; }
        public IList<ItemStatInfo> ReplicaStats { get; set; } 
    }
}
