using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Synchronizer {
    class PlayerItemRepo : BasicSynchronizer<PlayerItem>, IPlayerItemDao {
        private readonly IPlayerItemDao repo;
        public PlayerItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this.repo = new PlayerItemDaoImpl(sessionCreator, new DatabaseItemStatDaoImpl(sessionCreator));
            this.BaseRepo = repo;
        }

        public void ClearAllItemStats() {
            ThreadExecuter.Execute(
                () => repo.ClearAllItemStats()
            );
        }

        public void Import(List<PlayerItem> items) {
            ThreadExecuter.Execute(
                () => repo.Import(items)
            );
        }

        public int ClearItemsMarkedForOnlineDeletion() {
            return ThreadExecuter.Execute(
                () => repo.ClearItemsMarkedForOnlineDeletion()
            );
        }

        public void DeleteAll() {
            ThreadExecuter.Execute(
                () => repo.DeleteAll()
            );
        }

        public Dictionary<string, int> GetCountByRecord(string mod) {
            return ThreadExecuter.Execute(
                () => repo.GetCountByRecord(mod)
            );
        }

        public PlayerItem GetByOnlineId(long OID) {
            return ThreadExecuter.Execute(
                () => repo.GetByOnlineId(OID)
            );
        }

        public IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord) {
            return ThreadExecuter.Execute(
                () => repo.GetByRecord(prefixRecord, baseRecord, suffixRecord, materiaRecord)
            );
        }

        public IList<DeletedPlayerItem> GetItemsMarkedForOnlineDeletion() {
            return ThreadExecuter.Execute(
                () => repo.GetItemsMarkedForOnlineDeletion()
            );
        }

        public IList<ModSelection> GetModSelection() {
            return ThreadExecuter.Execute(
                () => repo.GetModSelection()
            );
        }

        public long GetNumUnsynchronizedItems() {
            return ThreadExecuter.Execute(
                () => repo.GetNumUnsynchronizedItems()
            );
        }

        public PlayerItem GetSingleUnsynchronizedItem() {
            return ThreadExecuter.Execute(
                () => repo.GetSingleUnsynchronizedItem()
            );
        }

        public IList<string> ListAllRecords() {
            return ThreadExecuter.Execute(
                () => repo.ListAllRecords()
            );
        }


        public void Remove(PlayerItem obj) {
            ThreadExecuter.Execute(
                () => repo.Remove(obj)
            );
        }

        public bool RequiresStatUpdate() {
            return ThreadExecuter.Execute(
                () => repo.RequiresStatUpdate()
            );
        }

        public void Save(IEnumerable<PlayerItem> items) {
            ThreadExecuter.Execute(
                () => repo.Save(items)
            );
        }

        public void Save(PlayerItem item) {
            ThreadExecuter.Execute(
                () => repo.Save(item)
            );
        }

        public List<PlayerItem> SearchForItems(Search query) {
            return ThreadExecuter.Execute(
                () => repo.SearchForItems(query)
            );
        }

        public void Update(IList<PlayerItem> items, bool clearOnlineId) {
            ThreadExecuter.Execute(
                () => repo.Update(items, clearOnlineId)
            );
        }

        public void UpdateAllItemStats(IList<PlayerItem> items, Action<int> progress) {
            ThreadExecuter.Execute(
                () => repo.UpdateAllItemStats(items, progress)
            );
        }

        public void UpdateHardcoreSettings() {
            ThreadExecuter.Execute(
                () => repo.UpdateHardcoreSettings()
            );
        }
    }
}
