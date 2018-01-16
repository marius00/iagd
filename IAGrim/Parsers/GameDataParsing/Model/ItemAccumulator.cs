using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using IAGrim.Database;
using NHibernate.Util;

namespace IAGrim.Parsers.GameDataParsing.Model {
    class ItemAccumulator {
        private Dictionary<string, InternalItem> _items = new Dictionary<string, InternalItem>();

        public List<DatabaseItem> Items {
            get {
                return _items.Values.Select(m => new DatabaseItem {
                    Record = m.Record,
                    Stats = m.Stats.Values.Select(s => new DatabaseItemStat {
                        Stat = s.Stat,
                        TextValue = s.TextValue,
                        Value = s.Value
                    }).ToList()
                }).ToList();
            }
        }

        class InternalItem {
            public string Record { get; set; }
            public Dictionary<string, IItemStat> Stats { get; } = new Dictionary<string, IItemStat>();
        }

        public void Add(IItem item) {
            var internalItem = _items.ContainsKey(item.Record) ? _items[item.Record] : new InternalItem { Record = item.Record };
            foreach (var stat in item.Stats) {
                internalItem.Stats[stat.Stat] = stat;
            }

            _items[item.Record] = internalItem;
        }
    }
}
