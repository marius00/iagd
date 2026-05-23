using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate;

namespace IAGrim.Database.Migrations {
    /// <summary>
    /// Fixes DatabaseItem_v2.id_databaseitem column type from LONG to INTEGER so that
    /// Microsoft.Data.Sqlite treats it as a rowid alias (autoincrement).
    /// Since this table is repopulated by parsing, if rebuild fails we drop and recreate.
    /// </summary>
    class FixDatabaseItemIdTypeMigration : IDatabaseMigration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FixDatabaseItemIdTypeMigration));

        private const string CorrectCreateTable =
            "CREATE TABLE DatabaseItem_v2 (id_databaseitem INTEGER not null, baserecord TEXT unique, name TEXT, hash INTEGER, namelowercase TEXT, primary key (id_databaseitem))";

        private static readonly List<string> Indices = new List<string> {
            "CREATE INDEX idx_databaseitemv2_record on DatabaseItem_v2 (baserecord)",
            "CREATE INDEX idx_databaseitemv2_baserecord on DatabaseItem_v2 (baserecord)",
        };

        private static readonly HashSet<string> NewTableColumns = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) {
            "id_databaseitem", "baserecord", "name", "hash", "namelowercase"
        };

        public override void Migrate(SessionFactory sessionCreator) {
            if (!TableExists(sessionCreator, "DatabaseItem_v2")) return;
            if (!NeedsRebuild(sessionCreator)) return;

            Logger.Info("DatabaseItem_v2.id_databaseitem column type is not INTEGER — rebuilding table");

            var existingColumns = GetColumnNames(sessionCreator, "DatabaseItem_v2");
            var columnsToCopy = existingColumns.Where(c => NewTableColumns.Contains(c)).ToList();
            var columnList = string.Join(", ", columnsToCopy);

            try {
                using ISession session = sessionCreator.OpenSession();
                
                // Disable foreign key checks — DatabaseItemStat_v2 references this table
                session.CreateSQLQuery("PRAGMA foreign_keys = OFF").ExecuteUpdate();
                
                using ITransaction transaction = session.BeginTransaction();

                session.CreateSQLQuery(CorrectCreateTable.Replace("DatabaseItem_v2", "DatabaseItem_v2_new")).ExecuteUpdate();

                session.CreateSQLQuery($"INSERT INTO DatabaseItem_v2_new ({columnList}) SELECT {columnList} FROM DatabaseItem_v2")
                    .ExecuteUpdate();

                var oldCount = session.CreateSQLQuery("SELECT COUNT(*) FROM DatabaseItem_v2").UniqueResult<long>();
                var newCount = session.CreateSQLQuery("SELECT COUNT(*) FROM DatabaseItem_v2_new").UniqueResult<long>();

                if (oldCount != newCount) {
                    Logger.Warn($"Row count mismatch: old={oldCount}, new={newCount}. Dropping and recreating empty.");
                    session.CreateSQLQuery("DROP TABLE IF EXISTS DatabaseItem_v2_new").ExecuteUpdate();
                    session.CreateSQLQuery("DROP TABLE DatabaseItem_v2").ExecuteUpdate();
                    session.CreateSQLQuery(CorrectCreateTable).ExecuteUpdate();
                    transaction.Commit();
                    session.CreateSQLQuery("PRAGMA foreign_keys = ON").ExecuteUpdate();
                    RecreateIndices(sessionCreator);
                    return;
                }

                session.CreateSQLQuery("DROP TABLE DatabaseItem_v2").ExecuteUpdate();
                session.CreateSQLQuery("ALTER TABLE DatabaseItem_v2_new RENAME TO DatabaseItem_v2").ExecuteUpdate();

                transaction.Commit();
                session.CreateSQLQuery("PRAGMA foreign_keys = ON").ExecuteUpdate();
                Logger.Info($"DatabaseItem_v2 table rebuilt successfully with {newCount} rows");
            } catch (System.Exception ex) {
                Logger.Warn("Failed to rebuild DatabaseItem_v2 with data, dropping and recreating empty.", ex);
                using ISession session = sessionCreator.OpenSession();
                session.CreateSQLQuery("PRAGMA foreign_keys = OFF").ExecuteUpdate();
                using ITransaction transaction = session.BeginTransaction();
                session.CreateSQLQuery("DROP TABLE IF EXISTS DatabaseItem_v2_new").ExecuteUpdate();
                session.CreateSQLQuery("DROP TABLE IF EXISTS DatabaseItem_v2").ExecuteUpdate();
                session.CreateSQLQuery(CorrectCreateTable).ExecuteUpdate();
                transaction.Commit();
                session.CreateSQLQuery("PRAGMA foreign_keys = ON").ExecuteUpdate();
            }

            RecreateIndices(sessionCreator);
        }

        private void RecreateIndices(SessionFactory sessionCreator) {
            foreach (var index in Indices) {
                var indexName = index.Split(" ")[2];
                if (IndexExists(sessionCreator, indexName)) continue;

                using ISession session = sessionCreator.OpenSession();
                using ITransaction tx = session.BeginTransaction();
                session.CreateSQLQuery(index).ExecuteUpdate();
                tx.Commit();
            }
        }

        private static bool NeedsRebuild(SessionFactory sessionCreator) {
            using ISession session = sessionCreator.OpenSession();
            foreach (var row in session.CreateSQLQuery("PRAGMA table_info('DatabaseItem_v2')").List()) {
                var arr = (object[])row;
                var name = (string)arr[1];
                var type = (string)arr[2];

                if (name.Equals("id_databaseitem", System.StringComparison.OrdinalIgnoreCase)) {
                    return !type.Equals("INTEGER", System.StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }

        private static List<string> GetColumnNames(SessionFactory sessionCreator, string table) {
            var columns = new List<string>();
            using ISession session = sessionCreator.OpenSession();
            foreach (var row in session.CreateSQLQuery($"PRAGMA table_info('{table}')").List()) {
                var arr = (object[])row;
                columns.Add((string)arr[1]);
            }
            return columns;
        }
    }
}
