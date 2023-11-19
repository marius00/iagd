using IAGrim.Database.Interfaces;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Database.Synchronizer.Core;
using System.Security.Policy;

namespace IAGrim.Database.Synchronizer {
    class ItemReplicaRepo : BasicSynchronizer<ReplicaItem>, IReplicaItemDao {
        private readonly ReplicaItemDaoImpl repo;
        public ItemReplicaRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            repo = new ReplicaItemDaoImpl(sessionCreator, dialect);
            this.BaseRepo = repo;
        }

        public void UpdatePlayerItemId(int uqHash, long playerItemId) {
            ThreadExecuter.Execute(
                () => repo.UpdatePlayerItemId(uqHash, playerItemId)
            );
        }
        public void DeleteAll() {
            ThreadExecuter.Execute(
                () => repo.DeleteAll()
            );
        }
    }
}
