using IAGrim.Database.Interfaces;
using NHibernate;


namespace IAGrim.Database {
    /// <summary>
    /// Database class for handling internal Grim Dawn items
    /// These are not user owned items
    /// </summary>
    public class BuddySubscriptionDaoImpl : BaseDao<BuddySubscription>, IBuddySubscriptionDao {

        public BuddySubscriptionDaoImpl(SessionFactory sessionCreator) : base(sessionCreator) {
        }


        /// <summary>
        /// List all items cached
        /// </summary>
        /// <returns></returns>
        public override IList<BuddySubscription> ListAll() {
            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateCriteria<BuddySubscription>().List<BuddySubscription>();
            }
        }
    }
}
