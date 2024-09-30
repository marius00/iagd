using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database {
    public class ReplicaItemRow {
        public virtual long Id { get; set; }
        public virtual long ReplicaItemId { get; set; }
        public virtual long Type { get; set; } = 0;
        public virtual string Text { get; set; } = "";
        public virtual string TextLowercase { get; set; } = "";
    }
}
