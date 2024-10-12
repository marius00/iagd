using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database;
using IAGrim.UI.Controller.dto;
using NHibernate.Util;

namespace IAGrim.Services {
    class ItemPaginationService {
        private readonly int _limit;
        private readonly Comparison<List<PlayerHeldItem>> _comparer;

        private int _skip;
        private List<List<PlayerHeldItem>> _items = new List<List<PlayerHeldItem>>();

        public int NumItems => _items.Count;

        private int Remaining {
            get {
                return Math.Min(_limit, NumItems - _skip);
            }
        }


        public ItemPaginationService(int limit) {
            this._limit = limit;


            this._comparer = (a, b) => a?[0].Name?.CompareTo(b?[0]?.Name) ?? 0;

            // A bit retarded, but due to crappy JS code, we need there to be 'whole rows' of items on high resolutions.
            // TODO: improve the JS to avoid this restriction.
            System.Diagnostics.Debug.Assert(limit % 4 == 0);
        }

        private int CompareToMinimumLevel(List<PlayerHeldItem> itemA, List<PlayerHeldItem> itemB) {
            if (itemA != null && itemB != null) {
                var order = itemA[0].MinimumLevel.CompareTo(itemB[0].MinimumLevel);
                if (order == 0) {
                    return itemA[0].CompareTo(itemB[0]);
                }
                else {
                    return order;
                }
            }

            return 0;
        }

        private bool Compare(PlayerHeldItem a, PlayerHeldItem b) {
            if (a is PlayerItem pi1) {
                if (b is PlayerItem pi2) {
                    return pi1.BaseRecord == pi2.BaseRecord
                           && pi1.PrefixRecord == pi2.PrefixRecord
                           && pi1.Seed == pi2.Seed
                           && pi1.SuffixRecord == pi2.SuffixRecord;
                }
            } else if (a is BuddyItem bi1) {
                if (b is BuddyItem bi2) {
                    return bi1.BaseRecord == bi2.BaseRecord
                           && bi1.PrefixRecord == bi2.PrefixRecord
                           && bi1.Seed == bi2.Seed
                           && bi1.SuffixRecord == bi2.SuffixRecord;

                }
            }

            return false;
        }


        public bool Update(List<List<PlayerHeldItem>> items, bool orderByLevel) {
            this._skip = 0;
            this._items = items;
            _items.Sort(orderByLevel ? CompareToMinimumLevel : _comparer);
            return true;
        }

        public List<List<PlayerHeldItem>> Fetch() {
            var remaining = Remaining;
            var batch = _items?.Skip(_skip).Take(remaining);
            this._skip += remaining;
            return batch?.ToList() ?? new List<List<PlayerHeldItem>>();
        }
    }
}