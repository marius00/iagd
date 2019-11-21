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
using IAGrim.Backup.Azure.Dto;
using IAGrim.Database.DAO;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.DAO.Util;
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
            IDatabaseItemStatDao databaseItemStatDao) : base(sessionCreator) {
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
                if (string.IsNullOrEmpty(mod)) {
                    string sql = string.Join(" ",
                        $"SELECT sum(max(1, {PlayerItemTable.Stackcount})), {PlayerItemTable.Record} ",
                          $"FROM {PlayerItemTable.Table}",
                          $"WHERE {PlayerItemTable.Mod} IS NULL OR {PlayerItemTable.Mod} = ''",
                          $"GROUP BY {PlayerItemTable.Record}"
                    );
                    rows = session.CreateSQLQuery(sql).List<object[]>();
                }
                else {
                    string sql = string.Join(" ",
                        $"SELECT sum(max(1, {PlayerItemTable.Stackcount})), {PlayerItemTable.Record} ",
                        $"FROM {PlayerItemTable.Table}",
                        $"WHERE {PlayerItemTable.Mod} = :mod",
                        $"GROUP BY {PlayerItemTable.Record}"
                    );

                    rows = session.CreateSQLQuery(sql).SetParameter("mod", mod)
                        .List<object[]>();
                }

                Dictionary < string, int> result = new Dictionary<string, int>();
                foreach (var row in rows) {
                    var sum = (long)row[0];
                    var record = (string)row[1];
                    result[record] = (int)sum + (result.ContainsKey(record) ? result[record] : 0);
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
        private static List<string> GetRecordsForItem(PlayerItem item) {
            List<string> records = new List<string>();
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

            return records;
        }
        
        public IList<PlayerItem> GetUnsynchronizedItems() {
            using (var session = SessionCreator.OpenSession()) {
                var req = Restrictions.Or(
                    Restrictions.Eq(nameof(PlayerItem.AzureUuid), ""), 
                    Restrictions.Eq(nameof(PlayerItem.AzureHasSynchronized), false)
                    );

                return session.CreateCriteria<PlayerItem>()
                    .Add(req)
                    .List<PlayerItem>();
            }
        }

        public void SetAsSynchronized(IList<PlayerItem> items) {
            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        session.CreateQuery($"UPDATE PlayerItem SET " +
                                            $" {PlayerItemTable.AzureHasSynchronized} = true, " +
                                            // Should not be needed for new items, required for legacy items which had no partition.
                                            $" {PlayerItemTable.AzureUuid} = :uuid, " +
                                            $" {PlayerItemTable.AzurePartition} = :partition " +
                                            $"WHERE Id = :id")
                            .SetParameter("id", item.Id)
                            .SetParameter("uuid", item.AzureUuid)
                            .SetParameter("partition", item.AzurePartition)
                            .ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
        }

        public long GetNumItems(string backupPartition) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery($"SELECT COUNT(*) FROM {PlayerItemTable.Table} WHERE {PlayerItemTable.AzurePartition} = :partition")
                        .SetParameter("partition", backupPartition)
                        .UniqueResult<long>();
                }
            }
        }


        private IEnumerable<string> GetPetBonusRecords(Dictionary<string, List<DBStatRow>> stats, List<string> records) {
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
            Save(new List<PlayerItem> { item });
            Logger.Info("Stored player item to database.");
        }



        public void Import(List<PlayerItem> items) {
            Logger.Debug($"Importing {items.Count} new items");
            List<PlayerItem> filteredItems;

            // Attempt to exclude any items already owned, only applicable for users with online sync enabled
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var existingItems = session.CreateCriteria<PlayerItem>()
                        .Add(Restrictions.IsNotNull(nameof(PlayerItem.AzureUuid)))
                        .List<PlayerItem>();

                    filteredItems = items
                        .Where(m => string.IsNullOrEmpty(m.AzureUuid) || !existingItems.Any(item => item.AzureUuid == m.AzureUuid))
                        .Where(m => !Exists(session, m)) // May be slow, but should prevent duplicates better than anything
                        .ToList();
                }
            }

            Save(filteredItems);
        }

        public override void Update(PlayerItem item) {
            Update(new List<PlayerItem> { item }, false);
        }

        public void ResetOnlineSyncState() {
            using (var session = SessionCreator.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.CreateQuery($"UPDATE PlayerItem SET {PlayerItemTable.AzureHasSynchronized} = false")
                        .ExecuteUpdate();
                    session.CreateQuery($"DELETE FROM {nameof(AzurePartition)}")
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
                        /*
                        if (clearOnlineId && !string.IsNullOrEmpty(item.AzureUuid) ) {
                            session.CreateSQLQuery($"INSERT OR IGNORE INTO {DeletedPlayerItemTable.Table} ({DeletedPlayerItemTable.Id}) VALUES (:id)")
                                .SetParameter("id", item.OnlineId.Value)
                                .ExecuteUpdate();

                            item.OnlineId = null;
                        }*/

                        session.CreateQuery($"UPDATE {table} SET {stack} = :count, {PlayerItemTable.AzureUuid} = :uuid WHERE {id} = :id")
                            .SetParameter("count", item.StackCount)
                            .SetParameter("id", item.Id)
                            .SetParameter("uuid", item.AzureUuid)
                            .ExecuteUpdate();
                    }
                    
                    // insert into DeletedPlayerItem(oid) select onlineid from playeritem where onlineid is not null and stackcount <= 0 and id in (1,2,3)
                    session.CreateSQLQuery($" INSERT OR IGNORE INTO {DeletedPlayerItemTable.Table} ({DeletedPlayerItemTable.Id}, {DeletedPlayerItemTable.Partition}) " +
                                        $" SELECT {PlayerItemTable.AzureUuid}, {PlayerItemTable.AzurePartition} FROM {PlayerItemTable.Table} " +
                                        $" WHERE {PlayerItemTable.AzureUuid} IS NOT NULL " +
                                        $" AND {PlayerItemTable.Stackcount} <= 0 " +
                                        $" AND {PlayerItemTable.Id} IN ( :ids )")
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
        /*
        private long? GetIfExists(ISession session, PlayerItem item) {
            long? id = session.CreateCriteria<PlayerItem>()
                .Add(Restrictions.Eq(nameof(PlayerItem.BaseRecord), item.BaseRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.PrefixRecord), item.PrefixRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.SuffixRecord), item.SuffixRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.ModifierRecord), item.ModifierRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.TransmuteRecord), item.TransmuteRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.Seed), item.Seed))
                .Add(Restrictions.Eq(nameof(PlayerItem.MateriaRecord), item.MateriaRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.RelicCompletionBonusRecord), item.RelicCompletionBonusRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.RelicSeed), item.RelicSeed))
                .Add(Restrictions.Eq(nameof(PlayerItem.EnchantmentRecord), item.EnchantmentRecord))
                .Add(Restrictions.Eq(nameof(PlayerItem.EnchantmentSeed), item.EnchantmentSeed))
                .Add(Restrictions.Eq(nameof(PlayerItem.MateriaCombines), item.MateriaCombines))
                .Add(Restrictions.Ge(nameof(PlayerItem.StackCount), 1L)) // Old entries may have 0
                .SetMaxResults(1)
                .SetProjection(Projections.Id())
                .UniqueResult<long?>();

            return id;
        }*/

        private bool Exists(ISession session, string azurePartition, string azureUuid) {
            long id = session.CreateCriteria<PlayerItem>()
                .Add(Restrictions.Eq(nameof(PlayerItem.AzurePartition), azurePartition))
                .Add(Restrictions.Eq(nameof(PlayerItem.AzureUuid), azureUuid))
                .SetMaxResults(1)
                .SetProjection(Projections.RowCountInt64())
                .UniqueResult<long>();

            return id > 0;
        }


        private void UpdateRecords(ISession session, PlayerItem item) {
            var table = nameof(PlayerItemRecord);
            var id = nameof(PlayerItemRecord.PlayerItemId);
            var rec = nameof(PlayerItemRecord.Record);

            var records = GetRecordsForItem(item);
            foreach (var record in records) {
                session.CreateSQLQuery($"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES (:id, :record);")
                    .SetParameter("id", item.Id)
                    .SetParameter("record", record)
                    .ExecuteUpdate();
            }
        }

        private void UpdatePetRecords(ISession session, PlayerItem item, Dictionary<String, List<DBStatRow>> stats) {
            var records = GetRecordsForItem(item);

            var table = nameof(PlayerItemRecord);
            var id = nameof(PlayerItemRecord.PlayerItemId);
            var rec = nameof(PlayerItemRecord.Record);
            foreach (var record in GetPetBonusRecords(stats, records)) {

                session.CreateSQLQuery($"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES (:id, :record);")
                    .SetParameter("id", item.Id)
                    .SetParameter("record", record)
                    .ExecuteUpdate();
            }

        }

        private void UpdateItemDetails(ISession session, PlayerItem item, Dictionary<String, List<DBStatRow>> stats) {
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
                    SET {name} = :name, {nameLowercase} = :namelowercase, {rarity} = :rarity, {levelReq} = :levelreq, {prefixRarity} = :prefixrarity 
                    WHERE {id} = :id"
                )
                .SetParameter("name", itemName)
                .SetParameter("namelowercase", itemName.ToLowerInvariant())
                .SetParameter("prefixrarity", GetGreenQualityLevelForRecords(stats, records))
                .SetParameter("rarity", ItemOperationsUtility.GetRarityForRecords(stats, records))
                .SetParameter("levelreq", ItemOperationsUtility.GetMinimumLevelForRecords(stats, records))
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

        public void Delete(List<AzureItemDeletionDto> items) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        session.CreateQuery($"DELETE FROM PlayerItem WHERE {PlayerItemTable.AzurePartition} = :partition AND {PlayerItemTable.AzureUuid} = :uuid")
                            .SetParameter("partition", item.Partition)
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
                    items = items_.Where(m => string.IsNullOrWhiteSpace(m.AzurePartition) || !Exists(session, m.AzurePartition, m.AzureUuid)).ToList();

                    for (int i = 0; i < items.Count; i++) {
                        var item = items.ElementAt(i);

                        // Stack item if possible
                        session.Save(item);
                        ids.Add(item.Id);

                        if (i > 0 && i % 1000 == 0) {
                            Logger.Debug($"Have now stored {i} / {items.Count} items");
                        }
                    }


                    Logger.Debug("Finished storing items, updating internal records..");

                    // Get the base records stored
                    for (int i = 0; i < items.Count; i++) {
                        if (ids.Contains(items.ElementAt(i).Id))
                            UpdateRecords(session, items.ElementAt(i));


                        if (i > 0 && i % 1000 == 0) {
                            Logger.Debug($"Have updated internal recorsd for {i} / {items.Count} items");
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
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.QueryOver<DeletedPlayerItem>().List();
                }
            }
        }

        /// <summary>
        /// Simply delete the 'mark for deletion' tags
        /// </summary>
        /// <returns></returns>
        public int ClearItemsMarkedForOnlineDeletion() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    int n = session.CreateQuery($"DELETE FROM {nameof(DeletedPlayerItem)}").ExecuteUpdate();
                    transaction.Commit();
                    return n;
                }
            }
        }

        /// <summary>
        /// Delete a player item and all its stats
        /// </summary>
        public override void Remove(PlayerItem obj) {
            var azurePartition = obj.AzurePartition;
            var azureId = obj.AzureUuid;
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {

                    session.CreateQuery($"DELETE FROM {nameof(PlayerItemRecord)} WHERE {nameof(PlayerItemRecord.PlayerItemId)} = :id")
                        .SetParameter("id", obj.Id)
                        .ExecuteUpdate();


                    session.Delete(obj);

                    transaction.Commit();
                }

                // Mark item for deletion from the online backup
                if (!string.IsNullOrEmpty(azurePartition)) {
                    try {
                        using (ITransaction transaction = session.BeginTransaction()) {
                            session.SaveOrUpdate(new DeletedPlayerItem { Partition = azurePartition, Id = azureId });
                            transaction.Commit();
                        }
                    } catch (Exception ex) {
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

            foreach (var desiredClass in query.Classes) {
                queryFragments.Add("exists (select id_databaseitem from databaseitemstat_v2 dbs where stat IN ('augmentSkill1Extras','augmentSkill2Extras','augmentMastery1','augmentMastery2') "
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
                    AND {string.Join(" AND ", queryFragments)}
                )
                ";
                return new DatabaseItemStatQuery {
                    SQL = sql,
                    Parameters = queryParamsList
                };
            }

            return null;

        }

        public List<PlayerItem> SearchForItems(ItemSearchRequest query) {
            Logger.Debug($"Searching for items with query {query}");
            List<string> queryFragments = new List<string>();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(query.Wildcard)) {
                queryFragments.Add("LOWER(PI.namelowercase) LIKE :name");
                queryParams.Add("name", $"%{query.Wildcard.Replace(' ', '%').ToLower()}%");
            }

            // Filter by mod/hc
            queryFragments.Add("(LOWER(PI.Mod) = LOWER( :mod ) OR PI.Mod IS NULL)");
            queryParams.Add("mod", query.Mod);

            if (query.IsHardcore)
                queryFragments.Add("PI.IsHardcore");
            else
                queryFragments.Add("NOT PI.IsHardcore");

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
                sum(max(1,stackcount)) as StackCount, 
                rarity as Rarity, 
                levelrequirement as LevelRequirement, 
                baserecord as BaseRecord, 
                prefixrecord as PrefixRecord, 
                suffixrecord as SuffixRecord, 
                ModifierRecord as ModifierRecord, 
                MateriaRecord as MateriaRecord,
                {PlayerItemTable.PrefixRarity} as PrefixRarity,
                {PlayerItemTable.AzurePartition} as AzurePartition,
                {PlayerItemTable.AzureUuid} as AzureUuid,
                (SELECT Record FROM PlayerItemRecord pir WHERE pir.PlayerItemId = PI.Id AND NOT Record IN (PI.BaseRecord, PI.SuffixRecord, PI.MateriaRecord, PI.PrefixRecord)) AS PetRecord
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

            sql.Add(" GROUP BY Name, LevelRequirement COLLATE NOCASE");

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
                    List<PlayerItem> items = new List<PlayerItem>();
                    foreach (PlayerItem pi in q.List()) {
                        items.Add(pi);
                    }

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
                AND {PlayerItemTable.Seed} = :seed
                LIMIT 1
            ";

            var result = session.CreateSQLQuery(sql)
                .SetParameter("base", item.BaseRecord)
                .SetParameter("prefix", item.PrefixRecord)
                .SetParameter("suffix", item.SuffixRecord)
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
    }
}