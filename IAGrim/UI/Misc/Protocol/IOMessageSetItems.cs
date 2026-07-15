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

        public List<List<JsonItem>>? Items { get; set; }

        /// <summary>
        /// The number of items found, total (eg 3000 found, but batch has 64)
        /// </summary>
        public int NumItemsFound { get; set; }

        /// <summary>
        /// True when <see cref="NumItemsFound"/> is a floor rather than the exact total: the result was capped
        /// and the (expensive) COUNT was deferred, so the UI shows it as "1000+". Becomes false once the exact
        /// total is computed (when the user paginates past the first page).
        /// </summary>
        public bool NumItemsApproximate { get; set; }

        /// <summary>
        /// Whether further items can still be fetched (more buffered pages, or more DB pages to load).
        /// The frontend uses this to decide whether to keep infinite-scrolling, rather than comparing
        /// counts (NumItemsFound is pre stack-merge, so it can exceed the number of displayed rows).
        /// </summary>
        public bool HasMore { get; set; }
    }
}
