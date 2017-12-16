using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;

namespace IAGrim.Database.Synchronizer {
    class BuddySubscriptionRepo : BasicSynchronizer<BuddySubscription>, IBuddySubscriptionDao {
        private readonly IBuddySubscriptionDao repo;
        public BuddySubscriptionRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            repo = new BuddySubscriptionDaoImpl(sessionCreator);
            this.BaseRepo = repo;
        }

    }
}
