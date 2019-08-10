using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    public class ItemDownloadDto {
        public List<AzureItem> Items { get; set; }
        public List<AzureItemDeletionDto> Removed { get; set; }
    }
}
