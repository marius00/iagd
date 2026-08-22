using System.Collections.Generic;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Model;
using IAGrim.Parsers.Arz.dto;
using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Transform;

namespace IAGrim.Database.DAO {
    internal class ItemSkillDaoImpl : IItemSkillDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemSkillDaoImpl));
        private readonly SessionFactory _sessionCreator;

        // Every item record in the game database that grants a skill. Used as an "does this record grant a skill"
        // set test, so it deliberately does not join PlayerItem: callers already test it against a player item's
        // record, which makes an owned-only restriction redundant and turns the join into one row per owned copy.
        public static readonly string SkillGrantingRecordsQuery = string.Join(" ",
            $"SELECT DISTINCT db.{DatabaseItemTable.Record} as PlayerItemRecord",
            $"from {SkillTable.Table} s, {SkillMappingTable.Table} map, {DatabaseItemTable.Table} db ",
            $"where s.{SkillTable.Id} = map.{SkillMappingTable.Skill} ",
            $"and map.{SkillMappingTable.Item} = db.{DatabaseItemTable.Id} "
        );

        // As above, but restricted to records the player owns a copy of. The buddy-item search has always scoped
        // its "grants a skill" filter that way; kept as-is so this stays a performance change and nothing else.
        public static readonly string OwnedSkillGrantingRecordsQuery = string.Join(" ",
            SkillGrantingRecordsQuery,
            $"and db.{DatabaseItemTable.Record} IN (SELECT {PlayerItemTable.Record} FROM {PlayerItemTable.Table})"
        );

        // The skills granted by a given set of item records. Scoped to the records actually being displayed:
        // the unscoped form returned one row per (skill-granting record, owned copy of it), which on a large
        // collection is tens of thousands of duplicate rows to materialize on every search and every page.
        private static readonly string ListForRecordsQuery = string.Join(" ",
            $"SELECT DISTINCT db.{DatabaseItemTable.Record} as PlayerItemRecord, ",
            $"s.{SkillTable.Description} as Description, ",
            $"s.{SkillTable.Level} as Level, ",
            $"s.{SkillTable.Name} as Name, ",
            $"s.{SkillTable.Trigger} as TriggerRecord, ",
            $"s.{SkillTable.StatsId} as StatsId, ",
            $"s.{SkillTable.Record} as Record",
            $"from {SkillTable.Table} s, {SkillMappingTable.Table} map, {DatabaseItemTable.Table} db ",
            $"where s.{SkillTable.Id} = map.{SkillMappingTable.Skill} ",
            $"and map.{SkillMappingTable.Item} = db.{DatabaseItemTable.Id} ",
            $"and db.{DatabaseItemTable.Record} IN ( :records )"
        );

        public ItemSkillDaoImpl(SessionFactory sessionCreator) {
            _sessionCreator = sessionCreator;
        }

        public void Save(ISet<ItemGrantedSkill> skills, bool additive) {
            Logger.Debug($"Storing {skills.Count} skills to the database");

            var itemSubquery =
                $"SELECT {DatabaseItemTable.Id} FROM {DatabaseItemTable.Table} WHERE {DatabaseItemTable.Record} = :record LIMIT 1";

            var sql =
                $"INSERT INTO {SkillTable.Table} ({SkillTable.Description}, {SkillTable.Level}, {SkillTable.Name}, {SkillTable.Record}, {SkillTable.StatsId}, {SkillTable.Trigger})" +
                $" VALUES (:description, :level, :name, :record, ({itemSubquery}), :trigger)";

            using (ISession session = _sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    if (!additive) {
                        session.CreateSQLQuery($"DELETE FROM {SkillTable.Table}")
                            .ExecuteUpdate();
                    }

                    foreach (var skill in skills) {
                        session.CreateSQLQuery(sql)
                            .SetParameter("description", skill.Description)
                            .SetParameter("level", skill.Level)
                            .SetParameter("name", skill.Name)
                            .SetParameter("record", skill.Record)
                            .SetParameter("trigger", skill.Trigger)
                            .ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }

            Logger.Debug("Skills stored");
        }

        public void EnsureCorrectSkillRecords() {
            Logger.Debug("Updating skill records in case of displacement..");
            string sql = $"UPDATE {SkillTable.Table} SET {SkillTable.StatsId} = (SELECT {DatabaseItemTable.Id} FROM {DatabaseItemTable.Table} i WHERE i.{DatabaseItemTable.Record} = {SkillTable.Record})";
            using (ISession session = _sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery(sql).ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        public IList<PlayerItemSkill> ListForRecords(IEnumerable<string> baseRecords) {
            var records = baseRecords.Where(r => !string.IsNullOrEmpty(r)).Distinct().ToList();
            if (records.Count == 0) {
                return new List<PlayerItemSkill>(0);
            }

            using (ISession session = _sessionCreator.OpenSession()) {
                return session.CreateSQLQuery(ListForRecordsQuery)
                    .SetParameterList("records", records)
                    .SetResultTransformer(Transformers.AliasToBean<PlayerItemSkill>())
                    .List<PlayerItemSkill>();
            }
        }


        public void Save(Dictionary<string, List<string>> skillItemMapping, bool additive) {
            Logger.Debug($"Storing skill mappings for {skillItemMapping.Count} skills to the database");

            var skillSubquery =
                $"SELECT {SkillTable.Id} FROM {SkillTable.Table} WHERE {SkillTable.Record} = :skillRecord LIMIT 1";

            var itemSubquery =
                $"SELECT {DatabaseItemTable.Id} FROM {DatabaseItemTable.Table} WHERE {DatabaseItemTable.Record} = :itemRecord LIMIT 1";

            var sql =
                $"INSERT INTO {SkillMappingTable.Table} ({SkillMappingTable.Skill}, {SkillMappingTable.Item})" +
                $" VALUES (({skillSubquery}), ({itemSubquery}))";

            using (ISession session = _sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    if (!additive) {
                        session.CreateSQLQuery($"DELETE FROM {SkillMappingTable.Table}")
                            .ExecuteUpdate();
                    }

                    foreach (var skillRecord in skillItemMapping.Keys) {
                        foreach (var itemRecord in skillItemMapping[skillRecord]) {
                            session.CreateSQLQuery(sql)
                                .SetParameter("skillRecord", skillRecord)
                                .SetParameter("itemRecord", itemRecord)
                                .ExecuteUpdate();
                        }
                    }

                    transaction.Commit();
                }
            }

            Logger.Debug("Skill mappings stored");
        }
    }
}