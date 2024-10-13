using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Dto;
using IAGrim.Database.Synchronizer.Core;

namespace IAGrim.Database.Synchronizer {
    class PlayerItemRepo : BasicSynchronizer<PlayerItem>, IPlayerItemDao {
        private readonly IPlayerItemDao _repo;

        public PlayerItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            this._repo = new PlayerItemDaoImpl(sessionCreator, new DatabaseItemStatDaoImpl(sessionCreator, dialect), dialect);
            this.BaseRepo = _repo;
        }

        public void Import(List<PlayerItem> items) {
            ThreadExecuter.Execute(
                () => _repo.Import(items)
            );
        }

        public IList<string> GetOnlineIds() {
            return ThreadExecuter.Execute(
                () => _repo.GetOnlineIds()
            );
        }

        public void ClearItemsMarkedForOnlineDeletion() {
            ThreadExecuter.Execute(
                () => _repo.ClearItemsMarkedForOnlineDeletion()
            );
        }

        public void ResetOnlineSyncState() {
            ThreadExecuter.Execute(
                () => _repo.ResetOnlineSyncState()
            );
        }

        public void Delete(List<DeleteItemDto> items) {
            ThreadExecuter.Execute(
                () => _repo.Delete(items)
            );
        }

        public IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord, string mod, bool isHardcore) {
            return ThreadExecuter.Execute(
                () => _repo.GetByRecord(prefixRecord, baseRecord, suffixRecord, materiaRecord, mod, isHardcore)
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

        public void DeleteDuplicates() {
            ThreadExecuter.Execute(
                () => _repo.DeleteDuplicates()
            );
        }

        public IList<PlayerItem> ListMissingReplica() {
            return ThreadExecuter.Execute(
                () => _repo.ListMissingReplica()
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

        public Dictionary<long, string> FindRecordsFromIds(IEnumerable<long> ids) {
            return ThreadExecuter.Execute(
                () => _repo.FindRecordsFromIds(ids)
            );
        }
    }
}