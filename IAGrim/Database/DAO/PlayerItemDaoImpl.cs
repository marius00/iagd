using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Parser.Helperclasses;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IAGrim.Database.DAO.Table;

namespace IAGrim.Database {

    /// <summary>
    /// Database class for handling player owned items
    /// </summary>
    public class PlayerItemDaoImpl : BaseDao<PlayerItem>, IPlayerItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IPlayerItemDao));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        public PlayerItemDaoImpl(ISessionCreator sessionCreator, IDatabaseItemStatDao databaseItemStatDao) : base(sessionCreator) {
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
        /// Find an item based on it's online ID
        /// </summary>
        /// <returns></returns>
        public PlayerItem GetByOnlineId(long oid) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.QueryOver<PlayerItem>()
                        .Where(m => m.OnlineId == oid)
                        .SingleOrDefault();
                }
            }
        }
        

        private static float GetMinimumLevelForRecords(Dictionary<string, List<DBSTatRow>> stats, List<string> records) {

            var levels = stats.Where(m => records.Contains(m.Key))
                .SelectMany(m => m.Value.Where(v => v.Stat == "levelRequirement"))
                .Select(m => m.Value)
                .ToList();
            

            if (levels.Count == 0)
                return 0;
            else
                return (float)levels.Max<double>();
        }

        /// <summary>
        /// Calculate the item rarity for a given set of records (suffix, prefix, etc)
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private string GetRarityForRecords(Dictionary<string, List<DBSTatRow>> stats, List<string> records) {
            var classifications = stats.Where(m => records.Contains(m.Key))
                .SelectMany(m => m.Value.Where(v => v.Stat == "itemClassification"))
                .Select(m => m.TextValue);

            return TranslateClassification(classifications);
        }

        private string TranslateClassification(IEnumerable<string> classifications) {
            var enumerable = classifications as string[] ?? classifications.ToArray();
            if (enumerable.Contains("Legendary"))
                return "Epic";
            else if (enumerable.Contains("Epic"))
                return "Blue";
            else if (enumerable.Contains("Rare"))
                return "Green";
            else if (enumerable.Contains("Magical"))
                return "Yellow";
            else if (enumerable.Contains("Common"))
                return "White";
            else
                return "Unknown";
        }

        /// <summary>
        /// Get the "level of green" for a set of records
        /// An item with a white suffix and green prefix is "less than" an item with a green suffix and green prefix
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        private int GetGreenQualityLevelForRecords(Dictionary<string, List<DBSTatRow>> stats, List<string> records) {
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


        [Obsolete]
        private string GetSkillDisplayName(ISession session, string skillRecord) {

            var subquery = Subqueries.PropertyEq("P.Id", DetachedCriteria.For<DatabaseItem>()
                .Add(Restrictions.Eq("Record", skillRecord))
                .SetProjection(Projections.Property("Id"))
                );

            IList<DatabaseItemStat> skill = session.CreateCriteria<DatabaseItemStat>()
                .Add(subquery)
                .CreateAlias("Parent", "P")
                .List<DatabaseItemStat>();

            // Get the tag-name from the skill
            var skillNameEntry = skill.FirstOrDefault(m => m.Stat.Equals("skillDisplayName"));
            if (skillNameEntry?.TextValue != null)
                return skillNameEntry.TextValue;

            // It may be without a name, but reference a pet skill
            var petSkillName = skill.Where(m => m.Stat.Equals("petSkillName"));
            if (petSkillName.Any())
                return GetSkillDisplayName(session, petSkillName.FirstOrDefault().TextValue);

            // The pet skill may in turn reference a buff skill, or the original skill might.
            var buffSkillName = skill.Where(m => m.Stat.Equals("buffSkillName"));
            if (buffSkillName.Any())
                return GetSkillDisplayName(session, buffSkillName.FirstOrDefault().TextValue);

            return null;
        }

        /// <summary>
        /// Get a single item which has not been synchronized with online backups
        /// </summary>
        public PlayerItem GetSingleUnsynchronizedItem() {
            using (var session = SessionCreator.OpenSession()) {
                return session.QueryOver<PlayerItem>()
                    .Where(m => m.OnlineId == null)
                    .Take(1)
                    .SingleOrDefault();
            }
        }


        public long GetNumUnsynchronizedItems() {
            using (var session = SessionCreator.OpenSession()) {
                return session.QueryOver<PlayerItem>()
                    .Where(m => m.OnlineId == null)
                    .ToRowCountInt64Query()
                    .SingleOrDefault<long>();
            }
        }

        private IEnumerable<string> GetPetBonusRecords(Dictionary<String, List<DBSTatRow>> stats, List<string> records) {
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
        /// Update the "IsHardcore" flag depending on being an old Hard
        /// </summary>
        public void UpdateHardcoreSettings() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    bool value = (bool)Properties.Settings.Default.IsHardcore;
                    int n = session.CreateQuery("UPDATE PlayerItem SET IsHardcore = :val WHERE IsHardcore IS NULL")
                        .SetParameter("val", value)
                        .ExecuteUpdate();

                    if (n > 0) {
                        transaction.Commit();
                        Logger.InfoFormat("Updated the IsHardcore flag for {0} items to: {1}", n, value);
                    }
                }
            }
        }




        /// <summary>
        /// Calculate the name, rarity and stats for a player item and store it to DB
        /// </summary>
        /// <param name="item"></param>
        public override void Save(PlayerItem item) {
            Save(new List<PlayerItem> { item });
            if (item.OnlineId.HasValue) {
                Logger.Info($"Stored player item to database, OID: {item.OnlineId.Value}.");
            } else {
                Logger.Info("Stored player item to database.");
            }
        }



        public void Import(List<PlayerItem> items) {
            Logger.Debug($"Importing {items.Count} new items");

            // Attempt to exclude any items already owned, only applicable for users with online sync enabled
            IList<long> existingItems;
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    existingItems = session.CreateCriteria<PlayerItem>()
                        .Add(Restrictions.Gt(nameof(PlayerItem.OnlineId), 0L))
                        .SetProjection(Projections.Property(nameof(PlayerItem.OnlineId)))
                        .List<long>();
                }
            }

            Save(items.Where(m => !m.OnlineId.HasValue || !existingItems.Contains(m.OnlineId.Value)));
        }

        public override void Update(PlayerItem item) {
            Update(new List<PlayerItem> { item }, false);
        }

        public void Update(IList<PlayerItem> items, bool clearOnlineId) {

            var table = nameof(PlayerItem);
            var stack = nameof(PlayerItem.StackCount);
            var id = nameof(PlayerItem.Id);
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        if (clearOnlineId && item.OnlineId.HasValue) {
                            session.CreateSQLQuery($"INSERT OR IGNORE INTO {DeletedPlayerItemTable.Table} ({DeletedPlayerItemTable.Id}) VALUES (:id)")
                                .SetParameter("id", item.OnlineId.Value)
                                .ExecuteUpdate();

                            item.OnlineId = null;
                        }

                        session.CreateQuery($"UPDATE {table} SET {stack} = :count, {PlayerItemTable.OnlineId} = :oid WHERE {id} = :id")
                            .SetParameter("count", item.StackCount)
                            .SetParameter("id", item.Id)
                            .SetParameter("oid", item.OnlineId)
                            .ExecuteUpdate();
                    }
                    
                    // insert into DeletedPlayerItem(oid) select onlineid from playeritem where onlineid is not null and stackcount <= 0 and id in (1,2,3)
                    session.CreateSQLQuery($" INSERT OR IGNORE INTO {DeletedPlayerItemTable.Table} ({DeletedPlayerItemTable.Id}) " +
                                        $" SELECT {PlayerItemTable.OnlineId} FROM {PlayerItemTable.Table} " +
                                        $" WHERE {PlayerItemTable.OnlineId} IS NOT NULL " +
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
        }


        public int IncrementExistingStacksize(ISession session, long id, long stacksize) {
            string sc = nameof(PlayerItem.StackCount);
            string oid = nameof(PlayerItem.OnlineId);

            // Delete the online ID for this item (treated as a new one)
            long? onlineId = session.CreateCriteria<PlayerItem>()
                .Add(Restrictions.Eq(nameof(PlayerItem.Id), id))
                .SetMaxResults(1)
                .SetProjection(Projections.Property(nameof(PlayerItem.OnlineId)))
                .UniqueResult<long?>();

            if (onlineId.HasValue) {
                session.SaveOrUpdate(new DeletedPlayerItem { OID = onlineId.Value });
            }


            var query = $"UPDATE {nameof(PlayerItem)} SET {sc} = {sc} + :stacksize, {oid} = NULL WHERE {nameof(PlayerItem.Id)} = :id";
            return session.CreateQuery(query)
                .SetParameter("stacksize", stacksize)
                .SetParameter("id", id)
                .ExecuteUpdate();
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

        private void UpdatePetRecords(ISession session, PlayerItem item, Dictionary<String, List<DBSTatRow>> stats) {
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

        /*
        private void Save_Or_Update(ISession session, PlayerItem item, Dictionary<String, List<DBSTatRow>> stats, bool isUpdate){
            long? existingId = isUpdate ? null : GetIfExists(session, item);
            if (existingId.HasValue) {
                if (IncrementExistingStacksize(session, existingId.Value, item.StackCount) == 0) {
                    logger.Warn("Could not update stacksize, item loss detected");
                    throw new Exception("Could not update item stack, aborting to prevent item loss");
                }
            }
            else {
                var records = GetRecordsForItem(item);
                item.Rarity = GetRarityForRecords(stats, records);
                item.MinimumLevel = GetMinimumLevelForRecords(stats, records);
                //if (string.IsNullOrEmpty(item.Name))
                    UpdateItemName(session, stats, item);

                item.Mod = item.Mod?.ToLower();

                if (!isUpdate)
                    session.Save(item);
                else
                    session.Update(item);
            }
        }
        */
        private void UpdateItemDetails(ISession session, PlayerItem item, Dictionary<String, List<DBSTatRow>> stats) {
            var table = nameof(PlayerItem);
            var rarity = nameof(PlayerItem.Rarity);
            var levelReq = nameof(PlayerItem.LevelRequirement);
            var name = nameof(PlayerItem.Name);
            var id = nameof(PlayerItem.Id);
            var prefixRarity = PlayerItemTable.PrefixRarity;

            var records = GetRecordsForItem(item);
            session.CreateQuery($@"UPDATE {table} 
                    SET {name} = :name, {rarity} = :rarity, {levelReq} = :levelreq, {prefixRarity} = :prefixrarity 
                    WHERE {id} = :id"
                )
                .SetParameter("name", GetItemName(session, stats, item))
                .SetParameter("prefixrarity", GetGreenQualityLevelForRecords(stats, records))
                .SetParameter("rarity", GetRarityForRecords(stats, records))
                .SetParameter("levelreq", GetMinimumLevelForRecords(stats, records))
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

        public void DeleteAll() {

            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery("DELETE FROM PlayerItemStat").ExecuteUpdate();
                    session.CreateSQLQuery("DELETE FROM PlayerItem").ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Save a collection of player items
        /// Will update the internal item name before storing, taking in account any pre/suffix
        /// </summary>
        public override void Save(IEnumerable<PlayerItem> items_) {
            var items = new List<PlayerItem>(items_);

            Logger.Debug($"Storing {items.Count} new items");

            List<long> ids = new List<long>();
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    for (int i = 0; i < items.Count; i++) {
                        var item = items.ElementAt(i);

                        // Stack item if possible
                        long? existingId = GetIfExists(session, item);
                        if (existingId.HasValue) {
                            if (IncrementExistingStacksize(session, existingId.Value, item.StackCount) == 0) {
                                Logger.Warn("Could not update stacksize, item loss situation detected");
                                throw new Exception("Could not update item stack, aborting to prevent item loss");
                            }
                        }
                        else {
                            session.Save(item);
                            ids.Add(item.Id);
                        }

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
            long? onlineId = obj.OnlineId;
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateQuery($"DELETE FROM {nameof(PlayerItemStat)} WHERE {nameof(PlayerItemStat.PlayerItemId)} = :id")
                        .SetParameter("id", obj.Id)
                        .ExecuteUpdate();

                    session.CreateQuery($"DELETE FROM {nameof(PlayerItemRecord)} WHERE {nameof(PlayerItemRecord.PlayerItemId)} = :id")
                        .SetParameter("id", obj.Id)
                        .ExecuteUpdate();


                    session.Delete(obj);

                    transaction.Commit();
                }

                // Mark item for deletion from the online backup
                if (onlineId.HasValue) {
                    try {
                        using (ITransaction transaction = session.BeginTransaction()) {
                            session.SaveOrUpdate(new DeletedPlayerItem { OID = onlineId.Value });
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
                using (ITransaction transaction = session.BeginTransaction()) {
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

        /// <summary>
        /// Update the item name to include suffix/affix/etc
        /// </summary>
        private static string GetItemName(ISession session, Dictionary<String, List<DBSTatRow>> stats, PlayerItem item) {
            // Grab tags

            var records = new string[] { item.PrefixRecord, item.BaseRecord, item.SuffixRecord, item.MateriaRecord };
            var desiredTagsNames = new string[] { "lootRandomizerName", "itemNameTag", "itemQualityTag", "itemStyleTag", "description" };
            var relevants = stats.Where(m => records.Contains(m.Key));
            var tagEntries = relevants.SelectMany(m => m.Value.Where(v => desiredTagsNames.Contains(v.Stat)));
            

            // Grab tag values
            ICriteria tagCrit = session.CreateCriteria<ItemTag>();
            tagCrit.Add(Restrictions.In("Tag", tagEntries.Select(m => m.TextValue).ToArray()));
            var tags = tagCrit.List<ItemTag>();


            string prefix = string.Empty;
            {
                var prefixEntry = tagEntries.FirstOrDefault(m => m.Record == item.PrefixRecord && m.Stat == "lootRandomizerName");
                if (prefixEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == prefixEntry.TextValue);
                    prefix = tag?.Name ?? prefixEntry.TextValue;
                }
            }

            string suffix = string.Empty;
            {
                var suffixEntry = tagEntries.FirstOrDefault(m => m.Record == item.SuffixRecord && m.Stat == "lootRandomizerName");
                if (suffixEntry != null) {
                    suffix = tags.FirstOrDefault(m => m.Tag == suffixEntry.TextValue)?.Name ?? suffixEntry.TextValue;
                }
            }

            string core = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemNameTag");
                if (coreEntry == null) // Potions
                    coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "description");

                if (coreEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == coreEntry.TextValue);
                    core = tag?.Name ?? coreEntry.TextValue;
                }
            }

            string quality = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemQualityTag");
                if (coreEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == coreEntry.TextValue);
                    quality = tag?.Name ?? coreEntry.TextValue;
                }
            }

            string style = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemStyleTag");
                if (coreEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == coreEntry.TextValue);
                    style = tag?.Name ?? coreEntry.TextValue;
                }
            }

            string materia = string.Empty;
            {
                var entry = tagEntries.FirstOrDefault(m => m.Record == item.MateriaRecord && m.Stat == "description");
                if (entry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == entry.TextValue);
                    materia = tag?.Name ?? entry.TextValue;

                    materia = $" [{materia}]";
                }
            }


            string localizedName = GlobalSettings.Language.TranslateName(prefix, quality, style, core, suffix);
            return localizedName + materia;
        }


        class DatabaseItemStatQuery {
            public String SQL;
            public Dictionary<string, string[]> Parameters;
        }
        private static DatabaseItemStatQuery CreateDatabaseStatQueryParams(Search query) {
            List<string> queryFragments = new List<string>();
            Dictionary<string, string[]> queryParamsList = new Dictionary<string, string[]>();

            // Add the damage/resist filter
            foreach (string[] filter in query.filters) {
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

        public List<PlayerItem> SearchForItems(Search query) {
            Logger.Debug($"Searching for items with query {query}");
            List<string> queryFragments = new List<string>();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(query.Wildcard)) {
                queryFragments.Add("LOWER(PI.Name) LIKE :name");
                queryParams.Add("name", string.Format("%{0}%", query.Wildcard.Replace(' ', '%').ToLower()));
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

            sql.Add(" GROUP BY Name, LevelRequirement");

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

        public void ClearAllItemStats() {
            // Clear existing stats
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateQuery("DELETE FROM PlayerItemStat").ExecuteUpdate();
                    transaction.Commit();
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
    }
}