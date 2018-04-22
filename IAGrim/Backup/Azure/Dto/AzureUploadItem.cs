using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Dto {
    class AzureUploadItem : CommonItem {
        // What differentiates it is the local item id i guess, so multiple items can be uploaded and later mapped

        /// <summary>
        /// Id in the local player db
        /// </summary>
        public long LocalId { get; set; }
    }
}
