using System.Collections.Generic;
using IAGrim.Services.Dto;

namespace IAGrim.Database {
    public class ItemSkill {
        public virtual long Id { get; set; }
        public virtual string Description { get; set; }
        public virtual long? Level { get; set; }
        public virtual string Name { get; set; }
        public virtual string Record { get; set; }
        public virtual List<DBStatRow> Stats { get; set; }
        public virtual long? StatsId { get; set; }
        public virtual string Trigger { get; set; }
    }
}