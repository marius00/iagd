using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database;

namespace IAGrim.Parsers.GameDataParsing.Model {
    class ItemTagAccumulator {
        public Dictionary<string, string> MappedTags { get; }= new Dictionary<string, string>();

        public List<ItemTag> Tags => MappedTags.Select(kv => new ItemTag {
            Name = kv.Value,
            Tag = kv.Key
        }).ToList();

        public void Add(string tag, string name) {
            MappedTags[tag] = name;
        }

    }
}
