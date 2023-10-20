using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Parsers.GameDataParsing.Model;

namespace IAGrim.Database {

    public class DatabaseItemStatDaoImpl : BaseDao<DatabaseItemStat>, IDatabaseItemStatDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DatabaseItemStatDaoImpl));

        // Utterly useless
        private static readonly string[] SpecialIgnores = {"physicsMass",
            "actorHeight", "actorRadius", "chest", "itemCost", "medalVisible",
            "medal", "marketAdjustmentPercent", "physicsFriction", "waist", "scale",
            "sword", "sword2h",  "castsShadows", "feet",};

        // Useful text stats
        private static readonly string[] SpecialStats = {
            "setName",
            "itemSetName",
            "petBonusName",
            "Class",
            "itemClassification",
            "augmentMasteryLevel1",
            "augmentMasteryLevel2",
            "augmentMasteryLevel4",
            "augmentMasteryLevel3",
            "augmentMasteryName1",
            "augmentMasteryName2",
            "augmentMasteryName3",
            "augmentMasteryName4",
            "augmentSkillLevel1",
            "augmentSkillLevel2",
            "augmentSkillLevel3",
            "augmentSkillLevel4",
            "augmentSkillName1",
            "augmentSkillName2",
            "augmentSkillName3",
            "augmentSkillName4",
            "augmentAllLevel",
            "factionSource",
            "skillDownBitmapName",
            "skillUpBitmapName",
            "bitmap",
            "noteBitmap",
            "artifactFormulaBitmapName",
            "artifactBitmap",
            "bitmapButtonDown",
            "bitmapButtonUp",
            "relicBitmap",
            "shardBitmap",
            "emptyBitmap",
            "fullBitmap",
            "lootRandomizerName",
            "itemNameTag",
            "itemQualityTag",
            "itemStyleTag",
            "description",
            "levelRequirement",
            "itemSkillName",
            "skillDisplayName",
            "petSkillName",
            "buffSkillName",
            "characterBaseAttackSpeedTag",
            "conversionInType",
            "conversionOutType",
            "racialBonusRace",
            "itemText",
            "MasteryEnumeration",
            "modifiedSkillName1",
            "modifiedSkillName2",
            "modifiedSkillName3",
            "modifiedSkillName4",
            "modifierSkillName1",
            "modifierSkillName2",
            "modifierSkillName3",
            "modifierSkillName4",
            "petconversionOutType", 
            "petconversionInType",
            "petBurstSpawn"
        };

        public DatabaseItemStatDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {
        }


        public void Save(IEnumerable<DatabaseItemStat> objs, ProgressTracker progressTracker) {
            var databaseItemStats = objs as DatabaseItemStat[] ?? objs.ToArray();
            progressTracker.MaxValue = databaseItemStats.Length;
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var entry in databaseItemStats) {
                        session.Save(entry);
                        progressTracker.Increment();
                    }
                    transaction.Commit();
                }
            }

            progressTracker.MaxProgress();
        }

        

        public Dictionary<String, List<DBStatRow>> GetStats(ISession session, StatFetch fetchMode) {
            return GetStats(session, new List<string>(), fetchMode);
        }

        public Dictionary<String, List<DBStatRow>> GetStats(IEnumerable<string> records, StatFetch fetchMode) {
            using (var session = SessionCreator.OpenSession()) {
                return GetStats(session, records, fetchMode);
            }
        }

        public Dictionary<long, List<DBStatRow>> GetStats(List<long> records, StatFetch fetchMode) {
            using (var session = SessionCreator.OpenSession()) {
                var statMap = new Dictionary<long, List<DBStatRow>>();

                if (fetchMode != StatFetch.Skills)
                    throw new ArgumentException(nameof(fetchMode));

                Logger.Debug($"Fetching all stats for {records.Count} items, fetchMode={fetchMode}");


                string sql = string.Join(" ",
                    new[] {
                        $"select db.{DatabaseItemTable.Id} as Id, db.{DatabaseItemTable.Record} as Record, s.{DatabaseItemStatTable.Stat} as Stat, s.{DatabaseItemStatTable.Value} as Value, s.{DatabaseItemStatTable.TextValue} as TextValue from {DatabaseItemTable.Table} db, {DatabaseItemStatTable.Table} s",
                        $"where s.{DatabaseItemStatTable.Item} = db.{DatabaseItemTable.Id}",
                        $"AND ({DatabaseItemStatTable.Value} > 0 or {DatabaseItemStatTable.Stat} in ( :whitelist ))",
                        //$"AND db.{DatabaseItemTable.Id} IN (SELECT map.{SkillMappingTable.Item} FROM {SkillMappingTable.Table} map)",
                        $"AND NOT {DatabaseItemStatTable.Stat} IN ( :blacklist )",
                        records.Any() ? $"AND db.{DatabaseItemTable.Id} IN ( :filter )" : ""
                    }
                );


                IQuery query = session.CreateSQLQuery(sql)
                    .SetParameterList("whitelist", SpecialStats)
                    .SetParameterList("blacklist", SpecialIgnores)
                    .SetResultTransformer(Transformers.AliasToBean<DBStatRow>());

                Logger.Debug(sql);
                if (records.Count > 0) {
                    query.SetParameterList("filter", records);
                }


                foreach (DBStatRow row in query.List<DBStatRow>()) {
                    if (!statMap.ContainsKey(row.Id))
                        statMap[row.Id] = new List<DBStatRow>();

                    statMap[row.Id].Add(row);
                }

                Logger.Debug($"Fetching stat for {statMap.Count} records");


#if DEBUG
                // If you have run into this exception, you've messed up.
                // This is a safety catch to detect issues with applying stats too broadly.
                if (statMap.Count > 10000) {
                    throw new InvalidOperationException(
                        "Stat fetch has been done with too many items, severe performance penalties.");
                }
#endif


                return statMap;
            }
        }


        private Dictionary<string, List<DBStatRow>> GetStats(ISession session, IEnumerable<string> records, StatFetch fetchMode) {
            Dictionary<string, List<DBStatRow>> statMap = new Dictionary<string, List<DBStatRow>>();

            Logger.Debug($"Fetching all stats for {records.Count()} items, fetchMode={fetchMode}");


            string sql = string.Join(" ",
                new[] { $@"select db.{DatabaseItemTable.Record} as Record, s.stat as Stat, s.val1 as Value, s.textvalue as TextValue 
                FROM {DatabaseItemTable.Table} db, databaseitemstat_v2 s where s.id_databaseitem = db.{DatabaseItemTable.Id}",

                "AND (val1 > 0 or stat in ( :whitelist ))",
                    fetchMode == StatFetch.PlayerItems ? $"AND baserecord IN (SELECT record FROM playeritemrecord)" : "",
                    fetchMode == StatFetch.AugmentItems ? $"AND db.{DatabaseItemTable.Id} IN (SELECT a.{AugmentationItemTable.Id} FROM {AugmentationItemTable.Table} a)" : "",
                    fetchMode == StatFetch.BuddyItems ? $"AND {DatabaseItemTable.Record} IN (SELECT {BuddyItemRecordTable.Record} FROM {BuddyItemRecordTable.Table})" : "",
                fetchMode == StatFetch.RecipeItems ? $"AND baserecord in (SELECT {RecipeItemTable.Record} FROM {RecipeItemTable.Table})" : "",
                fetchMode == StatFetch.Skills ? $"AND db.{DatabaseItemTable.Id} IN (SELECT map.{SkillMappingTable.Item} FROM {SkillMappingTable.Table} map)" : "", // Redundant? Either doesn't cover enough, or completely redundant
                "AND NOT stat IN ( :blacklist )",
                records.Any() ? $"AND db.{DatabaseItemTable.Record} IN ( :filter )" : ""
                }
            );


            IQuery query = session.CreateSQLQuery(sql)
                .SetParameterList("whitelist", SpecialStats)
                .SetParameterList("blacklist", SpecialIgnores)
                .SetResultTransformer(Transformers.AliasToBean<DBStatRow>());

            Logger.Debug(sql);
            if (records.Count() > 0) {
                query.SetParameterList("filter", records);
                //logger.Debug($":filter={String.Join(",", records)}");
            }


            statMap[string.Empty] = new List<DBStatRow>();
            foreach (DBStatRow row in query.List<DBStatRow>()) {
                if (!statMap.ContainsKey(row.Record))
                    statMap[row.Record] = new List<DBStatRow>();

                statMap[row.Record].Add(row);
            }

            Logger.Debug($"Fetching stat for {statMap.Count} records");


#if DEBUG
            // If you have run into this exception, you've messed up.
            // This is a safety catch to detect issues with applying stats too broadly.
            if (statMap.Count > 10000) {
                throw new InvalidOperationException("Stat fetch has been done with too many items, severe performance penalties.");
            }
#endif


            return statMap;
        }

        public Dictionary<string, string> MapItemBitmaps(List<string> records) {
            Dictionary<string, int> bitmapScores = new Dictionary<string, int> {
                ["bitmap"] = 10,
                ["relicBitmap"] = 8,
                ["shardBitmap"] = 6,
                ["artifactBitmap"] = 4,
                ["noteBitmap"] = 2,
                ["artifactFormulaBitmapName"] = 0
            };

            Dictionary<string, string> recordBitmap = new Dictionary<string, string>();
            foreach (string record in records)
                recordBitmap[record] = record;

            // Grab all the possible bitmaps for each record
            using (var session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    DatabaseItemStat stat = null;
                    DatabaseItem P = null;
                    var stats = session.QueryOver<DatabaseItemStat>(() => stat)
                        .JoinAlias(() => stat.Parent, () => P)
                        .Where(m => P.Record.IsIn(records))
                        .Where(m => m.Stat.IsIn(new string[] { "bitmap", "relicBitmap", "shardBitmap", "artifactBitmap", "noteBitmap", "artifactFormulaBitmapName" }))
                        .List<DatabaseItemStat>();

                        
                    // Find the best bitmap for each record
                    foreach (string record in records) {
                        var best = stats.Where(m => m.Parent.Record.Equals(record)).OrderByDescending(m => bitmapScores[m.Stat]);
                        if (best.Any()) {
                            recordBitmap[record] = best.First().TextValue;
                        }
                    }

                }
            }

            return recordBitmap;
        }
    }
}
