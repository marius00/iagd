using IAGrim.Database.Interfaces;
using System.Collections.Generic;
using IAGrim.Database.Model;

namespace IAGrim.Database.Synchronizer {
    public class ItemCollectionRepo : BasicSynchronizer<CollectionItem>, IItemCollectionDao {
        private readonly ItemCollectionDaoImpl _repo;
        public ItemCollectionRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this._repo = new ItemCollectionDaoImpl(sessionCreator);
            this.BaseRepo = null;
        }

        public IList<CollectionItem> GetItemCollection() {
            return ThreadExecuter.Execute(
                () => _repo.GetItemCollection()
            );
        }
    }
}
