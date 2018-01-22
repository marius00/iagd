using System;
using System.Collections.Generic;
using System.Globalization;
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
                var items = _items.Values.Select(m => new DatabaseItem {
                    Record = m.Record,
                    Stats = m.Stats.Values.Select(s => new DatabaseItemStat {
                        Stat = s.Stat,
                        TextValue = s.TextValue,
                        Value = s.Value
                    }).ToList()
                }).ToList();

                foreach (var item in items) {
                    item.Hash = CalculateHash(item);
                }

                return items;
            }
        }

        private int CalculateHash(DatabaseItem item) {
           List<string> source = new List<string>();
            source.Add(item.Record);
            foreach (var stat in item.Stats) {
                source.Add(stat.Stat ?? "-");
                source.Add((stat?.Value ?? 0).ToString(CultureInfo.CurrentCulture));
                source.Add(stat.TextValue ?? "-");
            }

            return string.Join(":", source).GetHashCode();
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
