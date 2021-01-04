using IAGrim.Database.Interfaces;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Synchronizer.Core;

namespace IAGrim.Database.Synchronizer {
    class BuddySubscriptionRepo : BasicSynchronizer<BuddySubscription>, IBuddySubscriptionDao {
        private readonly IBuddySubscriptionDao repo;
        public BuddySubscriptionRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            repo = new BuddySubscriptionDaoImpl(sessionCreator, dialect);
            this.BaseRepo = repo;
        }

    }
}
