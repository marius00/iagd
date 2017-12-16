using System.Collections.Generic;
using IAGrim.Services.Dto;

namespace IAGrim.Database.Model {
    public class ItemSkill {
        public string Description { get; set; }
        public long? Level { get; set; }
        public string Name { get; set; }
        public string Record { get; set; }
        public List<DBSTatRow> Stats { get; set; }
        public long? StatsId { get; set; }
        public string Trigger { get; set; }
    }
}