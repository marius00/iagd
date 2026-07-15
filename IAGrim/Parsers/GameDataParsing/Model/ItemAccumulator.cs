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
        private readonly Dictionary<string, InternalItem> _items = new Dictionary<string, InternalItem>();

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
            List<string> source = new List<string> {item.Record ?? "-"};
            foreach (var stat in item.Stats ?? Enumerable.Empty<DatabaseItemStat>()) {
                source.Add(stat.Stat ?? "-");
                source.Add((stat?.Value ?? 0).ToString(CultureInfo.CurrentCulture));
                source.Add(stat?.TextValue ?? "-");
                source.Add(item.Name ?? "-"); // Useful for re-parsing with language packs
            }

            return string.Join(":", source).GetHashCode();
        }

        class InternalItem {
            public required string Record { get; set; }
            public Dictionary<string, IItemStat> Stats { get; } = new Dictionary<string, IItemStat>();
        }

        public void Add(IItem item) {
            // The engine loads each record path as a whole-record replace (last file that
            // defines the path wins entirely), not a per-field union. Arz files must be
            // processed in ascending priority order (base -> GDX1 -> GDX2 -> ...) so that
            // this replace-on-redefine matches which file "wins" in-game.
            if (item.Record == null) {
                return;
            }

            var internalItem = new InternalItem { Record = item.Record };
            foreach (var stat in item.Stats ?? Enumerable.Empty<IItemStat>()) {
                if (stat.Stat == null) {
                    continue;
                }

                internalItem.Stats[stat.Stat] = stat;
            }

            _items[item.Record] = internalItem;
        }
    }
}
