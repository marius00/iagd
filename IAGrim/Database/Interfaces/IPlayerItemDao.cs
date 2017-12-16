using System;
using System.Collections.Generic;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Interfaces {
    public interface IPlayerItemDao : IBaseDao<PlayerItem> {
        IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord);

        Dictionary<string, int> GetCountByRecord(string mod);

        PlayerItem GetByOnlineId(long onlineId);
        PlayerItem GetSingleUnsynchronizedItem();
        long GetNumUnsynchronizedItems();
        void UpdateHardcoreSettings();
        void UpdateAllItemStats(IList<PlayerItem> items, Action<int> progress);
        void DeleteAll();

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
        List<PlayerItem> SearchForItems(Search query);
        void ClearAllItemStats();

        IList<ModSelection> GetModSelection();
    }
}