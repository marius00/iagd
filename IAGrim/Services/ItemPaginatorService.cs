using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Services {
    class ItemPaginatorService {        
        private readonly int _limit;
        private readonly Comparison<PlayerHeldItem> _comparer;

        private int _skip;
        private List<PlayerHeldItem> _items;


        private int Remaining => Math.Min(_limit, _items.Count - _skip);


        public ItemPaginatorService(int limit) {
            this._limit = limit;

            this._comparer = (a, b) => a?.Name?.CompareTo(b?.Name) ?? 0;

            // A bit retarded, but due to crappy JS code, we need there to be 'whole rows' of items on high resolutions.
            // TODO: improve the JS to avoid this restriction.
            System.Diagnostics.Debug.Assert(limit % 4 == 0);
        }

        private int CompareToMinimumLevel(PlayerHeldItem itemA, PlayerHeldItem itemB) {
            if (itemA != null && itemB != null)
            {
                var order = itemA.MinimumLevel.CompareTo(itemB.MinimumLevel);
                if (order == 0)
                {
                    return itemA.CompareTo(itemB);
                }
                else
                {
                    return order;
                }
            }

            return 0;
        }

        public void Update(List<PlayerHeldItem> items, bool orderByLevel) {
            this._skip = 0;
            this._items = items;
            if (orderByLevel) {
                _items.Sort(CompareToMinimumLevel);
            } else {
                _items.Sort(_comparer);
            }
        }

        public List<PlayerHeldItem> Fetch() {
            var remaining = Remaining;
            var batch = _items.Skip(_skip).Take(remaining);
            this._skip += remaining;
            return batch.ToList();
        }
    }
}
