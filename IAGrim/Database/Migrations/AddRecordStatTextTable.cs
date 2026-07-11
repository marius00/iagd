using IAGrim.Database.DAO.Table;
using NHibernate;

namespace IAGrim.Database.Migrations {
    /// <summary>
    /// Adds the <see cref="RecordStatTextTable"/> lookup table that backs free-text stat search.
    /// The table is populated/refreshed at runtime by the search-text indexer (game DB parse and
    /// language change), not by this migration.
    /// </summary>
    class AddRecordStatTextTable : IDatabaseMigration {
        public override void Migrate(SessionFactory sessionCreator) {
            if (TableExists(sessionCreator, RecordStatTextTable.Table)) {
                return;
            }

            using ISession session = sessionCreator.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            session.CreateSQLQuery(
                $"CREATE TABLE {RecordStatTextTable.Table} (" +
                $"{RecordStatTextTable.Record} TEXT not null, " +
                $"{RecordStatTextTable.SearchText} TEXT, " +
                $"primary key ({RecordStatTextTable.Record}))"
            ).ExecuteUpdate();

            transaction.Commit();
        }
    }
}
