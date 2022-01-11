using IAGrim.Database.Interfaces;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Database.Synchronizer.Core;

namespace IAGrim.Database.Synchronizer {
    class BuddyItemReplicaRepo : BasicSynchronizer<BuddyReplicaItem>, IBuddyReplicaItemDao {
        private readonly BuddyReplicaItemDaoImpl repo;
        public BuddyItemReplicaRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            repo = new BuddyReplicaItemDaoImpl(sessionCreator, dialect);
            this.BaseRepo = repo;
        }
    }
}
