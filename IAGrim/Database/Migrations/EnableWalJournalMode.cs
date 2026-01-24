using IAGrim.Database.DAO.Table;
using NHibernate;

namespace IAGrim.Database.Migrations {
    class EnableWalJournalMode : IDatabaseMigration {
        public override void Migrate(SessionFactory sessionCreator) {
            using (ISession session = sessionCreator.OpenSession()) {
                session.CreateSQLQuery($"PRAGMA journal_mode = WAL;").ExecuteUpdate();
            }
        }
    }
}
