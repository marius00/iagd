using System;
using System.Collections.Generic;
using IAGrim.Backup.Azure.Dto;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Interfaces {
    public interface IPlayerItemDao : IBaseDao<PlayerItem> {
        IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord);

        Dictionary<string, int> GetCountByRecord(string mod);

        IList<PlayerItem> GetUnsynchronizedItems();
        void SetAzureIds(List<AzureUploadedItem> mappings);


        void UpdateAllItemStats(IList<PlayerItem> items, Action<int> progress);
        void Delete(List<AzureItemDeletionDto> items);

        /// <summary>
        ///     Does an additional UNIQUE check on OnlineID
        /// </summary>
        /// <param name="items"></param>
        void Import(List<PlayerItem> items);

        IList<DeletedPlayerItem> GetItemsMarkedForOnlineDeletion();
        int ClearItemsMarkedForOnlineDeletion();

        void Update(IList<PlayerItem> items, bool clearOnlineId);

        bool RequiresStatUpdate();
        IList<string> ListAllRecords();
        List<PlayerItem> SearchForItems(ItemSearchRequest query);

        IList<ModSelection> GetModSelection();

        bool Exists(PlayerItem item);
    }
}