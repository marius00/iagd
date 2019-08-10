using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    public class AzureItem : CommonItem {
        public string Id { get; set; }
        public string Partition { get; set; }

    }
}
