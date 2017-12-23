using System;
using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;
using IAGrim.BuddyShare.dto;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.Dto;
using IAGrim.Utilities;
using NHibernate.Linq;
using NHibernate.Transform;

namespace IAGrim.Database {
    public class BuddyItemDaoImpl : BaseDao<BuddyItem>, IBuddyItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BuddyItemDaoImpl));

        public BuddyItemDaoImpl(ISessionCreator sessionCreator) : base(sessionCreator) {
            //
        }

        public void RemoveBuddy(long buddyId) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery($"DELETE FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.BuddyId} = :id")
                        .SetParameter("id", buddyId)
                        .ExecuteUpdate();

                    session.CreateQuery("DELETE FROM BuddyStash WHERE UserId = :id")
                        .SetParameter("id", buddyId)
                        .ExecuteUpdate();

                    session.CreateQuery("DELETE FROM BuddySubscription WHERE Id = :id")
                        .SetParameter("id", buddyId)
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }


        private class NameRow {
            public string Record { get; set; }
            public string Stat { get; set; }
            public string Text { get; set; }
        }

        private static string GetName(BuddyItem item, ICollection<NameRow> rows) {
            // Grab tags

            string prefix = rows.FirstOrDefault(m => m.Record == item.PrefixRecord && m.Stat == "lootRandomizerName")?.Text ?? string.Empty;
            string suffix = rows.FirstOrDefault(m => m.Record == item.SuffixRecord && m.Stat == "lootRandomizerName")?.Text ?? string.Empty;
            string core = rows.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemNameTag")?.Text ?? string.Empty;
            if (string.IsNullOrEmpty(core)) {
                core = rows.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "description")?.Text ?? string.Empty;
            }

            string quality = rows.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemQualityTag")?.Text ?? string.Empty;
            string style = rows.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemStyleTag")?.Text ?? string.Empty;
            string materia = rows.FirstOrDefault(m => m.Record == item.MateriaRecord && m.Stat == "description")?.Text ?? string.Empty;
            if (!string.IsNullOrEmpty(materia)) {
                materia = $" [{materia}]";
            }


            string localizedName = GlobalSettings.Language.TranslateName(prefix, quality, style, core, suffix);
            return localizedName + materia;
        }

        public void UpdateNames(IList<BuddyItem> items) {
            Logger.Debug("Updating item names");

            string sql = $@"
                SELECT {DatabaseItemTable.Record} as Record, {DatabaseItemStatTable.Stat} as Stat, tags.{ItemTagTable.Name} as Text
                FROM {DatabaseItemTable.Table} item, {DatabaseItemStatTable.Table} stat, {ItemTagTable.Table} tags
                WHERE item.{DatabaseItemTable.Id} = stat.{DatabaseItemStatTable.Item}
                AND {DatabaseItemStatTable.TextValue} = {ItemTagTable.Id}
                AND {DatabaseItemStatTable.Stat} IN ('lootRandomizerName', 'itemNameTag', 'itemQualityTag', 'itemStyleTag', 'description')
                AND ({DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.BaseRecord} FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.Name} IS NULL OR {BuddyItemsTable.Name} = '')
		            OR {DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.PrefixRecord} FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.Name} IS NULL OR {BuddyItemsTable.Name} = '')
		            OR {DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.SuffixRecord} FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.Name} IS NULL OR {BuddyItemsTable.Name} = '')
                )";


            IList<NameRow> rows;
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    rows = session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<NameRow>())
                        .List<NameRow>();
                }
            }

            Logger.Debug("Updating the names for buddy items");
            string updateSql = $"UPDATE {BuddyItemsTable.Table} SET {BuddyItemsTable.Name} = :name WHERE {BuddyItemsTable.Id} = :id";
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        var name = GetName(item, rows);
                        if (!string.IsNullOrEmpty(name)) {
                            session.CreateSQLQuery(updateSql)
                                .SetParameter("id", item.Id)
                                .SetParameter("name", name)
                                .ExecuteUpdate();
                        }
                    }

                    transaction.Commit();
                }
            }
            Logger.Debug("Names updated");
        }

        private static int GetLevelRequirement(BuddyItem item, IEnumerable<LevelRequirementRow> rows) {
            return (int)rows.Where(row => row.Record == item.BaseRecord
                                     || row.Record == item.PrefixRecord
                                     || row.Record == item.SuffixRecord)
                .OrderByDescending(row => row.LevelRequirement)
                .Select(row => row.LevelRequirement)
                .FirstOrDefault();
        }

        public void UpdateLevelRequirements(IList<BuddyItem> items) {
            Logger.Debug("Updating item level requirements");

            string sql = $@"
                SELECT {DatabaseItemTable.Record} as Record, {DatabaseItemStatTable.Value} as LevelRequirement 
                FROM {DatabaseItemTable.Table} item, {DatabaseItemStatTable.Table} stat
                WHERE item.{DatabaseItemTable.Id} = stat.{DatabaseItemStatTable.Item}
                AND {DatabaseItemStatTable.Stat} = 'levelRequirement'
                AND ({DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.BaseRecord} FROM {BuddyItemsTable.Table})
		            OR {DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.PrefixRecord} FROM {BuddyItemsTable.Table})
		            OR {DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.SuffixRecord} FROM {BuddyItemsTable.Table})
                )";

            IList<LevelRequirementRow> rows;
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    rows = session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<LevelRequirementRow>())
                        .List<LevelRequirementRow>();
                }
            }

            Logger.Debug("Updating the level requirements for buddy items");
            string updateSql = $"UPDATE {BuddyItemsTable.Table} SET {BuddyItemsTable.LevelRequirement} = :requirement WHERE {BuddyItemsTable.Id} = :id";
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        var requirement = GetLevelRequirement(item, rows);
                        session.CreateSQLQuery(updateSql)
                            .SetParameter("id", item.Id)
                            .SetParameter("requirement", requirement)
                            .ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
            Logger.Debug("Level requirements updated");
        }

        private class LevelRequirementRow {
            public string Record { get; set; }
            public double LevelRequirement { get; set; }
        }

        private class RarityRow {
            public string Record { get; set; }
            public string Rarity { get; set; }
        }

        private static Dictionary<string, int> _rarityMap = new Dictionary<string, int> {
            {"Legendary", 6},
            {"Epic", 5},
            {"Rare", 4},
            {"Quest", 3},
            {"Magical", 2},
        };

        private static Dictionary<string, string> _rarityTranslations = new Dictionary<string, string> {
            {"Legendary", "Epic"},
            {"Epic", "Blue"},
            {"Rare", "Green"},
            {"Quest", "Green"},
            {"Magical", "Yellow"},
        };

        private static int ClassifyRarity(string rarity) {
            if (_rarityMap.ContainsKey(rarity))
                return _rarityMap[rarity];
            else {
                return -1;
            }
        }
        private static string TranslateRarity(string rarity) {
            if (string.IsNullOrEmpty(rarity))
                return "White";

            if (_rarityTranslations.ContainsKey(rarity))
                return _rarityTranslations[rarity];

            return "White";
            
        }


        private static string GetRarity(BuddyItem item, IEnumerable<RarityRow> rows) {
            return rows.Where(row => row.Record == item.BaseRecord 
                                    || row.Record == item.PrefixRecord 
                                    || row.Record == item.SuffixRecord)
                .OrderByDescending(row => ClassifyRarity(row.Rarity))
                .Select(row => TranslateRarity(row.Rarity))
                .FirstOrDefault();
        }

        public void UpdateRarity(IList<BuddyItem> items) {
            // This will need a custom table, mapping 'yellow' to priority 0 and the like
            Logger.Debug("Updating item rarities");

            // TODO: Change player item to buddy item
            string sql = $@"
                SELECT {DatabaseItemTable.Record} as Record, {DatabaseItemStatTable.TextValue} as Rarity 
                FROM {DatabaseItemTable.Table} item, {DatabaseItemStatTable.Table} stat
                WHERE item.{DatabaseItemTable.Id} = stat.{DatabaseItemStatTable.Item}
                AND {DatabaseItemStatTable.Stat} = 'itemClassification'
                AND ({DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.BaseRecord} FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.Rarity} IS NULL)
		            OR {DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.PrefixRecord} FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.Rarity} IS NULL)
		            OR {DatabaseItemTable.Record} IN (SELECT {BuddyItemsTable.SuffixRecord} FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.Rarity} IS NULL)
                )";

            IList<RarityRow> rows;
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    rows = session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<RarityRow>())
                        .List<RarityRow>();
                }
            }
            
            Logger.Debug("Updating the rarity for buddy items");
            string updateSql = $"UPDATE {BuddyItemsTable.Table} SET {BuddyItemsTable.Rarity} = :rarity WHERE {BuddyItemsTable.Id} = :id";
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        string rarity = GetRarity(item, rows);
                        session.CreateSQLQuery(updateSql)
                            .SetParameter("id", item.Id)
                            .SetParameter("rarity", rarity)
                            .ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
            Logger.Debug("Rarities updated");
        }

        /// <summary>
        /// Transformers.AliasToBean<BuddyItem>() has simply been too unreliable
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private BuddyItem ToDomainObject(object ob) {
            object[] obj = (object[]) ob;
            var count = (long) obj[10];
            var minimumLevel = obj[8] as float?;
            if (!minimumLevel.HasValue) {
                minimumLevel = obj[8] as long?;
            }
            
            var id = (long) obj[9];
            return new BuddyItem {
                BaseRecord = obj[0] as string,
                PrefixRecord = obj[1] as string,
                SuffixRecord = obj[2] as string,
                ModifierRecord = obj[3] as string,
                TransmuteRecord = obj[4] as string,
                MateriaRecord = obj[5] as string,
                Rarity = obj[6] as string,
                Name = obj[7] as string,
                MinimumLevel = minimumLevel ?? 0,
                Stash = obj[11] as string,
                Id = id,
                Count = (uint)count
            };
        }

        private IList<BuddyItem> ListAll(string where) {
            if (string.IsNullOrEmpty(where))
                where = "WHERE 1=1";
            var sql = $@"SELECT {BuddyItemsTable.BaseRecord} as BaseRecord,
                                {BuddyItemsTable.PrefixRecord} as PrefixRecord,
                                {BuddyItemsTable.SuffixRecord} as SuffixRecord,
                                {BuddyItemsTable.ModifierRecord} as ModifierRecord,
                                {BuddyItemsTable.TransmuteRecord} as TransmuteRecord,
                                {BuddyItemsTable.MateriaRecord} as MateriaRecord,
                                {BuddyItemsTable.Rarity} as Rarity,
                                {BuddyItemsTable.Name} as Name,
                                {BuddyItemsTable.LevelRequirement} as MinimumLevel,
                                {BuddyItemsTable.Id} as Id,
                                MAX(1, {BuddyItemsTable.StackCount}) as Count,
                                {BuddyStashTable.Name} as Buddy
                        FROM {BuddyItemsTable.Table}, {BuddyStashTable.Table}
                        {where}
                        AND {BuddyItemsTable.BuddyId} = {BuddyStashTable.User}
            ";

            
            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateSQLQuery(sql)
                    .List<Object>()
                    .Select(ToDomainObject)
                    .ToList();
            }
        }

        public IList<BuddyItem> ListItemsWithMissingRarity() {
            return ListAll($"WHERE {BuddyItemsTable.Rarity} IS NULL");
        }

        public IList<BuddyItem> ListItemsWithMissingName() {
            return ListAll($"WHERE {BuddyItemsTable.Name} IS NULL");
        }

        public IList<BuddyItem> ListItemsWithMissingLevelRequirement() {
            return ListAll($"WHERE {BuddyItemsTable.LevelRequirement} IS NULL OR {BuddyItemsTable.LevelRequirement} < 1");
        }

        public override IList<BuddyItem> ListAll() {
            return ListAll(string.Empty);
        }

        public void SetItems(long userid, string description, List<JsonBuddyItem> items) {
            string sql = string.Join(" ", 
                $"INSERT INTO {BuddyItemsTable.Table} (",
                    $"{BuddyItemsTable.BuddyId}, ",
                    $"{BuddyItemsTable.RemoteItemId}, ",
                    $"{BuddyItemsTable.BaseRecord}, ",
                    $"{BuddyItemsTable.PrefixRecord},",
                    $"{BuddyItemsTable.SuffixRecord},",
                    $"{BuddyItemsTable.ModifierRecord},",
                    $"{BuddyItemsTable.TransmuteRecord},",
                    $"{BuddyItemsTable.MateriaRecord},",
                    $"{BuddyItemsTable.StackCount},",
                    $"{BuddyItemsTable.Mod}) VALUES (:buddy, :remoteId, :base, :pre, :suff, :modif, :transmute, :materia, :stacksize, :mod);");

            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {

                    IList<long> existingItemsForBuddy = session.CreateSQLQuery($"SELECT {BuddyItemsTable.RemoteItemId} FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.BuddyId} = :buddy")
                        .SetParameter("buddy", userid)
                        .List<long>();

                    var newItems = items.Where(m => !existingItemsForBuddy.Contains(m.Id));
                    var deletedItems = existingItemsForBuddy.Where(m => items.All(item => item.Id != m)).ToList();

                    Logger.Debug($"Deleting existing buddy items for {userid}");
                    const int deleteBatchSize = 800;
                    for (int i = 0; i < 1 + deletedItems.Count / deleteBatchSize; i++) {
                        var batch = deletedItems.Skip(i * deleteBatchSize).Take(deleteBatchSize).ToList();
                        if (batch.Count > 0) {
                            session.CreateSQLQuery($"DELETE FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.RemoteItemId} IN ( :items )")
                                .SetParameterList("items", batch)
                                .ExecuteUpdate();
                        }
                    }

                    
                    session.CreateSQLQuery($"DELETE FROM {BuddyItemRecordTable.Table} WHERE NOT {BuddyItemRecordTable.Item} IN (SELECT {BuddyItemsTable.Id} FROM {BuddyItemsTable.Table})")
                        .ExecuteUpdate();



                    Logger.Debug($"Adding {items.Count} items for buddy {userid}");
                    foreach (var item in newItems) {
                        session.CreateSQLQuery(sql)
                            .SetParameter("buddy", userid)
                            .SetParameter("remoteId", item.Id)
                            .SetParameter("base", item.BaseRecord)
                            .SetParameter("pre", item.PrefixRecord)
                            .SetParameter("suff", item.SuffixRecord)
                            .SetParameter("modif", item.ModifierRecord)
                            .SetParameter("transmute", item.TransmuteRecord)
                            .SetParameter("materia", item.MateriaRecord)
                            .SetParameter("stacksize", item.StackCount)
                            .SetParameter("mod", item.Mod)
                            .ExecuteUpdate();
                    }

                    session.SaveOrUpdate(new BuddyStash {
                        UserId = userid,
                        Description = description
                    });

                    transaction.Commit();
                }
            }


            var allItems = ListAll($"WHERE NOT {BuddyItemsTable.Id} IN (SELECT {BuddyItemRecordTable.Item} FROM {BuddyItemRecordTable.Table})");
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in allItems) {
                        foreach (var record in new[] {item.BaseRecord, item.PrefixRecord, item.SuffixRecord, item.MateriaRecord}) {
                            if (!string.IsNullOrEmpty(record)) {
                                session.CreateSQLQuery(
                                        $@"INSERT OR IGNORE INTO {BuddyItemRecordTable.Table} ({BuddyItemRecordTable.Item}, {BuddyItemRecordTable.Record}, {BuddyItemRecordTable.BuddyId}) 
                                                VALUES (:id, :record, :buddy)")
                                    .SetParameter("id", item.Id)
                                    .SetParameter("record", record)
                                    .SetParameter("buddy", userid)
                                    .ExecuteUpdate();
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
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
                queryFragments.Add(
                    $@"EXISTS (SELECT {DatabaseItemStatTable.Item} FROM {DatabaseItemStatTable.Table} dbs 
                    WHERE {DatabaseItemStatTable.Stat} in ( :filter_{filter.GetHashCode()} ) 
                    AND db.{DatabaseItemTable.Id} = dbs.{DatabaseItemStatTable.Item})"
                );
                queryParamsList.Add($"filter_{filter.GetHashCode()}", filter);
            }


            if (query.IsRetaliation) {
                queryFragments.Add(
                    $@"EXISTS (SELECT {DatabaseItemStatTable.Item} FROM {DatabaseItemStatTable.Table} dbs
                    WHERE {DatabaseItemStatTable.Stat} LIKE 'retaliation%' 
                    AND db.{DatabaseItemTable.Id} = dbs.{DatabaseItemStatTable.Item})");
            }

            foreach (var desiredClass in query.Classes) {
                queryFragments.Add(
                    $@"EXISTS (SELECT {DatabaseItemStatTable.Item} FROM {DatabaseItemStatTable.Table} dbs 
                        WHERE {DatabaseItemStatTable.Stat} IN ('augmentSkill1Extras','augmentSkill2Extras','augmentMastery1','augmentMastery2') 
                        AND {DatabaseItemStatTable.TextValue} = '{desiredClass}'" // Not ideal
                        + $" AND db.{DatabaseItemTable.Id} = dbs.{DatabaseItemStatTable.Item})"
                );
            }



            // Can be several slots for stuff like "2 Handed"
            if (query.Slot?.Length > 0) {
                queryFragments.Add($@"
                    EXISTS (SELECT {DatabaseItemStatTable.Item} FROM {DatabaseItemStatTable.Table} dbs 
                    WHERE {DatabaseItemStatTable.Stat} = 'Class' 
                    AND {DatabaseItemStatTable.TextValue} IN ( :class ) 
                    AND db.{DatabaseItemTable.Id} = dbs.{DatabaseItemStatTable.Item})"
                );
                queryParamsList.Add("class", query.Slot);
            }

            if (queryFragments.Count > 0) {
                string sql = $"SELECT {BuddyItemRecordTable.Item} FROM {BuddyItemRecordTable.Table} WHERE {BuddyItemRecordTable.Record} IN (";
                sql += $@"select {DatabaseItemTable.Record} FROM {DatabaseItemTable.Table} db 
                        WHERE db.{DatabaseItemTable.Record} IN (
                                      SELECT {BuddyItemsTable.BaseRecord} FROM {BuddyItemsTable.Table}
                                UNION SELECT {BuddyItemsTable.PrefixRecord} FROM {BuddyItemsTable.Table} 
                                UNION SELECT {BuddyItemsTable.SuffixRecord} FROM {BuddyItemsTable.Table} 
                                UNION SELECT {BuddyItemsTable.MateriaRecord} FROM {BuddyItemsTable.Table}
                        ) ";
                sql += " AND " + string.Join(" AND ", queryFragments); 
                sql += ")";
                return new DatabaseItemStatQuery {
                    SQL = sql,
                    Parameters = queryParamsList
                };
            }

            return null;

        }

        class BuddyItemDto {
            public long Id { get; set; }

            public float MinimumLevel { get; set; }
            public string Rarity { get; set; }
            public string BaseRecord { get; set; }
            public string PrefixRecord { get; set; }
            public string SuffixRecord { get; set; }
            public string ModifierRecord { get; set; }
            public string TransmuteRecord { get; set; }
            public long BuddyId { get; set; }
            public virtual string MateriaRecord { get; set; }
            public virtual string EnchantmentRecord { get; set; }
            public uint Count { get; set; }
        }

        public IList<BuddyItem> FindBy(Search query) {
            List<string> queryFragments = new List<string>();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(query.Wildcard)) {
                queryFragments.Add($"{BuddyItemsTable.Name} LIKE :name");
                queryParams.Add("name", $"%{query.Wildcard.Replace(' ', '%')}%");
            }


            queryFragments.Add($"(LOWER({BuddyItemsTable.Mod}) = LOWER( :mod ) OR {BuddyItemsTable.Mod} IS NULL)");
            queryParams.Add("mod", query.Mod);

            /*
            if (query.IsHardcore)
                queryFragments.Add("PI.IsHardcore");
            else
                queryFragments.Add("NOT PI.IsHardcore");
            */


            if (!string.IsNullOrEmpty(query.Rarity)) {
                queryFragments.Add($"{BuddyItemsTable.Rarity} = :rarity");
                queryParams.Add("rarity", query.Rarity);
            }

            // Add the MINIMUM level requirement (if any)
            if (query.MinimumLevel > 0) {
                queryFragments.Add($"{BuddyItemsTable.LevelRequirement} >= :minlevel");
                queryParams.Add("minlevel", query.MinimumLevel);
            }

            // Add the MAXIMUM level requirement (if any)
            if (query.MaximumLevel < 120 && query.MaximumLevel > 0) {
                queryFragments.Add($"{BuddyItemsTable.LevelRequirement} <= :maxlevel");
                queryParams.Add("maxlevel", query.MaximumLevel);
            }


            List<string> sql = new List<string>();
            sql.Add($@"SELECT
                                {BuddyItemsTable.BaseRecord} as BaseRecord,
                                {BuddyItemsTable.PrefixRecord} as PrefixRecord,
                                {BuddyItemsTable.SuffixRecord} as SuffixRecord,
                                {BuddyItemsTable.ModifierRecord} as ModifierRecord,
                                {BuddyItemsTable.TransmuteRecord} as TransmuteRecord,
                                {BuddyItemsTable.MateriaRecord} as MateriaRecord,
                                {BuddyItemsTable.Rarity} as Rarity,
                                {BuddyItemsTable.Name} as Name,
                                {BuddyItemsTable.LevelRequirement} as MinimumLevel,
                                {BuddyItemsTable.Id} as Id,
                                SUM(MAX(1, {BuddyItemsTable.StackCount})) as Count,
                                S.{BuddyStashTable.Name} as Stash


                FROM {BuddyItemsTable.Table} PI, {BuddyStashTable.Table} S WHERE " + string.Join(" AND ", queryFragments)
                + $" AND {BuddyItemsTable.BuddyId} = {BuddyStashTable.User} "
                );

            var subquery = CreateDatabaseStatQueryParams(query);
            if (subquery != null) {
                sql.Add($" AND PI.{BuddyItemsTable.Id} IN (" + subquery.SQL + ")");
            }
            sql.Add($" GROUP BY {BuddyItemsTable.Name}, {BuddyItemsTable.LevelRequirement}");



            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    Logger.Debug(string.Join(" ", sql));
                    var q = session.CreateSQLQuery(string.Join(" ", sql));
                    q.AddScalar("BaseRecord", NHibernateUtil.String);
                    q.AddScalar("PrefixRecord", NHibernateUtil.String);
                    q.AddScalar("SuffixRecord", NHibernateUtil.String);
                    q.AddScalar("ModifierRecord", NHibernateUtil.String);
                    q.AddScalar("TransmuteRecord", NHibernateUtil.String);
                    q.AddScalar("MateriaRecord", NHibernateUtil.String);
                    q.AddScalar("Rarity", NHibernateUtil.String);
                    q.AddScalar("Name", NHibernateUtil.String);
                    q.AddScalar("MinimumLevel", NHibernateUtil.UInt32);
                    q.AddScalar("Id", NHibernateUtil.Int64);
                    q.AddScalar("Count", NHibernateUtil.UInt32);
                    q.AddScalar("Stash", NHibernateUtil.String);

                    foreach (var key in queryParams.Keys) {
                        q.SetParameter(key, queryParams[key]);
                        Logger.Debug($"{key}: " + queryParams[key]);
                    }

                    if (subquery != null) {
                        foreach (var key in subquery.Parameters.Keys) {
                            q.SetParameterList(key, subquery.Parameters[key]);
                            Logger.Debug($"{key}: " + string.Join(",", subquery.Parameters[key]));
                        }
                    }

                    Logger.Debug(q.QueryString);
                    q.SetResultTransformer(Transformers.AliasToBean<BuddyItem>());
                    var result = q.List<BuddyItem>();
                    
                    // stacksize is correct.. record is not
                    Logger.Debug($"Search returned {result.Count} items");
                    return result;
                }
            }
        }
    }
}
