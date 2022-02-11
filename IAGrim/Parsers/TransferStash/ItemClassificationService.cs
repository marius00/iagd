using System.Collections.Generic;
using System.Linq;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parser.Stash;
using IAGrim.Parsers.Arz;

namespace IAGrim.Parsers.TransferStash {
    public class ItemClassificationService {
        private readonly TransferStashServiceCache _cache;
        private readonly IPlayerItemDao _playerItemDao;

        public ItemClassificationService(TransferStashServiceCache cache, IPlayerItemDao playerItemDao) {
            _cache = cache;
            _playerItemDao = playerItemDao;
        }

        public List<Item> Duplicates { get; } = new List<Item>();
        public List<Item> Unknown { get; } = new List<Item>();
        public List<Item> Quest { get; } = new List<Item>();
        public List<Item> Stacked { get; } = new List<Item>();
        public List<Item> Remaining { get; } = new List<Item>();

        private List<Item> All { get; } = new List<Item>();

        public void Add(List<Item> items) {
            items.ForEach(Add);
        }


        public void Add(PlayerItem item) {
            Add(TransferStashService.Map(item));
        }

        public void Add(Item item) {
            // Dupe-check list

            bool stacked = item.StackCount > 1
                || _cache.StackableRecords.Contains(item.BaseRecord)
                || _cache.SpecialRecords.Contains(item.BaseRecord); // Special "single seed" items.

            if (stacked) {
                Stacked.Add(item);
                All.Add(item);
                return;
            }

            // TODO: Detect slith rings etc

            
            // We don't have this record at all, unknown to IA. Probably need to parse DB.
            bool unknownItem = !_cache.AllRecords.Contains(item.BaseRecord);
            if (unknownItem) {
                if (item.BaseRecord.StartsWith("records/storyelements/rewards/")) {
                    Quest.Add(item);
                }
                else {
                    Unknown.Add(item);
                }

                All.Add(item);
                return;
            }

            // We already have this item..
            if (All.Any(m => m.Equals(item))) {
                Duplicates.Add(item);
                All.Add(item);
                return;
            }

            // We already have this item..
            if (_playerItemDao.Exists(TransferStashService.Map(item, null, false))) {
                Duplicates.Add(item);
                All.Add(item);
                return;
            }
         
            Remaining.Add(item);
            All.Add(item);
        }
    }
}
