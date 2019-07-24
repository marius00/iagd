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
        private readonly IPlayerItemDao repo;
        public PlayerItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this.repo = new PlayerItemDaoImpl(sessionCreator, new DatabaseItemStatDaoImpl(sessionCreator));
            this.BaseRepo = repo;
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

        public void Delete(List<AzureItemDeletionDto> items) {
            ThreadExecuter.Execute(
                () => repo.Delete(items)
            );
        }

        public Dictionary<string, int> GetCountByRecord(string mod) {
            return ThreadExecuter.Execute(
                () => repo.GetCountByRecord(mod)
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

        public bool Exists(PlayerItem item) {
            return ThreadExecuter.Execute(
                () => repo.Exists(item)
            );
        }

        public IList<PlayerItem> GetUnsynchronizedItems() {
            return ThreadExecuter.Execute(
                () => repo.GetUnsynchronizedItems()
            );
        }

        public void SetAzureIds(List<AzureUploadedItem> mappings) {
            ThreadExecuter.Execute(
                () => repo.SetAzureIds(mappings)
            );
        }

        public IList<string> ListAllRecords() {
            return ThreadExecuter.Execute(
                () => repo.ListAllRecords()
            );
        }


        public bool RequiresStatUpdate() {
            return ThreadExecuter.Execute(
                () => repo.RequiresStatUpdate()
            );
        }

        public List<PlayerItem> SearchForItems(ItemSearchRequest query) {
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
    }
}
