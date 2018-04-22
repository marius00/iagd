using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    class AzureItemContainer {
        public AzureItemPartitionDto Partition { get; set; }
        public List<AzureItem> Items { get; set; }
    }
}
