using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Model;

namespace IAGrim.UI.Misc.CEF {
    class GetSetItemAssociationsEventArgs : EventArgs {
        public IList<ItemSetAssociation> Elements { get; set; }
    }
}
