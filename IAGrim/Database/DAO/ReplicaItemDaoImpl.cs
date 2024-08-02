using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using IAGrim.Database.DAO.Util;
using NHibernate.Exceptions;
using System.Collections.Generic;

namespace IAGrim.Database {
    public class ReplicaItemDaoImpl : BaseDao<ReplicaItem>, IReplicaItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReplicaItemDaoImpl));

        public ReplicaItemDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {

        }


        public void Save(ReplicaItem obj, List<ReplicaItemRow> rows) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.Save(obj);
                    foreach (ReplicaItemRow row in rows) {
                        row.ReplicaItemId = obj.Id;
                        session.Save(row);
                    }

                    transaction.Commit();
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
                    int numRowsAffected = session.CreateSQLQuery("DELETE FROM ReplicaItem")
                        .ExecuteUpdate();

                    transaction.Commit();
                }

            }
        }
    }
}
