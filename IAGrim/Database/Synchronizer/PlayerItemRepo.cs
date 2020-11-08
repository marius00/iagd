using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Backup.Azure.Dto;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Synchronizer {
    class PlayerItemRepo : BasicSynchronizer<PlayerItem>, IPlayerItemDao {
        private readonly IPlayerItemDao _repo;
        public PlayerItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this._repo = new PlayerItemDaoImpl(sessionCreator, new DatabaseItemStatDaoImpl(sessionCreator));
            this.BaseRepo = _repo;
        }

        public void Import(List<PlayerItem> items) {
            ThreadExecuter.Execute(
                () => _repo.Import(items)
            );
        }

        public int ClearItemsMarkedForOnlineDeletion() {
            return ThreadExecuter.Execute(
                () => _repo.ClearItemsMarkedForOnlineDeletion()
            );
        }

        public void ResetOnlineSyncState() {
            ThreadExecuter.Execute(
                () => _repo.ResetOnlineSyncState()
            );
        }

        public void Delete(List<AzureItemDeletionDto> items) {
            ThreadExecuter.Execute(
                () => _repo.Delete(items)
            );
        }

        public Dictionary<string, int> GetCountByRecord(string mod) {
            return ThreadExecuter.Execute(
                () => _repo.GetCountByRecord(mod)
            );
        }

        public IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord) {
            return ThreadExecuter.Execute(
                () => _repo.GetByRecord(prefixRecord, baseRecord, suffixRecord, materiaRecord)
            );
        }

        public IList<DeletedPlayerItem> GetItemsMarkedForOnlineDeletion() {
            return ThreadExecuter.Execute(
                () => _repo.GetItemsMarkedForOnlineDeletion()
            );
        }

        public IList<ModSelection> GetModSelection() {
            return ThreadExecuter.Execute(
                () => _repo.GetModSelection()
            );
        }

        public bool Exists(PlayerItem item) {
            return ThreadExecuter.Execute(
                () => _repo.Exists(item)
            );
        }

        public void DeleteDuplidates() {
            ThreadExecuter.Execute(
                () => _repo.DeleteDuplidates()
            );
        }

        public IList<PlayerItem> GetUnsynchronizedItems() {
            return ThreadExecuter.Execute(
                () => _repo.GetUnsynchronizedItems()
            );
        }

        public void SetAsSynchronized(IList<PlayerItem> items) {
            ThreadExecuter.Execute(
                () => _repo.SetAsSynchronized(items)
            );
        }

        public long GetNumItems(string backupPartition) {
            return ThreadExecuter.Execute(
                () => _repo.GetNumItems(backupPartition)
            );
        }

        public IList<string> ListAllRecords() {
            return ThreadExecuter.Execute(
                () => _repo.ListAllRecords()
            );
        }


        public bool RequiresStatUpdate() {
            return ThreadExecuter.Execute(
                () => _repo.RequiresStatUpdate()
            );
        }

        public List<PlayerItem> SearchForItems(ItemSearchRequest query) {
            return ThreadExecuter.Execute(
                () => _repo.SearchForItems(query)
            );
        }

        public void Update(IList<PlayerItem> items, bool clearOnlineId) {
            ThreadExecuter.Execute(
                () => _repo.Update(items, clearOnlineId)
            );
        }

        public void UpdateAllItemStats(IList<PlayerItem> items, Action<int> progress) {
            ThreadExecuter.Execute(
                () => _repo.UpdateAllItemStats(items, progress)
            );
        }
    }
}
