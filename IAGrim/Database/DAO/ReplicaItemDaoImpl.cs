using System;
using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using IAGrim.Database.DAO.Util;
using NHibernate.Exceptions;

namespace IAGrim.Database {
    public class ReplicaItemDaoImpl : BaseDao<ReplicaItem>, IReplicaItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReplicaItemDaoImpl));

        public ReplicaItemDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {

        }


        public override void Save(ReplicaItem obj) {
            using (ISession session = SessionCreator.OpenSession()) {
                try {
                    using (ITransaction transaction = session.BeginTransaction()) {
                        int numRowsAffected = 0;

                        // We may be receiving item info from a MouseOver, prior to the item being looted by IA.
                        if (obj.PlayerItemId != 0) {
                            numRowsAffected = session.CreateSQLQuery("UPDATE ReplicaItem SET playeritemid = :pid, Text = :text WHERE uqhash = :hash")
                                .SetParameter("pid", obj.PlayerItemId)
                                .SetParameter("hash", obj.UqHash)
                                .SetParameter("text", obj.Text)
                                .ExecuteUpdate();
                        }

                        if (numRowsAffected == 0) {
                            session.Save(obj);
                        }

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

        public void UpdatePlayerItemId(int uqHash, long playerItemId) {
            if (playerItemId == 0) {
                throw new ArgumentException("The argument playerItemId cannot be 0");
            }

            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    int numRowsAffected = session.CreateSQLQuery("UPDATE ReplicaItem SET playeritemid = :pid WHERE uqhash = :hash")
                        .SetParameter("pid", playerItemId)
                        .SetParameter("hash", uqHash)
                        .ExecuteUpdate();

                    // Maybe IA was not running when the item was added? Or it's from another player on the same cloud sync.
                    // IA will have to request these stats from GD.
                    if (numRowsAffected == 0) {
                        Logger.Debug($"Could not update Replica for {uqHash} to playerItemId={playerItemId}, does not exist.");
                    }

                    transaction.Commit();
                }

            }
        }
    }
}
