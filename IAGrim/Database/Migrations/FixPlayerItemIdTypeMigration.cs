using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate;

namespace IAGrim.Database.Migrations {
    /// <summary>
    /// Fixes PlayerItem.Id column type from LONG to INTEGER so that
    /// Microsoft.Data.Sqlite treats it as a rowid alias (autoincrement).
    /// Performs a full table rebuild if the type is wrong.
    /// </summary>
    class FixPlayerItemIdTypeMigration : IDatabaseMigration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FixPlayerItemIdTypeMigration));

        private const string CorrectCreateTable = """
            CREATE TABLE "PlayerItem" (
                "Id" INTEGER NOT NULL,
                "baserecord" TEXT,
                "PrefixRecord" TEXT,
                "SuffixRecord" TEXT,
                "ModifierRecord" TEXT,
                "TransmuteRecord" TEXT,
                "Seed" INTEGER,
                "MateriaRecord" TEXT,
                "RelicCompletionBonusRecord" NUMERIC,
                "RelicSeed" INTEGER,
                "EnchantmentRecord" TEXT,
                "PrefixRarity" INTEGER,
                "UNKNOWN" INTEGER,
                "EnchantmentSeed" INTEGER,
                "MateriaCombines" INTEGER,
                "StackCount" INTEGER,
                "Name" TEXT,
                "namelowercase" TEXT,
                "Rarity" TEXT,
                "LevelRequirement" REAL,
                "Mod" TEXT,
                "IsHardcore" INTEGER,
                "cloudid" TEXT,
                "cloud_hassync" INTEGER,
                "created_at" INTEGER,
                "AscendantAffixNameRecord" TEXT,
                "AscendantAffix2hNameRecord" TEXT,
                "RerollsUsed" INTEGER,
                PRIMARY KEY("Id")
            )
            """;

        private static readonly List<string> PlayerItemIndices = new List<string> {
            "CREATE INDEX idx_playeritem_baserecord on PlayerItem (baserecord)",
            "CREATE INDEX idx_playeritem_levelreq on PlayerItem (LevelRequirement)",
            "CREATE INDEX idx_playeritem_lowercasename on PlayerItem (namelowercase)",
            "CREATE INDEX idx_playeritem_prefix on PlayerItem (PrefixRecord)",
            "CREATE INDEX idx_playeritem_rarity on PlayerItem (Rarity)",
            "CREATE INDEX idx_playeritem_suffix on PlayerItem (SuffixRecord)",
        };

        // All columns in the new table (lowercase for comparison)
        private static readonly HashSet<string> NewTableColumns = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) {
            "Id", "baserecord", "PrefixRecord", "SuffixRecord", "ModifierRecord",
            "TransmuteRecord", "Seed", "MateriaRecord", "RelicCompletionBonusRecord",
            "RelicSeed", "EnchantmentRecord", "PrefixRarity", "UNKNOWN",
            "EnchantmentSeed", "MateriaCombines", "StackCount", "Name",
            "namelowercase", "Rarity", "LevelRequirement", "Mod", "IsHardcore",
            "cloudid", "cloud_hassync", "created_at",
            "AscendantAffixNameRecord", "AscendantAffix2hNameRecord", "RerollsUsed"
        };

        public override void Migrate(SessionFactory sessionCreator) {
            if (!TableExists(sessionCreator, "PlayerItem")) return;

            if (!NeedsRebuild(sessionCreator)) return;

            Logger.Info("PlayerItem.Id column type is not INTEGER — rebuilding table");

            // Get existing columns so we only copy what exists in both old and new
            var existingColumns = GetColumnNames(sessionCreator, "PlayerItem");
            var columnsToCopy = existingColumns.Where(c => NewTableColumns.Contains(c)).ToList();
            var columnList = string.Join(", ", columnsToCopy.Select(c => $"\"{c}\""));

            using ISession session = sessionCreator.OpenSession();
            using ITransaction transaction = session.BeginTransaction();

            // Create new table
            session.CreateSQLQuery(CorrectCreateTable.Replace("\"PlayerItem\"", "\"PlayerItem_new\"")).ExecuteUpdate();

            // Copy data
            session.CreateSQLQuery($"INSERT INTO \"PlayerItem_new\" ({columnList}) SELECT {columnList} FROM \"PlayerItem\"")
                .ExecuteUpdate();

            // Verify row counts
            var oldCount = session.CreateSQLQuery("SELECT COUNT(*) FROM \"PlayerItem\"").UniqueResult<long>();
            var newCount = session.CreateSQLQuery("SELECT COUNT(*) FROM \"PlayerItem_new\"").UniqueResult<long>();

            if (oldCount != newCount) {
                Logger.Error($"Row count mismatch during PlayerItem rebuild: old={oldCount}, new={newCount}. Aborting.");
                transaction.Rollback();
                return;
            }

            // Swap tables
            session.CreateSQLQuery("DROP TABLE \"PlayerItem\"").ExecuteUpdate();
            session.CreateSQLQuery("ALTER TABLE \"PlayerItem_new\" RENAME TO \"PlayerItem\"").ExecuteUpdate();

            transaction.Commit();

            // Recreate indexes (outside the main transaction)
            foreach (var index in PlayerItemIndices) {
                var indexName = index.Split(" ")[2];
                if (IndexExists(sessionCreator, indexName)) continue;

                using ISession idxSession = sessionCreator.OpenSession();
                using ITransaction idxTx = idxSession.BeginTransaction();
                idxSession.CreateSQLQuery(index).ExecuteUpdate();
                idxTx.Commit();
            }

            Logger.Info($"PlayerItem table rebuilt successfully with {newCount} rows");
        }

        private static bool NeedsRebuild(SessionFactory sessionCreator) {
            using ISession session = sessionCreator.OpenSession();
            foreach (var row in session.CreateSQLQuery("PRAGMA table_info('PlayerItem')").List()) {
                var arr = (object[])row;
                var name = (string)arr[1];
                var type = (string)arr[2];

                if (name.Equals("Id", System.StringComparison.OrdinalIgnoreCase)) {
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


