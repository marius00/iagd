
using IAGrim.Database.DAO.Table;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using log4net;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using EvilsoftCommons;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.DAO;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Services.Dto;
using NHibernate.Criterion;

namespace IAGrim.Database {
    public class BuddyItemDaoImpl : BaseDao<BuddyItem>, IBuddyItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BuddyItemDaoImpl));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;

        public BuddyItemDaoImpl(ISessionCreator sessionCreator, IDatabaseItemStatDao databaseItemStatDao, SqlDialect dialect) : base(sessionCreator, dialect) {
            _databaseItemStatDao = databaseItemStatDao;
        }

        public void RemoveBuddy(long buddyId) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery($"DELETE FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.SubscriptionId} = :id")
                        .SetParameter("id", buddyId)
                        .ExecuteUpdate();

                    session.CreateSQLQuery($"DELETE FROM {BuddySubscriptionTable.Table} WHERE {BuddySubscriptionTable.Id} = :id")
                        .SetParameter("id", buddyId)
                        .ExecuteUpdate();

                    // Remove record from records table (lookup table)
                    session.CreateSQLQuery($"DELETE FROM {BuddyItemRecordTable.Table} WHERE NOT {BuddyItemRecordTable.Item} IN (SELECT {BuddyItemsTable.RemoteItemId} FROM {BuddyItemsTable.Table})")
                        .ExecuteUpdate();

                    // Replica stats
                    session.CreateSQLQuery($"DELETE FROM BuddyReplicaItem WHERE NOT buddyitemid IN (SELECT {BuddyItemsTable.RemoteItemId} FROM {BuddyItemsTable.Table})")
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }

        public long GetNumItems(long subscriptionId) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    var numItems = session
                        .CreateSQLQuery(
                            $"SELECT COUNT(*) FROM {BuddyItemsTable.Table} WHERE {BuddyItemsTable.SubscriptionId} = :id")
                        .SetParameter("id", subscriptionId)
                        .UniqueResult<long>();

                    return numItems;
                }
            }
        }

        public IList<string> GetOnlineIds(BuddySubscription subscription) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateCriteria<BuddyItem>()
                        .Add(Restrictions.Eq(nameof(BuddyItem.BuddyId), subscription.Id))
                        .SetProjection(Projections.Property(nameof(BuddyItem.RemoteItemId)))
                        .List<string>();
                }
            }
        }

        public void Save(BuddySubscription subscription, List<BuddyItem> items) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        item.BuddyId = subscription.Id;
                        session.Save(item);
                    }


                    // Update / Insert records into lookup table
                    foreach (var item in items) {
                        foreach (var record in new[] {item.BaseRecord, item.PrefixRecord, item.SuffixRecord, item.MateriaRecord}) {
                            if (!string.IsNullOrEmpty(record)) {
                                session.CreateSQLQuery(
                                        $@"INSERT OR IGNORE INTO {BuddyItemRecordTable.Table} ({BuddyItemRecordTable.Item}, {BuddyItemRecordTable.Record}) 
                                                VALUES (:id, :record)")
                                    .SetParameter("id", item.RemoteItemId)
                                    .SetParameter("record", record)
                                    .ExecuteUpdate();
                            }
                        }
                    }

                    transaction.Commit();
                }

                UpdatePetRecords(session, items);
            }
        }


        public void UpdatePetRecords(ISession session, List<BuddyItem> items) {
            using (ITransaction transaction = session.BeginTransaction()) {
                // Now that we have base stats, we can calculate pet records as well
                var stats = _databaseItemStatDao.GetStats(session, StatFetch.BuddyItems);
                for (int i = 0; i < items.Count; i++) {
                    UpdatePetRecords(session, items.ElementAt(i), stats);
                }
                transaction.Commit();
            }
        }


        private void UpdatePetRecords(ISession session, BuddyItem item, Dictionary<string, List<DBStatRow>> stats) {
            var records = PlayerItemDaoImpl.GetRecordsForItem(item);

            var table = BuddyItemRecordTable.Table;
            var id = BuddyItemRecordTable.Item;
            var rec = BuddyItemRecordTable.Record;
            foreach (var record in PlayerItemDaoImpl.GetPetBonusRecords(stats, records)) {
                Logger.Debug($"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES ({item.RemoteItemId}, {record})");
                session.CreateSQLQuery(SqlUtils.EnsureDialect(Dialect, $"INSERT OR IGNORE INTO {table} ({id}, {rec}) VALUES (:id, :record)"))
                    .SetParameter("id", item.RemoteItemId)
                    .SetParameter("record", record)
                    .ExecuteUpdate();
            }
        }

        public void Delete(BuddySubscription subscription, List<DeleteItemDto> items) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        session.CreateSQLQuery($@"DELETE FROM {BuddyItemsTable.Table} 
                                WHERE {BuddyItemsTable.RemoteItemId} = :cloudId
                                AND {BuddyItemsTable.SubscriptionId} = :subscriptionId")
                            .SetParameter("cloudId", item.Id)
                            .SetParameter("subscriptionId", subscription.Id)
                            .ExecuteUpdate();
                    }

                    // Remove record from records table (lookup table)
                    session.CreateSQLQuery($"DELETE FROM {BuddyItemRecordTable.Table} WHERE NOT {BuddyItemRecordTable.Item} IN (SELECT {BuddyItemsTable.RemoteItemId} FROM {BuddyItemsTable.Table})")
                        .ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }

        public IList<BuddyItem> ListMissingReplica(int limit) {
            // TODO: Relics are "ItemArtifact", those will crash the game.
            var specificItemTypesOnlySql = $@"
                
                    select baserecord from databaseitem_V2 db where db.baserecord in (
						select baserecord from buddyitems_v6
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
                            'WeaponMelee_Sword2h',
                            'WeaponMelee_Mace2h',
                            'WeaponMelee_Axe2h',
                            'WeaponHunting_Ranged1h',
                            'WeaponHunting_Ranged2h',
                            'WeaponArmor_Offhand',
                            'WeaponArmor_Shield'
                        ) 
                        AND db.id_databaseitem = dbs.id_databaseitem
                    )
                ";


            var sql = $@"SELECT

                                {BuddyItemsTable.BaseRecord} as BaseRecord,
                                {BuddyItemsTable.PrefixRecord} as PrefixRecord,
                                {BuddyItemsTable.SuffixRecord} as SuffixRecord,
                                {BuddyItemsTable.ModifierRecord} as ModifierRecord,
                                {BuddyItemsTable.TransmuteRecord} as TransmuteRecord,
                                {BuddyItemsTable.MateriaRecord} as MateriaRecord,
                                {BuddyItemsTable.Rarity} as Rarity,
                                {BuddyItemsTable.PrefixRarity} as PrefixRarity,
                                {BuddyItemsTable.Name} as Name,
                                {BuddyItemsTable.Seed} as Seed,
                                {BuddyItemsTable.RelicSeed} as RelicSeed,
                                {BuddyItemsTable.EnchantmentSeed} as EnchantmentSeed,
 
                                {BuddyItemsTable.LevelRequirement} as MinimumLevel,
                                {BuddyItemsTable.RemoteItemId} as RemoteItemId,
                                {BuddyItemsTable.StackCount} as Count,
                                {BuddyItemsTable.SubscriptionId} as BuddyId,
                                S.{BuddySubscriptionTable.Nickname} as Stash


                FROM {BuddyItemsTable.Table} PI, {BuddySubscriptionTable.Table} S
                WHERE PI.{BuddyItemsTable.RemoteItemId} NOT IN (SELECT R.BuddyItemId FROM BuddyReplicaItem R)
                AND MOD = '' 

                AND PI.{BuddyItemsTable.BaseRecord} IN ({specificItemTypesOnlySql})
                AND {BuddyItemsTable.SubscriptionId} = {BuddySubscriptionTable.Id}

                order by RANDOM ()
                LIMIT :limit ";

            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    Logger.Debug(string.Join(" ", sql));
                    var q = session.CreateSQLQuery(string.Join(" ", sql));
                    q.AddScalar("BaseRecord", NHibernateUtil.String);
                    q.AddScalar("PrefixRecord", NHibernateUtil.String);
                    q.AddScalar("SuffixRecord", NHibernateUtil.String);
                    q.AddScalar("ModifierRecord", NHibernateUtil.String);
                    q.AddScalar("PrefixRarity", NHibernateUtil.Int64);
                    q.AddScalar("TransmuteRecord", NHibernateUtil.String);
                    q.AddScalar("MateriaRecord", NHibernateUtil.String);
                    q.AddScalar("Rarity", NHibernateUtil.String);
                    q.AddScalar("Name", NHibernateUtil.String);
                    q.AddScalar("MinimumLevel", NHibernateUtil.UInt32);
                    q.AddScalar("Id", NHibernateUtil.Int64);
                    q.AddScalar("Count", NHibernateUtil.UInt32);
                    q.AddScalar("Stash", NHibernateUtil.String);
                    q.AddScalar("BuddyId", NHibernateUtil.Int64);
                    q.AddScalar("RemoteItemId", NHibernateUtil.String);
                    q.AddScalar("PetRecord", NHibernateUtil.String);

                    q.AddScalar("Seed", NHibernateUtil.Int64);
                    q.AddScalar("RelicSeed", NHibernateUtil.Int64);
                    q.AddScalar("EnchantmentSeed", NHibernateUtil.Int64);


                    Logger.Debug(q.QueryString);
                    q.SetResultTransformer(Transformers.AliasToBean<BuddyItem>());
                    q.SetParameter("limit", limit);
                    var result = q.List<BuddyItem>();

                    return result;
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

            string localizedName = RuntimeSettings.Language.TranslateName(prefix, quality, style, core, suffix);
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

            string updateSql = $"UPDATE {BuddyItemsTable.Table} SET {BuddyItemsTable.Name} = :name, {BuddyItemsTable.NameLowercase} = :nameLowercase WHERE {BuddyItemsTable.RemoteItemId} = :id";
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        var name = GetName(item, rows);
                        if (!string.IsNullOrEmpty(name)) {
                            session.CreateSQLQuery(updateSql)
                                .SetParameter("id", item.RemoteItemId)
                                .SetParameter("name", name)
                                .SetParameter("nameLowercase", name.ToLowerInvariant())
                                .ExecuteUpdate();
                        }
                    }

                    transaction.Commit();
                }
            }

            Logger.Debug("Names updated");
        }

        private static int GetLevelRequirement(BuddyItem item, IEnumerable<LevelRequirementRow> rows) {
            return (int) rows.Where(row => row.Record == item.BaseRecord
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
            string updateSql = $"UPDATE {BuddyItemsTable.Table} SET {BuddyItemsTable.LevelRequirement} = :requirement WHERE {BuddyItemsTable.RemoteItemId} = :id";
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        var requirement = GetLevelRequirement(item, rows);
                        session.CreateSQLQuery(updateSql)
                            .SetParameter("id", item.RemoteItemId)
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

        private static readonly Dictionary<string, int> _rarityMap = new Dictionary<string, int> {
            {"Legendary", 6},
            {"Epic", 5},
            {"Rare", 4},
            {"Quest", 3},
            {"Magical", 2},
        };

        private static readonly Dictionary<string, string> _rarityTranslations = new Dictionary<string, string> {
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
            string updateSql = $"UPDATE {BuddyItemsTable.Table} SET {BuddyItemsTable.Rarity} = :rarity WHERE {BuddyItemsTable.RemoteItemId} = :id";
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var item in items) {
                        string rarity = GetRarity(item, rows);
                        session.CreateSQLQuery(updateSql)
                            .SetParameter("id", item.RemoteItemId)
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
            var minimumLevel = obj[8] as float? ?? obj[8] as long?;

            var id = (string) obj[9];
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
                RemoteItemId = id,
                Count = (uint) count
            };
        }

        public IList<BuddyItem> ListAll(string where) {
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
                                {BuddyItemsTable.RemoteItemId} as RemoteItemId,
                                MAX(1, {BuddyItemsTable.StackCount}) as Count,
                                {BuddySubscriptionTable.Nickname} as Buddy
                        FROM {BuddyItemsTable.Table}, {BuddySubscriptionTable.Table}
                        {where}
                        AND {BuddyItemsTable.SubscriptionId} = {BuddySubscriptionTable.Id}
            ";


            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateSQLQuery(sql)
                    .List<object>()
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

        class DatabaseItemStatQuery {
            public string SQL;
            public Dictionary<string, string[]> Parameters;
        }

        private static DatabaseItemStatQuery CreateDatabaseStatQueryParams(ItemSearchRequest query) {
            List<string> queryFragments = new List<string>();
            Dictionary<string, string[]> queryParamsList = new Dictionary<string, string[]>();

            // Add the damage/resist filter
            foreach (string[] filter in query.Filters) {
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

            // Only items which grants new skills
            if (query.WithGrantSkillsOnly) {
                // TODO: Are there any prefixes or suffixes which grants skills?
                queryFragments.Add($"{BuddyItemsTable.BaseRecord} IN (SELECT PlayerItemRecord from ({ItemSkillDaoImpl.ListItemsQuery}) y)");
            }

            if (query.WithSummonerSkillOnly) {
                queryFragments.Add($@"{BuddyItemsTable.BaseRecord} IN (SELECT p.baserecord as PlayerItemRecord
                    from itemskill_v2 s, itemskill_mapping map, DatabaseItem_v2 db,  playeritem p, DatabaseItemStat_v2 stat  
                    where s.id_skill = map.id_skill 
                    and map.id_databaseitem = db.id_databaseitem  
                    and db.baserecord = p.baserecord 
                    and stat.id_databaseitem = s.id_databaseitem
                    and stat.stat = 'spawnObjects')");
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

        public IList<BuddyItem> FindBy(ItemSearchRequest query) {
            List<string> queryFragments = new List<string>();
            Dictionary<string, object> queryParams = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(query.Wildcard)) {
                queryFragments.Add($"{BuddyItemsTable.NameLowercase} LIKE :name");
                queryParams.Add("name", $"%{query.Wildcard.Replace(' ', '%').ToLowerInvariant()}%");
            }


            queryFragments.Add($"(LOWER({BuddyItemsTable.Mod}) = LOWER( :mod ) OR {BuddyItemsTable.Mod} IS NULL)");
            queryParams.Add("mod", query.Mod);

            if (!string.IsNullOrEmpty(query.Rarity)) {
                queryFragments.Add($"{BuddyItemsTable.Rarity} = :rarity");
                queryParams.Add("rarity", query.Rarity);
            }
            
            if (query.PrefixRarity > 1) {
                queryFragments.Add($"{BuddyItemsTable.PrefixRarity} >= :prefixRarity");
                queryParams.Add("prefixRarity", query.PrefixRarity);
            }

            queryFragments.Add(query.IsHardcore ? "IsHardcore" : "NOT IsHardcore");

            if (query.RecentOnly) {
                queryFragments.Add($"{BuddyItemsTable.CreatedAt} > :filter_recentOnly");
                queryParams.Add("filter_recentOnly", DateTime.UtcNow.AddHours(-12).ToTimestamp());
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

            queryFragments.Add($"NOT S.{BuddySubscriptionTable.IsHidden}");

            List<string> sql = new List<string>();
            sql.Add($@"SELECT
                                {BuddyItemsTable.BaseRecord} as BaseRecord,
                                {BuddyItemsTable.PrefixRecord} as PrefixRecord,
                                {BuddyItemsTable.SuffixRecord} as SuffixRecord,
                                {BuddyItemsTable.ModifierRecord} as ModifierRecord,
                                {BuddyItemsTable.TransmuteRecord} as TransmuteRecord,
                                {BuddyItemsTable.MateriaRecord} as MateriaRecord,
                                {BuddyItemsTable.Rarity} as Rarity,
                                {BuddyItemsTable.PrefixRarity} as PrefixRarity,
                                {BuddyItemsTable.Name} as Name,
 
                                {BuddyItemsTable.LevelRequirement} as MinimumLevel,
                                {BuddyItemsTable.RemoteItemId} as RemoteItemId,
                                {BuddyItemsTable.StackCount} as Count,
                                {BuddyItemsTable.SubscriptionId} as BuddyId,
                                S.{BuddySubscriptionTable.Nickname} as Stash,
                                coalesce((SELECT group_concat(Record, '|') FROM {BuddyItemRecordTable.Table} pir WHERE pir.{BuddyItemRecordTable.Item} = PI.{BuddyItemsTable.RemoteItemId} AND NOT {BuddyItemRecordTable.Record} IN (PI.BaseRecord, PI.SuffixRecord, PI.MateriaRecord, PI.PrefixRecord)), '') AS PetRecord,
                                R.text AS ReplicaInfo
                FROM {BuddyItemsTable.Table} PI, {BuddySubscriptionTable.Table} S 
                LEFT OUTER JOIN BuddyReplicaItem R ON PI.{BuddyItemsTable.RemoteItemId} = R.buddyitemid
                WHERE "
                    + string.Join(" AND ", queryFragments)
                    + $" AND {BuddyItemsTable.SubscriptionId} = {BuddySubscriptionTable.Id} "
            );

            var subquery = CreateDatabaseStatQueryParams(query);
            if (subquery != null) {
                sql.Add($" AND PI.{BuddyItemsTable.RemoteItemId} IN ({subquery.SQL})");
            }



            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    Logger.Debug(string.Join(" ", sql));
                    var q = session.CreateSQLQuery(string.Join(" ", sql));
                    q.AddScalar("BaseRecord", NHibernateUtil.String);
                    q.AddScalar("PrefixRecord", NHibernateUtil.String);
                    q.AddScalar("SuffixRecord", NHibernateUtil.String);
                    q.AddScalar("ModifierRecord", NHibernateUtil.String);
                    q.AddScalar("PrefixRarity", NHibernateUtil.Int64);
                    q.AddScalar("TransmuteRecord", NHibernateUtil.String);
                    q.AddScalar("MateriaRecord", NHibernateUtil.String);
                    q.AddScalar("Rarity", NHibernateUtil.String);
                    q.AddScalar("Name", NHibernateUtil.String);
                    q.AddScalar("MinimumLevel", NHibernateUtil.UInt32);
                    q.AddScalar("Id", NHibernateUtil.Int64);
                    q.AddScalar("Count", NHibernateUtil.UInt32);
                    q.AddScalar("Stash", NHibernateUtil.String);
                    q.AddScalar("BuddyId", NHibernateUtil.Int64);
                    q.AddScalar("RemoteItemId", NHibernateUtil.String);
                    q.AddScalar("PetRecord", NHibernateUtil.String);
                    q.AddScalar("ReplicaInfo", NHibernateUtil.String);

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

                    
                    Logger.Debug($"Search returned {result.Count} items");
                    return result;
                }
            }
        }
    }
}