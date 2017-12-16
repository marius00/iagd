using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Persister.Entity;

namespace IAGrim.Database {
    /// <summary>
    /// Database class for handling internal Grim Dawn items
    /// These are not user owned items
    /// </summary>
    public class BuddySubscriptionDaoImpl : BaseDao<BuddySubscription>, IBuddySubscriptionDao {
        private static ILog logger = LogManager.GetLogger(typeof(BuddySubscriptionDaoImpl));

        public BuddySubscriptionDaoImpl(ISessionCreator sessionCreator) : base(sessionCreator) {
        }


        /// <summary>
        /// List all items cached
        /// </summary>
        /// <returns></returns>
        public override IList<BuddySubscription> ListAll() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateCriteria<BuddySubscription>().List<BuddySubscription>();
                }
            }
        }
    }
}
