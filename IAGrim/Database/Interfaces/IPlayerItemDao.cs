using System;
using System.Collections.Generic;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Interfaces {
    public interface IPlayerItemDao : IBaseDao<PlayerItem> {
        IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord, string mod, bool isHardcore);

        IList<PlayerItem> GetUnsynchronizedItems();
        void SetAsSynchronized(IList<PlayerItem> items);


        void UpdateAllItemStats(IList<PlayerItem> items, Action<int> progress);
        void Delete(List<DeleteItemDto> items);

        /// <summary>
        ///     Does an additional UNIQUE check on OnlineID
        /// </summary>
        /// <param name="items"></param>
        void Import(List<PlayerItem> items);

        IList<DeletedPlayerItem> GetItemsMarkedForOnlineDeletion();
        IList<string> GetOnlineIds();
        void ClearItemsMarkedForOnlineDeletion();
        void ResetOnlineSyncState();

        void Update(IList<PlayerItem> items, bool clearOnlineId);

        bool RequiresStatUpdate();
        IList<string> ListAllRecords();
        List<PlayerItem> SearchForItems(ItemSearchRequest query);

        IList<ModSelection> GetModSelection();
        Dictionary<long, string> FindRecordsFromIds(IEnumerable<long> ids);

        bool Exists(PlayerItem item);


        IList<PlayerItem> ListMissingReplica();
    }
}