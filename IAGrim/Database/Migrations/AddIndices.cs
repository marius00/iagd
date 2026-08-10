using System.Linq;
using log4net;
using NHibernate;

namespace IAGrim.Database.Migrations {
    /// <summary>
    /// Single source of truth for every index the application relies on.
    ///
    /// Must run after HbmSchemaMigration: databases from older versions may be missing columns entirely,
    /// and those columns are only added once the mappings have been reconciled against the schema.
    /// Indexing a column that doesn't exist yet aborts the migration before the repair can happen.
    /// </summary>
    class AddIndices : IDatabaseMigration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AddIndices));

        public static readonly List<string> Indices = new List<string>() {
            "CREATE INDEX idx_databaseitemstatv2_parent_stat on DatabaseItemStat_v2 (id_databaseitem)",
            "CREATE INDEX idx_databaseitemstatv2_stat on DatabaseItemStat_v2 (Stat)",
            "CREATE INDEX idx_databaseitemv2_record on DatabaseItem_v2 (baserecord)",
            "CREATE INDEX idx_playeritem_baserecord on PlayerItem (baserecord)",
            "CREATE INDEX idx_playeritem_levelreq on PlayerItem (LevelRequirement)",
            "CREATE INDEX idx_playeritem_lowercasename on PlayerItem (namelowercase)",
            "CREATE INDEX idx_playeritem_prefix on PlayerItem (PrefixRecord)",
            "CREATE INDEX idx_playeritem_rarity on PlayerItem (Rarity)",
            "CREATE INDEX idx_playeritem_suffix on PlayerItem (SuffixRecord)",

            // The paged item search orders by "PI.name, PI.Id" (see SearchForItems) so LIMIT/OFFSET slices
            // are stable across pages. PlayerItem had no index on 'name', so that ORDER BY forced SQLite to
            // materialize and sort the entire match set before applying LIMIT - on a fully populated
            // collection (100k+ rows) a broad search sorted every matching row just to return the first page.
            // With this index the ordering is satisfied by an index walk, letting LIMIT terminate early
            // instead of sorting everything. The composite (name, Id) matches the ORDER BY key exactly.
            "CREATE INDEX idx_playeritem_name on PlayerItem (name, Id)",

            // The record-based search filters (stat/damage, class, slot) join PlayerItemRecord on its
            // 'record' column. The table's only pre-existing index is the composite primary key
            // (PlayerItemId, Record), whose leftmost column is PlayerItemId, so a join/lookup on 'record'
            // alone could not use it and fell back to a full table scan. On a fully populated collection
            // (hundreds of thousands of rows) that made filtered searches take over a minute.
            "CREATE INDEX idx_playeritemrecord_record on PlayerItemRecord (record)",

            "CREATE INDEX idx_replicaitem_buddyitemid on ReplicaItem2 (buddyitemid)",
            "CREATE INDEX idx_replicaitem_playeritemid on ReplicaItem2 (playeritemid)",
            "CREATE INDEX idx_replicaitemstat_replicaitemid on ReplicaItemRow (replicaitemid)",
            "CREATE INDEX idx_computeditemstat_playeritemid on ComputedItemStat (playeritemid)",
            "CREATE INDEX idx_computeditemstat_stat_value on ComputedItemStat (stat, value)",
        };

        /// <summary>
        /// Indexes that were created by earlier versions and are no longer wanted.
        /// Existing installations keep them until they are explicitly dropped, paying the write cost for nothing.
        /// </summary>
        private static readonly List<string> ObsoleteIndices = new List<string>() {
            // Duplicate of idx_databaseitemv2_record: same table, same column. The name below came from a
            // separate migration, while idx_databaseitemv2_record is the one declared in DatabaseItem.hbm.xml.
            "idx_databaseitemv2_baserecord",
        };

        public override void Migrate(SessionFactory sessionCreator) {
            DropObsolete(sessionCreator);
            CreateMissing(sessionCreator, Indices);
        }

        private static void DropObsolete(SessionFactory sessionCreator) {
            foreach (var name in ObsoleteIndices) {
                if (!IndexExists(sessionCreator, name)) {
                    continue;
                }

                try {
                    using ISession session = sessionCreator.OpenSession();
                    using ITransaction transaction = session.BeginTransaction();
                    session.CreateSQLQuery($"DROP INDEX IF EXISTS {name}").ExecuteUpdate();
                    transaction.Commit();
                    Logger.Info($"Dropped redundant index {name}");
                } catch (Exception ex) {
                    Logger.Warn($"Could not drop redundant index {name}: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// The indexes belonging to a single table, for the migrations that rebuild a table
        /// and need to restore the indexes dropped along with it.
        /// </summary>
        public static IEnumerable<string> For(string table) {
            return Indices.Where(i => i.Split(" ")[4].Equals(table, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates any of the given indexes that do not already exist.
        /// </summary>
        public static void CreateMissing(SessionFactory sessionCreator, IEnumerable<string> indices) {
            foreach (var index in indices) {
                var name = index.Split(" ")[2];

                if (IndexExists(sessionCreator, name)) {
                    continue;
                }

                try {
                    using ISession session = sessionCreator.OpenSession();
                    using ITransaction transaction = session.BeginTransaction();
                    session.CreateSQLQuery(index).ExecuteUpdate();
                    transaction.Commit();
                } catch (Exception ex) {
                    // An index is a performance optimization, never a correctness requirement.
                    // A single one failing must not prevent the remaining indexes or migrations from running.
                    Logger.Warn($"Could not create index {name}, continuing without it: {ex.Message}", ex);
                }
            }
        }
    }
}
