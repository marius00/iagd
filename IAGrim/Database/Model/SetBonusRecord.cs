using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Model {
    public class SetBonusRecord {
        public virtual long Id { get; set; }

        public virtual DatabaseItem Bonuses { get; set; }

        public virtual string Item01 { get; set; }
        public virtual string Item02 { get; set; }
        public virtual string Item03 { get; set; }
        public virtual string Item04 { get; set; }
        public virtual string Item05 { get; set; }
        public virtual string Item06 { get; set; }
    }
}
