using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    public class AzureUploadItem : CommonItem {
        // What differentiates it is the local item id i guess, so multiple items can be uploaded and later mapped

        public string RemoteId { get; set; }

        public string RemotePartition { get; set; }
    }
}
