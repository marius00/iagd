using DataAccess;
using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Database {
    public class DatabaseItemStat : IItemStatI {
        public virtual long Id { get; set; }

        public virtual string Stat { get; set; }

        public virtual float Value { get; set; }
        public virtual string TextValue { get; set; }

        public virtual DatabaseItem Parent { get; set; }
    }
}
