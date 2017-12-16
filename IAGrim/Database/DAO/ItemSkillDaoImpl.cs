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
        private readonly ISessionCreator _sessionCreator;

        public ItemSkillDaoImpl(ISessionCreator sessionCreator) {
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

        public IList<PlayerItemSkill> List() {
            var sql =
                string.Join(" ",
                    $"SELECT p.{PlayerItemTable.Record} as PlayerItemRecord, ",
                    $"s.{SkillTable.Description}, ",
                    $"s.{SkillTable.Level}, ",
                    $"s.{SkillTable.Name}, ",
                    $"s.{SkillTable.Trigger} as TriggerRecord, ",
                    $"s.{SkillTable.StatsId} as StatsId, ",
                    $"s.{SkillTable.Record} as Record",
                    $"from {SkillTable.Table} s, {SkillMappingTable.Table} map, {DatabaseItemTable.Table} db, {PlayerItemTable.Table} p ",
                    $"where s.{SkillTable.Id} = map.{SkillMappingTable.Skill} ",
                    $"and map.{SkillMappingTable.Item} = db.{DatabaseItemTable.Id} ",
                    $"and db.{DatabaseItemTable.Record} = p.{PlayerItemTable.Record} "
            );

            Logger.Debug(sql);
            
            using (ISession session = _sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<PlayerItemSkill>())
                        .List<PlayerItemSkill>();
                }
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