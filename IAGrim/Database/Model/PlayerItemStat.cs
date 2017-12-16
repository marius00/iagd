using DataAccess;
using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    public class PlayerItemStat : IItemStatI {
        public virtual long Id { get; set; } // Redundant, but beats having a string pkey (id:stat)
        public virtual long PlayerItemId { get; set; }
        public virtual string Stat { get; set; }
        public virtual float Value { get; set; }
        public virtual string TextValue { get; set; }

        public virtual string Record {
            get {
                return string.Empty;
            }
        }
    }
}
