using System;
using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using IAGrim.Database.DAO.Util;
using NHibernate.Exceptions;

namespace IAGrim.Database {
    public class BuddyReplicaItemDaoImpl : BaseDao<BuddyReplicaItem>, IBuddyReplicaItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BuddyReplicaItemDaoImpl));

        public BuddyReplicaItemDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {

        }

        public override void Save(BuddyReplicaItem obj) {
            using (ISession session = SessionCreator.OpenSession()) {
                try {
                    using (ITransaction transaction = session.BeginTransaction()) {
                        session.Save(obj);
                        transaction.Commit();
                    }
                }
                catch (GenericADOException ex) {
                    // If the playerItemId is 0, we're most likely just hovering over armor in-game and getting multiple hits.
                    // Might want some kind of caching service infront of the DB layer, which remembers the last ~5000 hashes this session.
                    Logger.Debug("Error storing replica item", ex);
                }
            }
        }
    }
}
