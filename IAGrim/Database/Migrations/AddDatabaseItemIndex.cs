using IAGrim.Database.DAO.Table;
using NHibernate;

namespace IAGrim.Database.Migrations {
    class AddDatabaseItemIndex : IDatabaseMigration {

        /// <summary>
        /// When the hash field got introduced, some users ended up with a NULL value, causing issues when later on fetching using "record || hash"
        /// </summary>
        /// <param name="sessionCreator"></param>
        public override void Migrate(SessionFactory sessionCreator) {
            if (IndexExists(sessionCreator, "idx_databaseitemv2_baserecord")) {
                return;
            }

            using (ISession session = sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery(
                        // todo: if not exists
                        $"CREATE INDEX idx_databaseitemv2_baserecord on DatabaseItem_v2 (baserecord)"
                    ).ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }
    }
}
