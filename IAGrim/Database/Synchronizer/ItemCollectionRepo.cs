using IAGrim.Database.Interfaces;
using IAGrim.Database.Dto;
using IAGrim.Database.Model;
using IAGrim.Database.Synchronizer.Core;

namespace IAGrim.Database.Synchronizer {
    public class ItemCollectionRepo : BasicSynchronizer<CollectionItem>, IItemCollectionDao {
        private readonly ItemCollectionDaoImpl _repo;
        public ItemCollectionRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this._repo = new ItemCollectionDaoImpl(sessionCreator);
            this.BaseRepo = null;
        }

        public IList<CollectionItem> GetItemCollection(ItemSearchRequest query) {
            return ThreadExecuter.Execute(
                () => _repo.GetItemCollection(query)
            );
        }

        public IList<CollectionItemAggregateRow> GetItemAggregateStats() {
            return ThreadExecuter.Execute(
                () => _repo.GetItemAggregateStats()
            );
        }
    }
}
