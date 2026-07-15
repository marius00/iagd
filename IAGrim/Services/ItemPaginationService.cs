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
        private bool _orderByLevel;
        private List<List<PlayerHeldItem>> _items = new List<List<PlayerHeldItem>>();

        /// <summary>
        /// Total item count after aggregation
        /// </summary>
        public int NumItems => _items.Count;

        /// <summary>
        /// Total item count before aggregation, across every DB page (not just what's currently buffered).
        /// </summary>
        public int NumTotalItems { get; private set; }

        private int Remaining {
            get {
                return Math.Min(_limit, NumItems - _skip);
            }
        }

        /// <summary>
        /// True once every buffered (in-memory) item has been served to the UI. When this is true and
        /// the DB still has further pages, the caller should fetch and Append the next DB batch.
        /// </summary>
        public bool BufferExhausted => _skip >= NumItems;


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


        public bool Update(List<List<PlayerHeldItem>> items, bool orderByLevel, int numTotalItems) {
            this._skip = 0;
            this._orderByLevel = orderByLevel;
            this._items = items;
            _items.Sort(orderByLevel ? CompareToMinimumLevel : _comparer);
            this.NumTotalItems = numTotalItems;
            return true;
        }

        /// <summary>
        /// Append a freshly fetched DB page to the buffer without disturbing how far the UI has already
        /// scrolled (_skip). Only the not-yet-served tail ([_skip..end)) is (re)sorted, so items the UI
        /// has already rendered are never reordered away (which would otherwise skip/duplicate rows if
        /// the SQLite and .NET orderings disagree). MergeStackSize does not preserve order, hence the sort.
        /// </summary>
        public void Append(List<List<PlayerHeldItem>> items) {
            this._items.AddRange(items);
            var tailStart = Math.Min(_skip, _items.Count);
            _items.Sort(tailStart, _items.Count - tailStart, Comparer<List<PlayerHeldItem>>.Create(_orderByLevel ? CompareToMinimumLevel : _comparer));
        }

        public List<List<PlayerHeldItem>> Fetch() {
            var remaining = Remaining;
            var batch = _items?.Skip(_skip).Take(remaining);
            this._skip += remaining;
            return batch?.ToList() ?? new List<List<PlayerHeldItem>>();
        }
    }
}