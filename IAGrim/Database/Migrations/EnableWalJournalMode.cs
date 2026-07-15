using IAGrim.Database.DAO.Table;
using NHibernate;

namespace IAGrim.Database.Migrations {
    class EnableWalJournalMode : IDatabaseMigration {
        public override void Migrate(SessionFactory sessionCreator) {
            using (ISession session = sessionCreator.OpenSession()) {
                var current = session.CreateSQLQuery($"PRAGMA journal_mode;").UniqueResult<string>();
                if (string.Equals(current, "wal", System.StringComparison.OrdinalIgnoreCase)) {
                    return;
                }

                session.CreateSQLQuery($"PRAGMA journal_mode = WAL;").ExecuteUpdate();
            }
        }
    }
}
