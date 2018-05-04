using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    public class AzureUploadResult {
        public string Partition { get; set; }
        public bool IsClosed { get; set; }
        public List<AzureUploadedItem> Items { get; set; }
    }
}
