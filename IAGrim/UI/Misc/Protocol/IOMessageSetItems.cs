using IAGrim.UI.Controller.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Misc.Protocol {
    internal class IOMessageSetItems {
        /// <summary>
        /// Replace all items, or append to existing list
        /// </summary>
        public bool ReplaceExistingItems;

        public List<List<JsonItem>> Items { get; set; }

        /// <summary>
        /// The number of items found, total (eg 3000 found, but batch has 64)
        /// </summary>
        public int NumItemsFound { get; set; }
    }
}
