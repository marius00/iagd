using EvilsoftCommons;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.DAO;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Model;
using IAGrim.Services.Dto;
using log4net;
using Microsoft.Data.Sqlite;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace IAGrim.Database {
    /// <summary>
    /// Database class for handling player owned items
    /// </summary>
    public class PlayerItemDaoImpl : BaseDao<PlayerItem>, IPlayerItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IPlayerItemDao));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;

        public PlayerItemDaoImpl(
            SessionFactory sessionCreator,
            IDatabaseItemStatDao databaseItemStatDao) : base(sessionCreator) {
            _databaseItemStatDao = databaseItemStatDao;
        }

        /// <summary>
        /// List all player items
        /// </summary>
        /// <returns></returns>
        public IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord, string mod, bool isHardcore) {
            using (var session = SessionCreator.OpenSession()) {
                // TODO:
                var crits = session.CreateCriteria<PlayerItem>()
                    .Add(Restrictions.Eq("BaseRecord", baseRecord))
                    .Add(Restrictions.Eq("PrefixRecord", prefixRecord))
                    .Add(Restrictions.Eq("SuffixRecord", suffixRecord))
                    .Add(Restrictions.Eq("MateriaRecord", materiaRecord));

                if (string.IsNullOrEmpty(mod)) {
                    crits = crits.Add(Restrictions.Or(Restrictions.Eq("Mod", ""), Restrictions.IsNull("Mod")));
                }
                else {
                    crits = crits.Add(Restrictions.Eq("Mod", mod));
                }


                if (isHardcore) {
                    crits = crits.Add(Restrictions.Eq("IsHardcore", true));
                }
                else {
                    crits = crits.Add(Restrictions.Not(Restrictions.Eq("IsHardcore", true)));
                }

                return crits.List<PlayerItem>();
            }
        }

        /// <summary>
        /// Get the "level of green" for a set of records
        /// An item with a white suffix and green prefix is "less than" an item with a green suffix and green prefix
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        private int GetGreenQualityLevelForRecords(Dictionary<string, List<DBStatRow>> stats, List<string> records) {
            // Filter out green components
            var filteredRecords = records
                .Where(record => !record.StartsWith("records/items/materia/"))
                .Where(record => record.Contains("/lootaffixes/")) // Ignore the base record
                .ToList();

            var classifications = filteredRecords
                .Where(stats.ContainsKey)
                .SelectMany(record => stats[record].Where(v => v.Stat == "itemClassification"))
                .Select(m => m.TextValue)
                .ToList();


            if (classifications.All(m => m != "Legendary" && m != "Epic")) {
                return classifications.Count(m => m == "Rare");
            }

            return 0;
        }

        /// <summary>
        /// Get all the relative records for an item (including suffixes)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<string> GetRecordsForItem(BaseItem item) {
            var records = new List<string>();

            if (!string.IsNullOrEmpty(item.BaseRecord)) {
                records.Add(item.BaseRecord);
            }

            if (!string.IsNullOrEmpty(item.PrefixRecord)) {
                records.Add(item.PrefixRecord);
            }

            if (!string.IsNullOrEmpty(item.SuffixRecord)) {
                records.Add(item.SuffixRecord);
            }

            if (!string.IsNullOrEmpty(item.MateriaRecord)) {
                records.Add(item.MateriaRecord);
            }

            if (!string.IsNullOrEmpty(item.AscendantAffixNameRecord)) {
                records.Add(item.AscendantAffixNameRecord);
            }

            if (!string.IsNullOrEmpty(item.AscendantAffix2hNameRecord)) {
                records.Add(item.AscendantAffix2hNameRecord);
            }

            return records;
        }

        public IList<PlayerItem> GetUnsynchronizedItems() {
            using (var session = SessionCreator.OpenSession()) {
                return session.CreateCriteria<PlayerItem>()
                    .Add(Restrictions.Or(
                            Restrictions.Eq(nameof(PlayerItem.IsCloudSynchronizedValue), 0L),
                            Restrictions.IsNull(nameof(PlayerItem.IsCloudSynchronizedValue))
                        )
                    )
                    .List<PlayerItem>();
            }
        }

        public void SetAsSynchronized(IList<PlayerItem> items) {
            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        var query = session.CreateSQLQuery($@"UPDATE PlayerItem SET 
                                             {PlayerItemTable.IsCloudSynchronized} = 1, 
                                             {PlayerItemTable.CloudId} = :uuid 
                                            WHERE Id = :id")
                            .SetParameter("id", item.Id)
                            .SetParameter("uuid", item.CloudId);

                        query.ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
        }

        public static IEnumerable<string> GetPetBonusRecords(Dictionary<string, List<DBStatRow>> stats, List<string> records) {
            return records
                .Where(stats.ContainsKey)
                .SelectMany(record => stats[record])
                .Where(m => m.Stat == "petBonusName")
                .Select(m => m.TextValue);
        }

        /// <summary>
        /// List all player items
        /// </summary>
        /// <returns></returns>
        public IList<string> ListAllRecords() {
            using (var session = SessionCreator.OpenSession()) {
                return session.CreateCriteria<PlayerItem>()
                    .SetProjection(Projections.Property("BaseRecord"))
                    .SetResultTransformer(new DistinctRootEntityResultTransformer())
                    .List<string>();
            }
        }

        /// <summary>
        /// Calculate the name, rarity and stats for a player item and store it to DB
        /// </summary>
        /// <param name="item"></param>
        public override void Save(PlayerItem item) {
            Save(new List<PlayerItem> {item});
            Logger.Info("Stored player item to database.");
        }

        public void Import(List<PlayerItem> items) {
            Logger.Debug($"Importing {items.Count} new items");
            List<PlayerItem> filteredItems;

            // Attempt to exclude any items already owned, only applicable for users with online sync enabled
            using (var session = SessionCreator.OpenSession()) {
                var existingItems = session.CreateCriteria<PlayerItem>()
                    .Add(Restrictions.IsNotNull(nameof(PlayerItem.CloudId)))
                    .List<PlayerItem>();

                filteredItems = items
                    .Where(m => string.IsNullOrEmpty(m.CloudId) || existingItems.All(existing => existing.CloudId != m.CloudId))
                    .Where(m => !Exists(session, m)) // May be slow, but should prevent duplicates better than anything
                    .ToList();
            }

            Save(filteredItems);
        }

        public override void Update(PlayerItem item) {
            Update(new List<PlayerItem> {item}, false);
        }

        public void ResetOnlineSyncState() {
            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    // TODO: Whoever calls this should also ensure that LastSync is set to 0
                    session.CreateSQLQuery($"UPDATE PlayerItem SET {PlayerItemTable.IsCloudSynchronized} = false")
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }

        public void Update(IList<PlayerItem> items, bool clearOnlineId) {
            const string table = nameof(PlayerItem);
            const string stack = nameof(PlayerItem.StackCount);
            var id = nameof(PlayerItem.Id);

            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        // This is if an item has been deleted due to a transfer to stash
                        session.CreateSQLQuery($"UPDATE {table} SET {stack} = :count, {PlayerItemTable.CloudId} = :uuid WHERE {id} = :id")
                            .SetParameter("count", item.StackCount)
                            .SetParameter("id", item.Id)
                            .SetParameter("uuid", item.CloudId)
                            .ExecuteUpdate();
                    }

                    // insert into DeletedPlayerItem(oid) select onlineid from playeritem where onlineid is not null and stackcount <= 0 and id in (1,2,3)
                    session.CreateSQLQuery(
                            $" INSERT OR IGNORE INTO {DeletedPlayerItemTable.Table} ({DeletedPlayerItemTable.Id}) " +
                            $" SELECT {PlayerItemTable.CloudId} FROM {PlayerItemTable.Table} " +
                            $" WHERE {PlayerItemTable.CloudId} IS NOT NULL " +
                            $" AND {PlayerItemTable.Stackcount} <= 0 " +
                            $" AND {PlayerItemTable.Id} IN ( :ids )")
                        .SetParameterList("ids", items.Select(m => m.Id).ToList())
                        .ExecuteUpdate();


                    session.CreateSQLQuery($"DELETE FROM ReplicaItemRow WHERE replicaitemid IN (SELECT id FROM ReplicaItem2 WHERE playeritemid IN ( :ids ))")
                        .SetParameterList("ids", items.Select(m => m.Id).ToList())
                        .ExecuteUpdate();

                    session.CreateSQLQuery($"DELETE FROM ReplicaItem2 WHERE playeritemid IN ( :ids )")
                        .SetParameterList("ids", items.Select(m => m.Id).ToList())
                        .ExecuteUpdate();

                    session.CreateSQLQuery($"DELETE FROM {table} WHERE {stack} <= 0 AND {id} IN ( :ids )")
                        .SetParameterList("ids", items.Select(m => m.Id).ToList())
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }

        private bool Exists(ISession session, string cloudId) {
            var id = session.CreateCriteria<PlayerItem>()
                .Add(Restrictions.Eq(nameof(PlayerItem.CloudId), cloudId))
                .SetMaxResults(1)
                .SetProjection(Projections.RowCountInt64())
                .UniqueResult<long>();

            return id > 0;
        }

        private void UpdateRecords(ISession session, PlayerItem item) {
            const string table = nameof(PlayerItemRecord);
            const string id = nameof(PlayerItemRecord.PlayerItemId);
            const string rec = nameof(PlayerItemRecord.Record);

            // INSERT INTO PlayerItemRecord (PlayerItemId, Record) VALUES (0, 'b') ON CONFLICT DO NOTHING;;
            var records = GetRecordsForItem(item);

            foreach (var record in records) {
                session.CreateSQLQuery($"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES (:id, :record)")
                    .SetParameter("id", item.Id)
                    .SetParameter("record", record)
                    .ExecuteUpdate();
            }
        }

        private void UpdatePetRecords(ISession session, PlayerItem item, Dictionary<string, List<DBStatRow>> stats) {
            const string table = nameof(PlayerItemRecord);
            const string id = nameof(PlayerItemRecord.PlayerItemId);
            const string rec = nameof(PlayerItemRecord.Record);

            var records = GetRecordsForItem(item);

            foreach (var record in GetPetBonusRecords(stats, records)) {
                session.CreateSQLQuery($"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES (:id, :record)")
                    .SetParameter("id", item.Id)
                    .SetParameter("record", record)
                    .ExecuteUpdate();
            }
        }

        /// <summary>
        /// Load the entire ItemTag table into a Tag -> Name lookup.
        /// Done once per rebuild so GetItemName doesn't issue a SELECT per item.
        /// </summary>
        private static Dictionary<string, string> LoadItemTags(ISession session) {
            // Scalar SQL (not CreateCriteria) on purpose: CreateCriteria would pull all ~19k
            // ItemTag rows into the session's first-level cache as managed entities. Those would
            // then be dirty-checked on every auto-flush before each write in the loops below,
            // turning stat updates into an O(items * tags) stall. Scalar rows are not tracked.
            var map = new Dictionary<string, string>();
            var rows = session.CreateSQLQuery($"SELECT {ItemTagTable.Id}, {ItemTagTable.Name} FROM {ItemTagTable.Table}")
                .List();

            foreach (object[] row in rows) {
                if (row[0] is string tag) {
                    map[tag] = row[1] as string;
                }
            }

            return map;
        }

        private void UpdateItemDetails(ISession session, PlayerItem item, Dictionary<string, List<DBStatRow>> stats, Dictionary<string, string> itemTags) {
            const string table = nameof(PlayerItem);
            const string rarity = nameof(PlayerItem.Rarity);
            const string levelReq = nameof(PlayerItem.LevelRequirement);
            const string name = nameof(PlayerItem.Name);
            const string id = nameof(PlayerItem.Id);
            const string prefixRarity = PlayerItemTable.PrefixRarity;
            const string nameLowercase = nameof(PlayerItem.NameLowercase);

            var itemName = ItemOperationsUtility.GetItemName(itemTags, stats, item);
            var records = GetRecordsForItem(item);
            session.CreateSQLQuery($@"UPDATE {table} 
                    SET {name} = :name, 
                    {nameLowercase} = :namelowercase, 
                    {rarity} = :rarity, 
                    {levelReq} = :levelreq, 
                    {prefixRarity} = :prefixrarity
                    WHERE {id} = :id"
                )
                .SetParameter("name", itemName)
                .SetParameter("namelowercase", itemName.ToLowerInvariant())
                .SetParameter("prefixrarity", GetGreenQualityLevelForRecords(stats, records))
                .SetParameter("rarity", ItemOperationsUtility.GetRarityForRecords(stats, records))
                .SetParameter("levelreq", (double) ItemOperationsUtility.GetMinimumLevelForRecords(stats, records))
                .SetParameter("id", item.Id)
                .ExecuteUpdate();
        }

        /// <summary>
        /// Batched INSERT OR IGNORE into PlayerItemRecord using a single reused, prepared command.
        /// Reusing one prepared statement across all rows avoids the per-statement construction cost
        /// (NHibernate query pipeline / command allocation) that dominated the old per-row approach.
        /// </summary>
        private static void InsertPlayerItemRecords(SqliteConnection connection, SqliteTransaction transaction,
                IList<PlayerItem> items, Func<PlayerItem, IEnumerable<string>> recordSelector) {
            var sql = $"INSERT OR IGNORE INTO {nameof(PlayerItemRecord)} " +
                      $"({nameof(PlayerItemRecord.PlayerItemId)}, {nameof(PlayerItemRecord.Record)}) VALUES (@id, @record)";

            using (var cmd = new SqliteCommand(sql, connection, transaction)) {
                var pId = cmd.Parameters.Add("@id", SqliteType.Integer);
                var pRecord = cmd.Parameters.Add("@record", SqliteType.Text);
                cmd.Prepare();

                foreach (var item in items) {
                    foreach (var record in recordSelector(item)) {
                        pId.Value = item.Id;
                        pRecord.Value = (object) record ?? DBNull.Value;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Batched name/rarity/level update for all items, using a single reused, prepared command.
        /// </summary>
        private void UpdateItemDetailsBatch(SqliteConnection connection, SqliteTransaction transaction,
                IList<PlayerItem> items, Dictionary<string, List<DBStatRow>> stats,
                Dictionary<string, string> itemTags, Action<int> progress) {
            var sql = $@"UPDATE {nameof(PlayerItem)}
                    SET {nameof(PlayerItem.Name)} = @name,
                        {nameof(PlayerItem.NameLowercase)} = @namelowercase,
                        {nameof(PlayerItem.Rarity)} = @rarity,
                        {nameof(PlayerItem.LevelRequirement)} = @levelreq,
                        {PlayerItemTable.PrefixRarity} = @prefixrarity
                    WHERE {nameof(PlayerItem.Id)} = @id";

            using (var cmd = new SqliteCommand(sql, connection, transaction)) {
                var pName = cmd.Parameters.Add("@name", SqliteType.Text);
                var pNameLower = cmd.Parameters.Add("@namelowercase", SqliteType.Text);
                var pRarity = cmd.Parameters.Add("@rarity", SqliteType.Text);
                var pLevel = cmd.Parameters.Add("@levelreq", SqliteType.Real);
                var pPrefix = cmd.Parameters.Add("@prefixrarity", SqliteType.Integer);
                var pId = cmd.Parameters.Add("@id", SqliteType.Integer);
                cmd.Prepare();

                foreach (var item in items) {
                    var records = GetRecordsForItem(item);
                    var itemName = ItemOperationsUtility.GetItemName(itemTags, stats, item);

                    pName.Value = (object) itemName ?? DBNull.Value;
                    pNameLower.Value = (object) itemName?.ToLowerInvariant() ?? DBNull.Value;
                    pRarity.Value = (object) ItemOperationsUtility.GetRarityForRecords(stats, records) ?? DBNull.Value;
                    pLevel.Value = (double) ItemOperationsUtility.GetMinimumLevelForRecords(stats, records);
                    pPrefix.Value = GetGreenQualityLevelForRecords(stats, records);
                    pId.Value = item.Id;
                    cmd.ExecuteNonQuery();

                    progress(1);
                }
            }
        }

        /// <summary>
        /// Update internal item stats
        /// May take a lifetime and a half
        /// </summary>
        public void UpdateAllItemStats(IList<PlayerItem> items, Action<int> progress) {
            // A lame workaround for PlayerItemRecord not being available the first run..
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug($"UpdateAllItemStats: starting for {items.Count} items");

            // Writes go through a dedicated raw connection with reused prepared commands (fast bulk
            // path, same approach as the game-db import). The NHibernate session is used only for the
            // reads (GetStats / LoadItemTags), which run after phase 1 has committed.
            using (var session = SessionCreator.OpenSession())
            using (var connection = new SqliteConnection(SessionFactoryLoader.SessionFactory.ConnectionString)) {
                connection.Open();

                // Phase 1: rebuild the base records
                using (var transaction = connection.BeginTransaction()) {
                    using (var delete = new SqliteCommand($"DELETE FROM {nameof(PlayerItemRecord)}", connection, transaction)) {
                        delete.ExecuteNonQuery();
                    }

                    InsertPlayerItemRecords(connection, transaction, items, GetRecordsForItem);
                    transaction.Commit();
                }
                Logger.Debug($"UpdateAllItemStats: phase 1 (base records) done at {sw.ElapsedMilliseconds}ms");

                // Reads: must observe the committed phase-1 records
                var stats = _databaseItemStatDao.GetStats(session, StatFetch.PlayerItems);
                Logger.Debug($"UpdateAllItemStats: GetStats returned {stats.Count} records at {sw.ElapsedMilliseconds}ms");
                var itemTags = LoadItemTags(session);
                Logger.Debug($"UpdateAllItemStats: LoadItemTags returned {itemTags.Count} tags at {sw.ElapsedMilliseconds}ms");

                // Phase 2 + 3: pet records and item details
                using (var transaction = connection.BeginTransaction()) {
                    InsertPlayerItemRecords(connection, transaction, items,
                        item => GetPetBonusRecords(stats, GetRecordsForItem(item)));
                    Logger.Debug($"UpdateAllItemStats: phase 2 (pet records) done at {sw.ElapsedMilliseconds}ms");

                    UpdateItemDetailsBatch(connection, transaction, items, stats, itemTags, progress);
                    Logger.Debug($"UpdateAllItemStats: phase 3 (item details) done at {sw.ElapsedMilliseconds}ms");

                    transaction.Commit();
                }

                Logger.Info($"UpdateAllItemStats: completed {items.Count} items in {sw.ElapsedMilliseconds}ms");
            }
        }

        public void Delete(List<DeleteItemDto> items) {
            using (var session = SessionCreator.OpenSession()) {
                
                using (var transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        session.CreateSQLQuery($"DELETE FROM ComputedItemStat WHERE playeritemid IN (select id from playeritem WHERE {PlayerItemTable.CloudId} = :uuid)")
                            .SetParameter("uuid", item.Id)
                            .ExecuteUpdate();

                        session.CreateSQLQuery($"DELETE FROM ReplicaItem2 WHERE playeritemid IN (select id from playeritem WHERE {PlayerItemTable.CloudId} = :uuid)")
                            .SetParameter("uuid", item.Id)
                            .ExecuteUpdate();

                        session.CreateSQLQuery($"DELETE FROM PlayerItem WHERE {PlayerItemTable.CloudId} = :uuid")
                            .SetParameter("uuid", item.Id)
                            .ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Save a collection of player items
        /// Will update the internal item name before storing, taking in account any pre/suffix
        /// </summary>
        public override void Save(IEnumerable<PlayerItem> items) {
            List<PlayerItem> itemsToStore;
            Logger.Debug($"Storing {items.Count()} new items");

            var ids = new List<long>();

            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    // Items not cloud synced -- No idea why -- Mass import?
                    itemsToStore = items.Where(m => !m.IsCloudSynchronized || !Exists(session, m.CloudId)).ToList();

                    for (var i = 0; i < itemsToStore.Count; i++) {
                        var itemToStore = itemsToStore.ElementAt(i);

                        session.Save(itemToStore);
                        ids.Add(itemToStore.Id);

                        if (i > 0 && i % 1000 == 0) {
                            Logger.Debug($"Have now stored {i} / {itemsToStore.Count} items");
                        }
                    }

                    Logger.Debug("Finished storing items, updating internal records..");

                    // Get the base records stored
                    for (var i = 0; i < itemsToStore.Count; i++) {
                        if (ids.Contains(itemsToStore[i].Id))
                            UpdateRecords(session, itemsToStore[i]);

                        if (i > 0 && i % 1000 == 0) {
                            Logger.Debug($"Have updated internal records for {i} / {itemsToStore.Count} items");
                        }
                    }

                    transaction.Commit();
                    Logger.Debug("Finished storing items");
                }

                if (itemsToStore.Count < 500) {
                    Logger.Debug($"Updating stats for {itemsToStore.Count} new items");

                    using (var transaction = session.BeginTransaction()) {
                        // Now that we have base stats, we can calculate pet records as well
                        var stats = _databaseItemStatDao.GetStats(session, StatFetch.PlayerItems);
                        var itemTags = LoadItemTags(session);

                        for (var i = 0; i < itemsToStore.Count; i++) {
                            if (ids.Contains(itemsToStore.ElementAt(i).Id))
                                UpdatePetRecords(session, itemsToStore.ElementAt(i), stats);
                        }

                        // Get the correct name etc
                        for (var i = 0; i < itemsToStore.Count; i++) {
                            var item = itemsToStore.ElementAt(i);
                            UpdateItemDetails(session, item, stats, itemTags);
                        }

                        transaction.Commit();
                    }
                }
                else {
                    Logger.Debug($"Stat update skipped, too many items.");
                }
            }

            Logger.InfoFormat("Stored {0} player items to database.", itemsToStore.Count);
        }

        public IList<DeletedPlayerItem> GetItemsMarkedForOnlineDeletion() {
            using (var session = SessionCreator.OpenSession()) {
                return session.QueryOver<DeletedPlayerItem>().List();
            }
        }

        public IList<string> GetOnlineIds() {
            using (var session = SessionCreator.OpenSession()) {
                return session.CreateCriteria<PlayerItem>()
                    .SetProjection(Projections.Property(nameof(PlayerItem.CloudId)))
                    .List<string>();
            }
        }

        /// <summary>
        /// Simply delete the 'mark for deletion' tags
        /// </summary>
        /// <returns></returns>
        public void ClearItemsMarkedForOnlineDeletion() {
            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.CreateQuery($"DELETE FROM {nameof(DeletedPlayerItem)}").ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Delete a player item and all its stats
        /// </summary>
        public override void Remove(PlayerItem item) {
            var cloudId = item.CloudId;

            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.CreateQuery($"DELETE FROM {nameof(PlayerItemRecord)} WHERE {nameof(PlayerItemRecord.PlayerItemId)} = :id")
                        .SetParameter("id", item.Id)
                        .ExecuteUpdate();

                    session.Delete(item);

                    transaction.Commit();
                }

                // Mark item for deletion from the online backup
                if (!item.IsCloudSynchronized)
                    return;


                try {
                    using (var transaction = session.BeginTransaction()) {
                        session.SaveOrUpdate(new DeletedPlayerItem {Id = cloudId});
                        transaction.Commit();
                    }
                }
                catch (Exception ex) {
                    Logger.Warn("Unable to mark item for deletion, duplication may occur");
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Check whetever we need to recalculate item stats
        /// </summary>
        /// <returns></returns>
        public bool RequiresStatUpdate() {
            using (var session = SessionCreator.OpenSession()) {
                var itemsLackingRarity = session.CreateCriteria<PlayerItem>().Add(Restrictions.IsNull("Rarity")).List().Count;

                if (session.CreateCriteria<PlayerItem>().Add(Restrictions.IsNull("Rarity")).List().Count > 50) {
                    Logger.Debug($"A stat parse is required, there are {itemsLackingRarity} items lacking rarity");

                    return true;
                }

                if (session.CreateSQLQuery($"SELECT COUNT(*) as C FROM {SkillTable.Table}").UniqueResult<long>() <= 0) {
                    Logger.Debug("A stat parse is required, there no entries in the skills table");

                    return true;
                }

                var itemsLackingName = session.CreateCriteria<PlayerItem>().Add(Restrictions.IsNull("Name")).List().Count;

                if (session.CreateCriteria<PlayerItem>().Add(Restrictions.IsNull("Name")).List().Count > 20) {
                    Logger.Debug($"A stat parse is required, there are {itemsLackingName} items lacking a name");

                    return true;
                }

                if (session.QueryOver<PlayerItemRecord>()
                    .ToRowCountInt64Query()
                    .SingleOrDefault<long>() == 0) {
                    Logger.Debug("A stat parse is required, there no entries in the item records table");

                    return true;
                }

                Logger.Debug("A stat parse is not required");

                return false;
            }
        }

        class DatabaseItemStatQuery {
            public List<string> SQL;
            public Dictionary<string, string[]> Parameters;
        }

        // A PlayerItemRecord row is a "pet record" iff it isn't one of the item's own core records
        // (base/prefix/suffix/materia/ascendant affixes). GetPetBonusRecords inserts the resolved petBonusName targets (which can point at devotion procs, granted skills, set bonuses, relics, etc
        // IFNULL is required: Prefix/Suffix/Materia/Ascendant records are NULL (not '') on most items,
        // and "x NOT IN (a, b, NULL)" evaluates to NULL/false for every row once any operand is NULL.
        private const string PetRecordCondition = @"pir.record NOT IN (
                IFNULL(pi2.BaseRecord, ''), IFNULL(pi2.PrefixRecord, ''), IFNULL(pi2.SuffixRecord, ''),
                IFNULL(pi2.MateriaRecord, ''), IFNULL(pi2.AscendantAffixNameRecord, ''), IFNULL(pi2.AscendantAffix2hNameRecord, '')
            )";

        // Wrap a condition on the databaseitemstat_v2 row (aliased 'dbs') into a subquery returning
        // the matching player item ids. Driven by joining PlayerItemRecord (owned records only) to
        // databaseitem_v2 and databaseitemstat_v2 via indexes, rather than scanning all databaseitems
        // with a correlated EXISTS and an owned-records UNION (which was 20-25x slower on large collections).
        private static string RecordStatSubquery(string dbsCondition, bool petOnly = false) {
            var petJoin = petOnly ? "JOIN PlayerItem pi2 ON pi2.Id = pir.Playeritemid" : "";
            var petFilter = petOnly ? $"AND {PetRecordCondition}" : "";

            return $@"
                SELECT pir.Playeritemid FROM PlayerItemRecord pir
                {petJoin}
                JOIN databaseitem_v2 db ON db.baserecord = pir.record
                JOIN databaseitemstat_v2 dbs ON dbs.id_databaseitem = db.id_databaseitem AND ({dbsCondition})
                {petFilter}";
        }

        private static DatabaseItemStatQuery CreateDatabaseStatQueryParams(ItemSearchRequest query) {
            var queryFragments = new List<string>();
            var queryParamsList = new Dictionary<string, string[]>();

            // Pet-bonus target records store every one of their stats under a "pet"-prefixed name
            // (e.g. "petcharacterAttackSpeedModifier", "petretaliationPhysicalMin"), see StatManager's petStats handling, which does the same strip in reverse.
            var petPrefix = query.PetBonuses ? "pet" : "";

            // Add the damage/resist filter
            foreach (var filter in query.Filters) {
                var effectiveFilter = query.PetBonuses ? filter.Select(s => petPrefix + s).ToArray() : filter;
                queryFragments.Add($"dbs.stat in ( :filter_{filter.GetHashCode()} )");
                queryParamsList.Add($"filter_{filter.GetHashCode()}", effectiveFilter);
            }

            if (query.IsRetaliation) {
                queryFragments.Add($"dbs.stat like '{petPrefix}retaliation%'");
            }

            // TODO: Seems we only have LIST parameters here.. won't work for this, since we'd get OR not AND on classes.
            // No way to get a non-list param?
            foreach (var desiredClass in query.Classes) {
                var classStats = new[] {
                    "augmentSkill1Extras", "augmentSkill2Extras", "augmentSkill3Extras", "augmentSkill4Extras",
                    "augmentMastery1", "augmentMastery2", "augmentMastery3", "augmentMastery4"
                }.Select(s => $"'{petPrefix}{s}'");
                queryFragments.Add(
                    $"dbs.stat IN ({string.Join(",", classStats)}) "
                    + $" AND dbs.TextValue = '{desiredClass}'"); // Not ideal
            }

            if (queryFragments.Count > 0) {
                List<string> sql = new List<string>();
                foreach (var fragment in queryFragments) {
                    sql.Add(RecordStatSubquery(fragment, query.PetBonuses));
                }


                return new DatabaseItemStatQuery {
                    SQL = sql,
                    Parameters = queryParamsList,
                };
            }

            // Pet bonus checked with no other stat filter selected: just require the item to have
            // a pet record at all, i.e. the plain "has a pet bonus" meaning.
            if (query.PetBonuses) {
                return new DatabaseItemStatQuery {
                    SQL = new List<string> {
                        $@"SELECT pir.Playeritemid FROM PlayerItemRecord pir
                           JOIN PlayerItem pi2 ON pi2.Id = pir.Playeritemid
                           WHERE {PetRecordCondition}"
                    },
                    Parameters = queryParamsList,
                };
            }

            return null;
        }



        // A search with broad/empty filters can otherwise match the entire item collection
        // (100k+ rows on a well-played character), which is both useless to show the user
        // and extremely expensive to materialize/merge/sort just to display a 64-item page.
        public const int MaxSearchResults = 1000;

        public List<PlayerItem> SearchForItems(ItemSearchRequest query, out bool wasTruncated, PlayerItem? item = null) {
            Logger.Debug($"Searching for items with query {query}");
            wasTruncated = false;
            var queryFragments = new List<string>();
            var queryParams = new Dictionary<string, object>();

            if (item != null) {
                queryFragments.Add("(PI.id = :playerItemId)");
                queryParams.Add("playerItemId", item.Id);
            }

            if (!string.IsNullOrEmpty(query.Wildcard)) {
                // queryFragments.Add("(PI.namelowercase LIKE :name OR R.text LIKE :wildcard)");
                queryFragments.Add("(PI.namelowercase LIKE :name OR R.id IN (SELECT replicaitemid FROM replicaitemrow WHERE IFNULL(textlowercase, text) LIKE :wildcard))");
                queryParams.Add("wildcard", $"%{query.Wildcard.ToLowerInvariant()}%");
                queryParams.Add("name", $"%{query.Wildcard.Replace(' ', '%').ToLowerInvariant()}%");
            }

            // Filter by mod/hc
            if (string.IsNullOrEmpty(query.Mod)) {
                queryFragments.Add("(PI.Mod IS NULL OR PI.Mod = '')");
            } else {
                queryFragments.Add("LOWER(PI.Mod) = LOWER( :mod )");
                queryParams.Add("mod", query.Mod);
            }

            queryFragments.Add(query.IsHardcore ? "PI.IsHardcore" : "NOT PI.IsHardcore");

            if (!string.IsNullOrEmpty(query.Rarity)) {
                queryFragments.Add("PI.Rarity = :rarity");
                queryParams.Add("rarity", query.Rarity);
            }

            if (query.PrefixRarity > 0) {
                queryFragments.Add("PI.PrefixRarity >= :prefixRarity");
                queryParams.Add("prefixRarity", query.PrefixRarity);
            }

            if (query.SocketedOnly) {
                queryFragments.Add("PI.MateriaRecord is not null and PI.MateriaRecord != ''");
            }

            if (query.DuplicatesOnly) {
                var hcSc = query.IsHardcore ? "IsHardcore" : "NOT IsHardcore";
                if (string.IsNullOrEmpty(query.Mod)) {
                    queryFragments.Add($@"PI.BaseRecord IN (SELECT BaseRecord FROM (
                    select baserecord || prefixrecord || suffixrecord as Records, count(*) as N, BaseRecord from PlayerItem
                    WHERE (Mod IS NULL OR Mod = '')
                    AND {hcSc}
                    group by Records
                    HAVING N > 1
                    order by N desc
                    ))");
                }
                else {
                    queryFragments.Add($@"PI.BaseRecord IN (SELECT BaseRecord FROM (
                    select baserecord || prefixrecord || suffixrecord as Records, count(*) as N, BaseRecord from PlayerItem
                    WHERE LOWER(Mod) = LOWER( :mod )
                    AND {hcSc}
                    group by Records
                    HAVING N > 1
                    order by N desc
                    ))");
                }
            }

            // Add the MINIMUM level requirement (if any)
            if (query.MinimumLevel > 0) {
                queryFragments.Add("PI.LevelRequirement >= :minlevel");
                queryParams.Add("minlevel", query.MinimumLevel);
            }

            // Add the MAXIMUM level requirement (if any)
            if (query.MaximumLevel < 120 && query.MaximumLevel > 0) {
                queryFragments.Add("PI.LevelRequirement <= :maxlevel");
                queryParams.Add("maxlevel", query.MaximumLevel);
            }

            // Show only items from the past 12 hours
            if (query.RecentOnly) {
                queryFragments.Add("created_at > :filter_recentOnly");
                queryParams.Add("filter_recentOnly", DateTime.UtcNow.AddHours(-12).ToTimestamp());
            }

            // Only items which grants new skills
            if (query.WithGrantSkillsOnly) {
                // TODO: Are there any prefixes or suffixes which grants skills?
                queryFragments.Add($"PI.baserecord IN (SELECT PlayerItemRecord from ({ItemSkillDaoImpl.ListItemsQuery}) y)");
            }

            if (query.WithSummonerSkillOnly) {
                queryFragments.Add(@"PI.baserecord IN (SELECT p.baserecord as PlayerItemRecord
                    from itemskill_v2 s, itemskill_mapping map, DatabaseItem_v2 db,  playeritem p, DatabaseItemStat_v2 stat  
                    where s.id_skill = map.id_skill 
                    and map.id_databaseitem = db.id_databaseitem  
                    and db.baserecord = p.baserecord 
                    and stat.id_databaseitem = s.id_databaseitem
                    and stat.stat = 'spawnObjects')");
            }

            var sql = new List<string> {
                $@"select PI.name as Name, 
                PI.StackCount, 
                PI.rarity as Rarity, 
                PI.levelrequirement as LevelRequirement, 
                PI.baserecord as BaseRecord, 
                PI.prefixrecord as PrefixRecord, 
                PI.suffixrecord as SuffixRecord, 
                PI.ModifierRecord as ModifierRecord, 
                PI.MateriaRecord as MateriaRecord,
                PI.AscendantAffixNameRecord as AscendantAffixNameRecord,
                PI.AscendantAffix2hNameRecord as AscendantAffix2hNameRecord,
                PI.{PlayerItemTable.PrefixRarity} as PrefixRarity,
                PI.{PlayerItemTable.CloudId} as CloudId,
                PI.{PlayerItemTable.IsCloudSynchronized} as IsCloudSynchronizedValue,
                PI.{PlayerItemTable.Id} as Id,
                PI.{PlayerItemTable.Mod} as Mod,
                CAST({PlayerItemTable.IsHardcore} as bit) as IsHardcore,
                IFNULL(RerollsUsed, 0) as RerollsUsed,
                IFNULL(AffixRerollsUsed, 0) as AffixRerollsUsed,
                '' AS PetRecord,
                '[]' AS ReplicaInfo,
                PI.{PlayerItemTable.Seed} as Seed
                FROM PlayerItem PI
                LEFT OUTER JOIN ReplicaItem2 R ON PI.ID = R.playeritemid
                WHERE " + string.Join(" AND ", queryFragments)
            };

            var subQuery = CreateDatabaseStatQueryParams(query);

            if (subQuery != null) {
                foreach (var sub in subQuery.SQL) {
                    sql.Add($" AND PI.Id IN ({sub})");
                }
            }

            // Can be several slots for stuff like "2 Handed"
            if (query.Slot?.Length > 0) {
                var subQuerySql = RecordStatSubquery("dbs.stat = 'Class' AND dbs.TextValue in ( :class )");

                sql.Add($" AND PI.Id {(query.SlotInverse ? "NOT" : "")} IN ({subQuerySql})");

                // ItemRelic = Components, we don't want to find every item that has a component, only those that are one.
                if (query.Slot.Length == 1 && query.Slot[0] == "ItemRelic") {
                    sql.Add($" AND PI.{PlayerItemTable.Materia} = ''");
                }
            }

            // Only cap the general "browse" search. The single-item lookup (item != null) is
            // already scoped to one specific id and never needs a limit.
            if (item == null) {
                sql.Add($" LIMIT {MaxSearchResults + 1}");
            }

            var combinedSql = string.Join(" ", sql);

            void BindParams(ISQLQuery target) {
                foreach (var key in queryParams.Keys) {
                    target.SetParameter(key, queryParams[key]);
                }

                if (subQuery != null) {
                    foreach (var key in subQuery.Parameters.Keys) {
                        target.SetParameterList(key, subQuery.Parameters[key]);
                    }
                }

                if (query.Slot?.Length > 0) {
                    target.SetParameterList("class", query.Slot);
                }
            }

            using (var session = SessionCreator.OpenSession()) {
                ISQLQuery q = session.CreateSQLQuery(combinedSql);
                BindParams(q);

                var items = q.List<object>().Select(ToPlayerItem).ToList();

                if (item == null && items.Count > MaxSearchResults) {
                    items = items.Take(MaxSearchResults).ToList();
                    wasTruncated = true;
                }

                return items;
            }
        }


        /// <summary>
        /// Fetch the (expensive) PetRecord/ReplicaInfo columns for a small, specific set of items
        /// (typically just the current page of search results) and populate them onto the given
        /// PlayerItem instances. SearchForItems intentionally leaves these blank for the full match
        /// set, since computing them per-row via correlated subqueries for every match (rather than
        /// just the visible page) does not scale with large item collections.
        /// </summary>
        public void PopulateReplicaAndPetInfo(IList<PlayerItem> items) {
            if (items.Count == 0) {
                return;
            }

            const string sql = @"
                SELECT PI.Id as Id,
                coalesce((SELECT group_concat(Record, '|') FROM PlayerItemRecord pir WHERE pir.PlayerItemId = PI.Id AND NOT Record IN (PI.BaseRecord, PI.SuffixRecord, PI.MateriaRecord, PI.PrefixRecord, PI.AscendantAffixNameRecord, PI.AscendantAffix2hNameRecord)), '') AS PetRecord,
                IFNULL((select json_group_array(json_object('text', text, 'type', type)) from ReplicaItemRow where replicaitemid = R.id), '[]') AS ReplicaInfo
                FROM PlayerItem PI
                LEFT OUTER JOIN ReplicaItem2 R ON PI.ID = R.playeritemid
                WHERE PI.Id IN ( :ids )
            ";

            using (var session = SessionCreator.OpenSession()) {
                var rows = session.CreateSQLQuery(sql)
                    .SetParameterList("ids", items.Select(m => m.Id).Distinct().ToList())
                    .List();

                var byId = new Dictionary<long, (string PetRecord, string ReplicaInfo)>();
                foreach (var row in rows) {
                    var arr = (object[]) row;
                    long id = Convert(arr[0]);
                    string petRecord = Convert<string>(arr[1])?.Trim();
                    string replicaInfo = Convert<string>(arr[2]);
                    byId[id] = (petRecord, replicaInfo);
                }

                foreach (var item in items) {
                    if (byId.TryGetValue(item.Id, out var info)) {
                        item.PetRecord = info.PetRecord;
                        item.ReplicaInfo = info.ReplicaInfo;
                    }
                }
            }
        }


        private static T Convert<T>(object obj) {
            if (obj == null)
                return default(T);

            if (obj is T t) {
                return t;
            } else {
                var message = $"Expected value \"{obj}\" to be type \"{typeof(T)}\", but is \"{obj?.GetType()}\"";
                Logger.Error(message);
                throw new Exception(message);
            }
        }

        private static long Convert(object obj) {
            if (obj is long t) {
                return t;
            } else if (obj is int i) {
                return i;

            } else if (obj is double d) {
                return (long)d;

            } else {
                var message = $"Expected value \"{obj}\" to be type long|int|double, but is \"{obj?.GetType()}\"";
                Logger.Error(message);
                throw new Exception(message);
            }
        }

        private static bool ConvertToBoolean(object obj) {
            if (obj == null)
                return false;

            return Convert(obj) > 0;
        }

        private static PlayerItem ToPlayerItem(object o) {
            object[] arr = (object[])o;
            int idx = 0;
            string name = Convert<string>(arr[idx++]);
            long stackCount = Convert(arr[idx++]);
            string rarity = Convert<string>(arr[idx++]);
            int levelrequirement = (int)Convert(arr[idx++]);
            string baserecord = Convert<string>(arr[idx++])?.Trim();
            string prefixrecord = Convert<string>(arr[idx++])?.Trim();
            string suffixrecord = Convert<string>(arr[idx++])?.Trim();
            string ModifierRecord = Convert<string>(arr[idx++])?.Trim();
            string MateriaRecord = Convert<string>(arr[idx++])?.Trim();
            string AscendantAffixNameRecord = Convert<string>(arr[idx++])?.Trim();
            string AscendantAffix2hNameRecord = Convert<string>(arr[idx++])?.Trim();
            long PrefixRarity = Convert(arr[idx++]);
            string CloudId = Convert<string>(arr[idx++]);
            bool IsCloudSynchronized = ConvertToBoolean(arr[idx++]);
            long Id = Convert(arr[idx++]);
            string Mod = Convert<string>(arr[idx++]);
            bool IsHardcore = ConvertToBoolean(arr[idx++]);
            long RerollsUsed = Convert(arr[idx++]);
            long AffixRerollsUsed = Convert(arr[idx++]);
            string PetRecord = Convert<string>(arr[idx++])?.Trim();
            string replicaInfo = Convert<string>(arr[idx++]);
            long seed = Convert<long>(arr[idx++]);


            return new PlayerItem {
                Name = name,
                StackCount = stackCount,
                Rarity = rarity,
                LevelRequirement = levelrequirement,
                BaseRecord = baserecord,
                PrefixRecord = prefixrecord,
                SuffixRecord = suffixrecord,
                ModifierRecord = ModifierRecord,
                MateriaRecord = MateriaRecord,
                AscendantAffixNameRecord = AscendantAffixNameRecord,
                AscendantAffix2hNameRecord = AscendantAffix2hNameRecord,
                PrefixRarity = PrefixRarity,
                CloudId = CloudId,
                IsCloudSynchronized = IsCloudSynchronized,
                PetRecord = PetRecord,
                Id = Id,
                Mod = Mod,
                IsHardcore = IsHardcore,
                RerollsUsed = RerollsUsed,
                AffixRerollsUsed = AffixRerollsUsed,
                ReplicaInfo = replicaInfo,
                Seed = seed
            };
        }


        public IList<ModSelection> GetModSelection() {
            const string query = "select DISTINCT IFNULL(mod, '') as mod, ishardcore from playeritem";

            using (var session = SessionCreator.OpenSession()) {
                var selection = session.CreateSQLQuery(query)
                    .AddScalar("ishardcore", NHibernateUtil.Boolean)
                    .AddScalar("mod", NHibernateUtil.String)
                    .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ModSelection)))
                    .List<ModSelection>();


                // Even if we have no items, at least list vanilla/nomod
                if (!selection.Any(m => string.IsNullOrEmpty(m.Mod) && m.IsHardcore) && selection.Any(m => m.IsHardcore)) {
                    selection.Add(new ModSelection { IsHardcore = true });
                }

                if (!selection.Any(m => string.IsNullOrEmpty(m.Mod) && !m.IsHardcore) && selection.Any(m => !m.IsHardcore)) {
                    selection.Add(new ModSelection { IsHardcore = false });
                }

                return selection;
            }
        }

        public bool Exists(ISession session, PlayerItem item) {
            var sql = $@"
                SELECT 1 FROM {PlayerItemTable.Table}
                WHERE {PlayerItemTable.Record} = :base
                AND {PlayerItemTable.Prefix} = :prefix
                AND {PlayerItemTable.Suffix} = :suffix
                AND {PlayerItemTable.Materia} = :materia
                AND {PlayerItemTable.ModifierRecord} = :modifier
                AND {PlayerItemTable.Seed} = :seed
                LIMIT 1
            ";

            var result = session.CreateSQLQuery(sql)
                .SetParameter("base", item.BaseRecord)
                .SetParameter("prefix", item.PrefixRecord)
                .SetParameter("suffix", item.SuffixRecord)
                .SetParameter("materia", item.MateriaRecord)
                .SetParameter("modifier", item.ModifierRecord)
                .SetParameter("seed", item.Seed)
                .UniqueResult();

            return result != null;
        }

        public bool Exists(PlayerItem item) {
            using (var session = SessionCreator.OpenSession()) {
                return Exists(session, item);
            }
        }

        public Dictionary<long, string> FindRecordsFromIds(IEnumerable<long> ids) {
            Dictionary<long, string> result = new Dictionary<long, string>();
            using (var session = SessionCreator.OpenSession()) {
                foreach (var pair in session.CreateSQLQuery($"SELECT id,baserecord FROM playeritem WHERE id IN ( :ids )")
                    .SetParameterList("ids", ids)
                    .List()) {

                    var arr = (object[])pair;

                    var baseRecord = Convert<string>(arr[1])?.Trim();
                    long Id = Convert(arr[0]);
                    result[Id] = baseRecord;
                }
            }

            return result;
        }

        public IList<PlayerItem> ListMissingReplica() {


            //TODO: Relics are "ItemArtifact", those will crash the game. (TODO: Is this still true? I don't think so..)
            // TODO: Seems super redundant to check prefix, suffix and materia for Class? BaseRecord determines slot no? Same thing is done in regular searches..
            var specificItemTypesOnlySql = $@"
                SELECT id FROM PlayerItem WHERE baserecord IN (
                    select baserecord from databaseitem_V2 db where db.baserecord in (
                        select baserecord from {PlayerItemTable.Table}
                    )
                    AND exists (
                        select id_databaseitem from databaseitemstat_v2 dbs 
                        WHERE stat = 'Class' 
                        AND TextValue in ( 
                            'ArmorProtective_Head', 
                            'ArmorProtective_Hands', 
                            'ArmorProtective_Feet', 
                            'ArmorProtective_Legs', 
                            'ArmorProtective_Chest', 
                            'ArmorProtective_Waist', 
                            'ArmorJewelry_Medal', 
                            'ArmorJewelry_Ring', 
                            'ArmorProtective_Shoulders', 
                            'ArmorJewelry_Amulet',
                            'WeaponMelee_Dagger', 
                            'WeaponMelee_Mace', 
                            'WeaponMelee_Axe',
                            'WeaponMelee_Scepter',
                            'WeaponMelee_Sword',
                            'WeaponMelee_Spear2h',
                            'WeaponMelee_Sword2h',
                            'WeaponMelee_Mace2h',
                            'WeaponMelee_Axe2h',
                            'WeaponHunting_Ranged1h',
                            'WeaponHunting_Ranged2h',
                            'WeaponArmor_Offhand',
                            'WeaponArmor_Shield',
                            'ItemArtifact'
                        ) 
                        AND db.id_databaseitem = dbs.id_databaseitem
                    )
                )
                ";

            var sql = $@"
                select PI.name as Name, 
                PI.StackCount, 
                PI.rarity as Rarity, 
                PI.levelrequirement as LevelRequirement, 
                PI.baserecord as BaseRecord, 
                PI.prefixrecord as PrefixRecord, 
                PI.suffixrecord as SuffixRecord, 
                PI.ModifierRecord as ModifierRecord, 
                PI.MateriaRecord as MateriaRecord,
                PI.AscendantAffixNameRecord as AscendantAffixNameRecord,
                PI.AscendantAffix2hNameRecord as AscendantAffix2hNameRecord,
                PI.{PlayerItemTable.PrefixRarity} as PrefixRarity,
                PI.{PlayerItemTable.CloudId} as CloudId,
                PI.{PlayerItemTable.IsCloudSynchronized} as IsCloudSynchronizedValue,
                PI.{PlayerItemTable.Id} as Id,
                PI.{PlayerItemTable.Mod} as Mod,
                CAST({PlayerItemTable.IsHardcore} as bit) as IsHardcore,
                IFNULL(RerollsUsed, 0) as RerollsUsed,
                IFNULL(AffixRerollsUsed, 0) as AffixRerollsUsed,
                '' AS PetRecord,
                '[]' AS ReplicaInfo,
                PI.{PlayerItemTable.Seed} as Seed
                FROM PlayerItem PI 
                WHERE PI.Id NOT IN (SELECT R.PlayerItemId FROM ReplicaItem2 R WHERE R.PlayerItemId IS NOT NULL)

                AND PI.Id IN ({specificItemTypesOnlySql})
                
                order by RANDOM ()
                ";



            using (var session = SessionCreator.OpenSession()) {
                return session.CreateSQLQuery(sql).List()
                    .Cast<object>()
                    .Select(ToPlayerItem)
                    .ToList();
            }
        }

    }
}