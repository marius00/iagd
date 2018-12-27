using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Table;
using log4net;
using NHibernate;

namespace IAGrim.Database.Migrations {
    class AddTimestampToPlayerItemsTable : IDatabaseMigration {

        /// <summary>
        /// When the hash field got introduced, some users ended up with a NULL value, causing issues when later on fetching using "record || hash"
        /// </summary>
        /// <param name="sessionCreator"></param>
        public void Migrate(ISessionCreator sessionCreator) {
            try {

                using (ISession session = sessionCreator.OpenSession()) {
                    using (ITransaction transaction = session.BeginTransaction()) {
                        session.CreateSQLQuery($"ALTER TABLE {PlayerItemTable.Table} ADD COLUMN CreatedAt BIGINT").ExecuteUpdate();
                        session.CreateSQLQuery($"UPDATE {PlayerItemTable.Table} SET CreatedAt = 0").ExecuteUpdate();

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex) {
                // Probably okay, without version control of the db, nothing to do.
            }
        }
    }
}