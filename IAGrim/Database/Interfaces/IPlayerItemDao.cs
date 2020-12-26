using System;
using System.Collections.Generic;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Interfaces {
    public interface IPlayerItemDao : IBaseDao<PlayerItem> {
        IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord);

        Dictionary<string, int> GetCountByRecord(string mod);

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
        void ClearItemsMarkedForOnlineDeletion();
        void ResetOnlineSyncState();

        void Update(IList<PlayerItem> items, bool clearOnlineId);

        bool RequiresStatUpdate();
        IList<string> ListAllRecords();
        List<PlayerItem> SearchForItems(ItemSearchRequest query);

        IList<ModSelection> GetModSelection();

        bool Exists(PlayerItem item);

        void DeleteDuplicates();

        IList<PlayerItem> ListWithMissingStatCache();
        void UpdateCachedStats(IList<PlayerItem> items);
    }
}