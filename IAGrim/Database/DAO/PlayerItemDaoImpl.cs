using EvilsoftCommons.Exceptions;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using EvilsoftCommons;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.DAO;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Settings;

namespace IAGrim.Database {
    /// <summary>
    /// Database class for handling player owned items
    /// </summary>
    public class PlayerItemDaoImpl : BaseDao<PlayerItem>, IPlayerItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IPlayerItemDao));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;

        public PlayerItemDaoImpl(
            ISessionCreator sessionCreator,
            IDatabaseItemStatDao databaseItemStatDao, SqlDialect dialect) : base(sessionCreator, dialect) {
            _databaseItemStatDao = databaseItemStatDao;
        }

        /// <summary>
        /// List all player items
        /// </summary>
        /// <returns></returns>
        public IList<PlayerItem> GetByRecord(string prefixRecord, string baseRecord, string suffixRecord, string materiaRecord) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateCriteria<PlayerItem>()
                        .Add(Restrictions.Eq("BaseRecord", baseRecord))
                        .Add(Restrictions.Eq("PrefixRecord", prefixRecord))
                        .Add(Restrictions.Eq("SuffixRecord", suffixRecord))
                        .Add(Restrictions.Eq("MateriaRecord", materiaRecord))
                        .List<PlayerItem>();
                }
            }
        }

        /// <summary>
        /// Return the number of items per record
        /// </summary>
        /// <param name="mod">The mod to check on/for</param>
        /// <returns></returns>
        public Dictionary<string, int> GetCountByRecord(string mod) {
            using (ISession session = SessionCreator.OpenSession()) {
                IList<object[]> rows;
                if (String.IsNullOrEmpty(mod)) {
                    string sql = String.Join(" ",
                        $"SELECT sum(max(1, {PlayerItemTable.Stackcount})), {PlayerItemTable.Record} ",
                        $"FROM {PlayerItemTable.Table}",
                        $"WHERE {PlayerItemTable.Mod} IS NULL OR {PlayerItemTable.Mod} = ''",
                        $"GROUP BY {PlayerItemTable.Record}"
                    );
                    rows = session.CreateSQLQuery(sql).List<object[]>();
                }
                else {
                    string sql = String.Join(" ",
                        $"SELECT sum(max(1, {PlayerItemTable.Stackcount})), {PlayerItemTable.Record} ",
                        $"FROM {PlayerItemTable.Table}",
                        $"WHERE {PlayerItemTable.Mod} = :mod",
                        $"GROUP BY {PlayerItemTable.Record}"
                    );

                    rows = session.CreateSQLQuery(sql).SetParameter("mod", mod)
                        .List<object[]>();
                }

                Dictionary<string, int> result = new Dictionary<string, int>();
                foreach (var row in rows) {
                    var sum = (long) row[0];
                    var record = (string) row[1];
                    result[record] = (int) sum + (result.ContainsKey(record) ? result[record] : 0);
                }

                return result;
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
            var classifications = stats.Where(m => records.Contains(m.Key))
                .SelectMany(m => m.Value.Where(v => v.Stat == "itemClassification"))
                .Select(m => m.TextValue)
                .ToList();

            int score = 0;
            /*
            score += classifications.Count(m => m == "Common");
            score += classifications.Count(m => m == "Yellow") * 2;
            score += classifications.Count(m => m == "Magical") * 3;*/
            if (classifications.All(m => m != "Legendary" && m != "Epic")) {
                score += classifications.Count(m => m == "Rare") * 1;
            }

            return score;
        }


        /// <summary>
        /// Get all the relative records for an item (including suffixes)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<string> GetRecordsForItem(BaseItem item) {
            List<string> records = new List<string>();
            if (!String.IsNullOrEmpty(item.BaseRecord)) {
                records.Add(item.BaseRecord);
            }

            if (!String.IsNullOrEmpty(item.PrefixRecord)) {
                records.Add(item.PrefixRecord);
            }

            if (!String.IsNullOrEmpty(item.SuffixRecord)) {
                records.Add(item.SuffixRecord);
            }

            if (!String.IsNullOrEmpty(item.MateriaRecord)) {
                records.Add(item.MateriaRecord);
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
                    .Add(Restrictions.Not(Restrictions.Or(
                            Restrictions.Eq(nameof(PlayerItem.CachedStats), ""),
                            Restrictions.IsNull(nameof(PlayerItem.CachedStats))
                        ))
                    )
                    .List<PlayerItem>();
            }
        }

        public void SetAsSynchronized(IList<PlayerItem> items) {
            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        var query = session.CreateQuery($@"UPDATE PlayerItem SET 
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
            var relevant = stats.Where(m => records.Contains(m.Key));
            return relevant.SelectMany(m => m.Value)
                .Where(m => m.Stat == "petBonusName")
                .Select(m => m.TextValue);
        }

        /// <summary>
        /// List all player items
        /// </summary>
        /// <returns></returns>
        public IList<string> ListAllRecords() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateCriteria<PlayerItem>()
                        .SetProjection(Projections.Property("BaseRecord"))
                        .SetResultTransformer(new DistinctRootEntityResultTransformer())
                        .List<string>();
                }
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
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var existingItems = session.CreateCriteria<PlayerItem>()
                        .Add(Restrictions.IsNotNull(nameof(PlayerItem.CloudId)))
                        .List<PlayerItem>();

                    filteredItems = items
                        .Where(m => string.IsNullOrEmpty(m.CloudId) || existingItems.All(existing => existing.CloudId != m.CloudId))
                        .Where(m => !Exists(session, m)) // May be slow, but should prevent duplicates better than anything
                        .ToList();
                }
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
                    session.CreateQuery($"UPDATE PlayerItem SET {PlayerItemTable.IsCloudSynchronized} = false")
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }

        public void Update(IList<PlayerItem> items, bool clearOnlineId) {
            const string table = nameof(PlayerItem);
            const string stack = nameof(PlayerItem.StackCount);
            var id = nameof(PlayerItem.Id);
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        // This is if an item has been deleted due to a transfer to stash
                        session.CreateQuery($"UPDATE {table} SET {stack} = :count, {PlayerItemTable.CloudId} = :uuid WHERE {id} = :id")
                            .SetParameter("count", item.StackCount)
                            .SetParameter("id", item.Id)
                            .SetParameter("uuid", item.CloudId)
                            .ExecuteUpdate();
                    }

                    // insert into DeletedPlayerItem(oid) select onlineid from playeritem where onlineid is not null and stackcount <= 0 and id in (1,2,3)
                    session.CreateSQLQuery(SqlUtils.EnsureDialect(Dialect, $" INSERT OR IGNORE INTO {DeletedPlayerItemTable.Table} ({DeletedPlayerItemTable.Id}) " +
                                                                           $" SELECT {PlayerItemTable.CloudId} FROM {PlayerItemTable.Table} " +
                                                                           $" WHERE {PlayerItemTable.CloudId} IS NOT NULL " +
                                                                           $" AND {PlayerItemTable.Stackcount} <= 0 " +
                                                                           $" AND {PlayerItemTable.Id} IN ( :ids )"))
                        .SetParameterList("ids", items.Select(m => m.Id).ToList())
                        .ExecuteUpdate();

                    // Delete any items with stacksize 0 (but only newly transferred ones, ignore any older items in case of errors)
                    session.CreateQuery($"DELETE FROM {table} WHERE {stack} <= 0 AND {id} IN ( :ids )")
                        .SetParameterList("ids", items.Select(m => m.Id).ToList())
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }


        private bool Exists(ISession session, string cloudId) {
            long id = session.CreateCriteria<PlayerItem>()
                .Add(Restrictions.Eq(nameof(PlayerItem.CloudId), cloudId))
                .SetMaxResults(1)
                .SetProjection(Projections.RowCountInt64())
                .UniqueResult<long>();

            return id > 0;
        }


        private void UpdateRecords(ISession session, PlayerItem item) {
            var table = nameof(PlayerItemRecord);
            var id = nameof(PlayerItemRecord.PlayerItemId);
            var rec = nameof(PlayerItemRecord.Record);

            // INSERT INTO PlayerItemRecord (PlayerItemId, Record) VALUES (0, 'b') ON CONFLICT DO NOTHING;;
            var records = GetRecordsForItem(item);
            foreach (var record in records) {
                session.CreateSQLQuery(SqlUtils.EnsureDialect(Dialect, $"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES (:id, :record)"))
                    .SetParameter("id", item.Id)
                    .SetParameter("record", record)
                    .ExecuteUpdate();
            }
        }

        private void UpdatePetRecords(ISession session, PlayerItem item, Dictionary<string, List<DBStatRow>> stats) {
            var records = GetRecordsForItem(item);

            var table = nameof(PlayerItemRecord);
            var id = nameof(PlayerItemRecord.PlayerItemId);
            var rec = nameof(PlayerItemRecord.Record);
            foreach (var record in GetPetBonusRecords(stats, records)) {
                session.CreateSQLQuery(SqlUtils.EnsureDialect(Dialect, $"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES (:id, :record)"))
                    .SetParameter("id", item.Id)
                    .SetParameter("record", record)
                    .ExecuteUpdate();
            }
        }

        private void UpdateItemDetails(ISession session, PlayerItem item, Dictionary<string, List<DBStatRow>> stats) {
            var table = nameof(PlayerItem);
            var rarity = nameof(PlayerItem.Rarity);
            var levelReq = nameof(PlayerItem.LevelRequirement);
            var name = nameof(PlayerItem.Name);
            var id = nameof(PlayerItem.Id);
            var prefixRarity = PlayerItemTable.PrefixRarity;
            var nameLowercase = nameof(PlayerItem.NameLowercase);


            var itemName = ItemOperationsUtility.GetItemName(session, stats, item);
            var records = GetRecordsForItem(item);
            session.CreateQuery($@"UPDATE {table} 
                    SET {name} = :name, 
                    {nameLowercase} = :namelowercase, 
                    {rarity} = :rarity, 
                    {levelReq} = :levelreq, 
                    {prefixRarity} = :prefixrarity,
                    cachedstats = NULL,
                    searchabletext = NULL
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
        /// Update internal item stats
        /// May take a lifetime and a half
        /// </summary>
        public void UpdateAllItemStats(IList<PlayerItem> items, Action<int> progress) {
            // A lame workaround for PlayerItemRecord not being available the first run..

            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateQuery("DELETE FROM PlayerItemRecord")
                        .ExecuteUpdate();

                    // Get the base records stored
                    for (int i = 0; i < items.Count; i++) {
                        UpdateRecords(session, items.ElementAt(i));
                    }

                    transaction.Commit();
                }


                using (ITransaction transaction = session.BeginTransaction()) {
                    // Now that we have base stats, we can calculate pet records as well
                    var stats = _databaseItemStatDao.GetStats(session, StatFetch.PlayerItems);
                    for (int i = 0; i < items.Count; i++) {
                        UpdatePetRecords(session, items.ElementAt(i), stats);
                    }


                    foreach (PlayerItem item in items) {
                        UpdateItemDetails(session, item, stats);

                        progress(1);
                    }

                    transaction.Commit();
                }
            }
        }

        public void Delete(List<DeleteItemDto> items) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        session.CreateQuery($"DELETE FROM PlayerItem WHERE {PlayerItemTable.CloudId} = :uuid")
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
        public override void Save(IEnumerable<PlayerItem> items_) {
            List<PlayerItem> items;

            Logger.Debug($"Storing {items_.Count()} new items");

            List<long> ids = new List<long>();
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    // Items not cloud synced -- No idea why -- Mass import?
                    items = items_.Where(m => !m.IsCloudSynchronized || !Exists(session, m.CloudId)).ToList();

                    for (int i = 0; i < items.Count; i++) {
                        var item = items.ElementAt(i);

                        session.Save(item);
                        ids.Add(item.Id);

                        if (i > 0 && i % 1000 == 0) {
                            Logger.Debug($"Have now stored {i} / {items.Count} items");
                        }
                    }


                    Logger.Debug("Finished storing items, updating internal records..");

                    // Get the base records stored
                    for (int i = 0; i < items.Count; i++) {
                        if (ids.Contains(items[i].Id))
                            UpdateRecords(session, items[i]);


                        if (i > 0 && i % 1000 == 0) {
                            Logger.Debug($"Have updated internal records for {i} / {items.Count} items");
                        }
                    }

                    transaction.Commit();
                    Logger.Debug("Finished storing items");
                }


                if (items.Count < 500) {
                    Logger.Debug($"Updating stats for {items.Count} new items");
                    using (ITransaction transaction = session.BeginTransaction()) {
                        // Now that we have base stats, we can calculate pet records as well
                        var stats = _databaseItemStatDao.GetStats(session, StatFetch.PlayerItems);
                        for (int i = 0; i < items.Count; i++) {
                            if (ids.Contains(items.ElementAt(i).Id))
                                UpdatePetRecords(session, items.ElementAt(i), stats);
                        }

                        // Get the correct name etc
                        for (int i = 0; i < items.Count; i++) {
                            var item = items.ElementAt(i);
                            UpdateItemDetails(session, item, stats);
                        }


                        transaction.Commit();
                    }
                }
                else {
                    Logger.Debug($"Stat update skipped, too many items.");
                }
            }


            Logger.InfoFormat("Stored {0} player items to database.", items.Count);
        }

        public IList<DeletedPlayerItem> GetItemsMarkedForOnlineDeletion() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.QueryOver<DeletedPlayerItem>().List();
                }
            }
        }

        public IList<string> GetOnlineIds() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateCriteria<PlayerItem>()
                        .SetProjection(Projections.Property(nameof(PlayerItem.CloudId)))
                        .List<string>();
                }
            }
        }

        /// <summary>
        /// Simply delete the 'mark for deletion' tags
        /// </summary>
        /// <returns></returns>
        public void ClearItemsMarkedForOnlineDeletion() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
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
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateQuery($"DELETE FROM {nameof(PlayerItemRecord)} WHERE {nameof(PlayerItemRecord.PlayerItemId)} = :id")
                        .SetParameter("id", item.Id)
                        .ExecuteUpdate();


                    session.Delete(item);

                    transaction.Commit();
                }

                // Mark item for deletion from the online backup
                if (item.IsCloudSynchronized) {
                    try {
                        using (ITransaction transaction = session.BeginTransaction()) {
                            session.SaveOrUpdate(new DeletedPlayerItem {Id = cloudId});
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex) {
                        Logger.Warn("Unable to mark item for deletion, duplication may occur");
                        Logger.Warn(ex.Message);
                        Logger.Warn(ex.StackTrace);
                        ExceptionReporter.ReportException(ex);
                    }
                }
            }
        }


        /// <summary>
        /// Check whetever we need to recalculate item stats
        /// </summary>
        /// <returns></returns>
        public bool RequiresStatUpdate() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
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
        }


        class DatabaseItemStatQuery {
            public String SQL;
            public Dictionary<string, string[]> Parameters;
        }

        private static DatabaseItemStatQuery CreateDatabaseStatQueryParams(ItemSearchRequest query) {
            List<string> queryFragments = new List<string>();
            Dictionary<string, string[]> queryParamsList = new Dictionary<string, string[]>();

            // Add the damage/resist filter
            foreach (string[] filter in query.Filters) {
                queryFragments.Add($"exists (select id_databaseitem from databaseitemstat_v2 dbs where stat in ( :filter_{filter.GetHashCode()} ) and db.id_databaseitem = dbs.id_databaseitem)");
                queryParamsList.Add($"filter_{filter.GetHashCode()}", filter);
            }


            if (query.IsRetaliation) {
                queryFragments.Add("exists (select id_databaseitem from databaseitemstat_v2 dbs where stat like 'retaliation%' and db.id_databaseitem = dbs.id_databaseitem)");
            }

            // TODO: Seems we only have LIST parameters here.. won't work for this, since we'd get OR not AND on classes.
            // No way to get a non-list param?
            foreach (var desiredClass in query.Classes) {
                queryFragments.Add("exists (select id_databaseitem from databaseitemstat_v2 dbs where stat IN ('augmentSkill1Extras','augmentSkill2Extras','augmentSkill3Extras','augmentSkill4Extras','augmentMastery1','augmentMastery2','augmentMastery3','augmentMastery4') "
                                   + $" AND TextValue = '{desiredClass}'" // Not ideal
                                   + " AND db.id_databaseitem = dbs.id_databaseitem)");
            }


            if (queryFragments.Count > 0) {
                string sql = $@"
                SELECT Playeritemid FROM PlayerItemRecord WHERE record IN (
                    select baserecord from databaseitem_V2 db where db.baserecord in (
                        select baserecord from playeritem union 
                        select prefixrecord from playeritem union 
                        select suffixrecord from playeritem union 
                        select materiarecord from playeritem
                    )
                    AND {String.Join(" AND ", queryFragments)}
                )
                ";
                return new DatabaseItemStatQuery {
                    SQL = sql,
                    Parameters = queryParamsList,
                };
            }

            return null;
        }

        private static PlayerItem ToPlayerItem(object o) {
            object[] arr = (object[]) o;
            int idx = 0;
            string name = arr[idx++] as string;
            long stackCount = (long) arr[idx++];
            string rarity = (string) arr[idx++];
            int levelrequirement = (int) (double) arr[idx++];
            string baserecord = (string) arr[idx++];
            string prefixrecord = (string) arr[idx++];
            string suffixrecord = (string) arr[idx++];
            string ModifierRecord = (string) arr[idx++];
            string MateriaRecord = (string) arr[idx++];
            int PrefixRarity = (int) arr[idx++];
            string AzureUuid = (string) arr[idx++];
            string CloudId = (string) arr[idx++];
            long? IsCloudSynchronized = (long?) arr[idx++];
            string CachedStats = (string) arr[idx++];
            string PetRecord = (string) arr[idx++];

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
                PrefixRarity = PrefixRarity,
                AzureUuid = AzureUuid,
                CloudId = CloudId,
                IsCloudSynchronized = IsCloudSynchronized.HasValue && IsCloudSynchronized.Value == 1,
                CachedStats = CachedStats,
                PetRecord = PetRecord
            };
        }

        public List<PlayerItem> SearchForItems(ItemSearchRequest query) {
            Logger.Debug($"Searching for items with query {query}");
            List<string> queryFragments = new List<string>();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            if (!String.IsNullOrEmpty(query.Wildcard)) {
                queryFragments.Add("(PI.namelowercase LIKE :name OR searchabletext LIKE :wildcard)");
                queryParams.Add("name", $"%{query.Wildcard.Replace(' ', '%').ToLower()}%");
                queryParams.Add("wildcard", $"%{query.Wildcard.ToLower()}%");
            }

            // Filter by mod/hc
            queryFragments.Add("(LOWER(PI.Mod) = LOWER( :mod ) OR PI.Mod IS NULL)");
            queryParams.Add("mod", query.Mod);

            if (query.IsHardcore)
                queryFragments.Add("PI.IsHardcore");
            else
                queryFragments.Add("NOT PI.IsHardcore");

            if (!String.IsNullOrEmpty(query.Rarity)) {
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


            List<string> sql = new List<string>();
            sql.Add($@"select name as Name, 
                StackCount, 
                rarity as Rarity, 
                levelrequirement as LevelRequirement, 
                baserecord as BaseRecord, 
                prefixrecord as PrefixRecord, 
                suffixrecord as SuffixRecord, 
                ModifierRecord as ModifierRecord, 
                MateriaRecord as MateriaRecord,
                {PlayerItemTable.PrefixRarity} as PrefixRarity,
                {PlayerItemTable.AzureUuid} as AzureUuid,
                {PlayerItemTable.CloudId} as CloudId,
                {PlayerItemTable.IsCloudSynchronized} as IsCloudSynchronizedValue,
                {PlayerItemTable.Id} as Id,
                CachedStats as CachedStats,
                coalesce((SELECT group_concat(Record, '|') FROM PlayerItemRecord pir WHERE pir.PlayerItemId = PI.Id AND NOT Record IN (PI.BaseRecord, PI.SuffixRecord, PI.MateriaRecord, PI.PrefixRecord)), '') AS PetRecord
                FROM PlayerItem PI WHERE " + string.Join(" AND ", queryFragments));

            var subquery = CreateDatabaseStatQueryParams(query);
            if (subquery != null) {
                sql.Add(" AND PI.Id IN (" + subquery.SQL + ")");
            }


            // Can be several slots for stuff like "2 Handed"
            if (query.Slot?.Length > 0) {
                string subquerySql = $@"
                SELECT Playeritemid FROM PlayerItemRecord WHERE record IN (
                    select baserecord from databaseitem_V2 db where db.baserecord in (
                        select baserecord from {PlayerItemTable.Table} union 
                        select prefixrecord from {PlayerItemTable.Table} union 
                        select suffixrecord from {PlayerItemTable.Table} union 
                        select materiarecord from {PlayerItemTable.Table}
                    )
                    AND exists (
                        select id_databaseitem from databaseitemstat_v2 dbs 
                        WHERE stat = 'Class' 
                        AND TextValue in ( :class ) 
                        AND db.id_databaseitem = dbs.id_databaseitem
                    )
                )
                ";

                sql.Add($" AND PI.Id IN ({subquerySql})");

                // ItemRelic = Components, we don't want to find every item that has a component, only those that are one.
                if (query.Slot.Length == 1 && query.Slot[0] == "ItemRelic") {
                    sql.Add($" AND PI.{PlayerItemTable.Materia} = ''");
                }
            }

            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    IQuery q = session.CreateSQLQuery(string.Join(" ", sql));
                    foreach (var key in queryParams.Keys) {
                        q.SetParameter(key, queryParams[key]);
                        Logger.Debug($"{key}: " + queryParams[key]);
                    }

                    if (subquery != null) {
                        foreach (var key in subquery.Parameters.Keys) {
                            var parameterList = subquery.Parameters[key];
                            q.SetParameterList(key, parameterList);
                            Logger.Debug($"{key}: " + string.Join(",", subquery.Parameters[key]));
                        }
                    }

                    if (query.Slot?.Length > 0) {
                        q.SetParameterList("class", query.Slot);
                    }

                    Logger.Debug(q.QueryString);
                    q.SetResultTransformer(new AliasToBeanResultTransformer(typeof(PlayerItem)));
                    /*
                    List<PlayerItem> items = new List<PlayerItem>();
                    foreach (var item in q.List()) {
                        items.Add(ToPlayerItem(item));
                    }*/

                    var items = ItemOperationsUtility.MergeStackSize(q.List<PlayerItem>());

                    Logger.Debug($"Search returned {items.Count} items");
                    return items;
                }
            }
        }

        public IList<ModSelection> GetModSelection() {
            const string query = "SELECT DISTINCT Mod as Mod, IsHardcore as IsHardcore FROM PlayerItem WHERE Mod IS NOT NULL";
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var selection = session.CreateQuery(query)
                        .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ModSelection)))
                        .List<ModSelection>();

                    return selection;
                }
            }
        }

        public bool Exists(ISession session, PlayerItem item) {
            string sql = $@"
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
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return Exists(session, item);
                }
            }
        }

        /// <summary>
        /// Delete duplicate items (items duplicated via bugs, not simply similar items)
        /// </summary>
        public void DeleteDuplicates() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    // Mark all duplicates for deletion from online backups
                    session.CreateSQLQuery(@"
insert into deletedplayeritem_v3(id)
select cloudid FROM playeritem WHERE Id IN (
	SELECT Id FROM (
		SELECT MAX(Id) as Id, (baserecord || prefixrecord || modifierrecord || suffixrecord || materiarecord || transmuterecord || seed) as UQ FROM PlayerItem WHERE (baserecord || prefixrecord || modifierrecord || suffixrecord || materiarecord || transmuterecord || seed) IN (
			SELECT UQ FROM (
				SELECT (baserecord || prefixrecord || modifierrecord || suffixrecord || materiarecord || transmuterecord || seed) as UQ, COUNT(*) as C FROM PlayerItem
					WHERE baserecord NOT LIKE '%materia%'
					AND baserecord NOT LIKE '%questitems%'
					AND baserecord NOT LIKE '%potions%'
					AND baserecord NOT LIKE '%crafting%'
					AND StackCount < 2 -- Potions, components, etc
					GROUP BY UQ
				) X 
			WHERE C > 1
		)

		Group BY UQ
	) Z
)

AND cloud_hassync
AND cloudid IS NOT NULL 
AND cloudid NOT IN (SELECT id FROM deletedplayeritem_v3)
AND cloudid != ''
").ExecuteUpdate();
                    // Delete duplicates (if there are multiple, only one will be deleted)
                    int duplicatesDeleted = session.CreateSQLQuery(@"
DELETE FROM PlayerItem WHERE Id IN (
	SELECT Id FROM (
		SELECT MAX(Id) as Id, (baserecord || prefixrecord || modifierrecord || suffixrecord || materiarecord || transmuterecord || seed) as UQ FROM PlayerItem WHERE (baserecord || prefixrecord || modifierrecord || suffixrecord || materiarecord || transmuterecord || seed) IN (
			SELECT UQ FROM (
				SELECT (baserecord || prefixrecord || modifierrecord || suffixrecord || materiarecord || transmuterecord || seed) as UQ, COUNT(*) as C FROM PlayerItem
					WHERE baserecord NOT LIKE '%materia%'
					AND baserecord NOT LIKE '%questitems%'
					AND baserecord NOT LIKE '%potions%'
					AND baserecord NOT LIKE '%crafting%'
					AND StackCount < 2 -- Potions, components, etc
					GROUP BY UQ
				) X 
			WHERE C > 1
		)

		Group BY UQ
	) Z
)
").ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }

        public IList<PlayerItem> ListWithMissingStatCache() {
            
            var sql = $@"
                select name as Name, 
                {PlayerItemTable.Id} as Id,
                {PlayerItemTable.Stackcount}, 
                rarity as Rarity, 
                levelrequirement as LevelRequirement, 
                {PlayerItemTable.Record} as BaseRecord, 
                {PlayerItemTable.Prefix} as PrefixRecord, 
                {PlayerItemTable.Suffix} as SuffixRecord, 
                {PlayerItemTable.ModifierRecord} as ModifierRecord, 
                MateriaRecord as MateriaRecord,
                {PlayerItemTable.PrefixRarity} as PrefixRarity,
                {PlayerItemTable.AzureUuid} as AzureUuid,
                {PlayerItemTable.CloudId} as CloudId,
                {PlayerItemTable.IsCloudSynchronized} as IsCloudSynchronizedValue,
                CachedStats as CachedStats,
                coalesce((SELECT group_concat(Record, '|') FROM PlayerItemRecord pir WHERE pir.PlayerItemId = PI.Id AND NOT Record IN (PI.BaseRecord, PI.SuffixRecord, PI.MateriaRecord, PI.PrefixRecord)),'') AS PetRecord
                FROM PlayerItem PI WHERE CachedStats IS NULL OR CachedStats = '' LIMIT 50";
        
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery(sql)
                        .SetResultTransformer(new AliasToBeanResultTransformer(typeof(PlayerItem)))
                        .List<PlayerItem>();
                }
            }
        }


        public void UpdateCachedStats(IList<PlayerItem> items) {
            var table = nameof(PlayerItem);
            var searchableText = nameof(PlayerItem.SearchableText);
            var cachedStats = nameof(PlayerItem.CachedStats);
            var id = nameof(PlayerItem.Id);

            using (ISession session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        session.CreateQuery($@"UPDATE {table} SET {searchableText} = :searchableText, {cachedStats} = :cachedStats WHERE {id} = :id")
                            .SetParameter("cachedStats", item.CachedStats)
                            .SetParameter("searchableText", item.SearchableText.ToLowerInvariant())
                            .SetParameter("id", item.Id)
                            .ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}