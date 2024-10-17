using NHibernate.Mapping;
using System.Collections.Generic;

namespace IAGrim.UI.Misc.Protocol {
    internal class IOMessageCloudIconStateChange {
        public IList<long> Ids { get; set; }
    }
}
