using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database;

namespace IAGrim.Services {
    class ItemPaginationService {
        private readonly int _limit;
        private readonly Comparison<PlayerHeldItem> _comparer;

        private int _skip;
        private List<PlayerHeldItem> _items = new List<PlayerHeldItem>();

        public int NumItems => _items.Count;

        private int Remaining {
            get {
                if (_limit >= NumItems - _skip) {
                    return Math.Min(_limit, NumItems - _skip);
                } else {
                    int takeUntil = Math.Min(_limit, NumItems - _skip);

                    // If the next base record is the same, keep taking more items to prevent splitting an item "stack"
                    while (takeUntil < NumItems - _skip - 1 && _items[takeUntil].BaseRecord == _items[takeUntil+1].BaseRecord) {
                        takeUntil++;
                    }

                    return takeUntil;
                }
            }
        }


        public ItemPaginationService(int limit) {
            this._limit = limit;

            this._comparer = (a, b) => a?.Name?.CompareTo(b?.Name) ?? 0;

            // A bit retarded, but due to crappy JS code, we need there to be 'whole rows' of items on high resolutions.
            // TODO: improve the JS to avoid this restriction.
            System.Diagnostics.Debug.Assert(limit % 4 == 0);
        }

        private int CompareToMinimumLevel(PlayerHeldItem itemA, PlayerHeldItem itemB) {
            if (itemA != null && itemB != null) {
                var order = itemA.MinimumLevel.CompareTo(itemB.MinimumLevel);
                if (order == 0) {
                    return itemA.CompareTo(itemB);
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


        public bool Update(List<PlayerHeldItem> items, bool orderByLevel) {
            // TODO: Figure out why this is causing issues for some users, suspect it may be that the setItems is done before the view is rendered, thus ending up without any view at all.
            //if (!IsIdenticalToExistingList(items)) {
                this._skip = 0;
                this._items = items;
                _items.Sort(orderByLevel ? CompareToMinimumLevel : _comparer);
                return true;
            //}

            //return false;
        }

        public List<PlayerHeldItem> Fetch() {
            var remaining = Remaining;
            var batch = _items?.Skip(_skip).Take(remaining);
            this._skip += remaining;
            return batch?.ToList() ?? new List<PlayerHeldItem>();
        }
    }
}