using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using IAGrim.Database.DAO.Util;
using NHibernate.Exceptions;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace IAGrim.Database {
    public class ReplicaItemDaoImpl : BaseDao<ReplicaItem>, IReplicaItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReplicaItemDaoImpl));

        public ReplicaItemDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {

        }


        public void Save(ReplicaItem obj, List<ReplicaItemRow> rows) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var id = session.CreateSQLQuery("INSERT INTO ReplicaItem2 (playeritemid, buddyitemid) VALUES (:player, :buddy) RETURNING id")
                        .SetParameter("player", obj.PlayerItemId)
                        .SetParameter("buddy", obj.BuddyItemId)
                        .UniqueResult<long>();
                    foreach (ReplicaItemRow row in rows) {
                        row.ReplicaItemId = id;
                        session.Save(row);
                    }

                    transaction.Commit();
                }
            }
        }

        public IList<long> GetPlayerItemIds() {
            using (var session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateCriteria<ReplicaItem>()
                        .Add(Restrictions.IsNotNull("PlayerItemId"))
                        .SetProjection(Projections.Property("PlayerItemId"))
                        .SetResultTransformer(new DistinctRootEntityResultTransformer())
                        .List<long>();
                }
            }
        }

        public IList<string> GetBuddyItemIds() {
            using (var session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateCriteria<ReplicaItem>()
                        .SetProjection(Projections.Property("BuddyItemId"))
                        .Add(Restrictions.IsNotNull("BuddyItemId"))
                        .SetResultTransformer(new DistinctRootEntityResultTransformer())
                        .List<string>();
                }
            }
        }

        /// <summary>
        /// Used primarily to reset replica stats when switching between 1.9, 2.0 and possibly the future expansion
        /// A more graceful solution would be to tag the items as outdated and re-fetch data..but should be a fairly rare occurrence that this is needed.
        /// </summary>
        public void DeleteAll() {
            Logger.Info("Deleting all replica data");
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    int numRowsAffected = session.CreateSQLQuery("DELETE FROM ReplicaItem2")
                        .ExecuteUpdate();

                    session.CreateSQLQuery("DELETE FROM ReplicaItemRow")
                        .ExecuteUpdate();

                    transaction.Commit();
                }

            }
        }
    }
}
