using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    class AzureItemPartitionDto {
        /// <summary>
        /// If this is an active or archived partition
        /// Archived partitions may be edited server-side, but no further action is required by the client.
        /// 
        /// If an item is deleted from an archived partition, it will be included as a deletion entry in the currently active partition.
        /// </summary>
        public bool IsActive { get; set; }

        public string Partition { get; set; }
    }
}
