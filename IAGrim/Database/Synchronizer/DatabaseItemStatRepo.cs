using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Services.Dto;
using NHibernate;
using IAGrim.Database.DAO.Dto;

namespace IAGrim.Database.Synchronizer {
    class DatabaseItemStatRepo : BasicSynchronizer<DatabaseItemStat>, IDatabaseItemStatDao {
        private readonly IDatabaseItemStatDao repo;
        public DatabaseItemStatRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this.repo = new DatabaseItemStatDaoImpl(sessionCreator);
            this.BaseRepo = repo;
        }

        public Dictionary<string, List<DBSTatRow>> GetStats(IEnumerable<string> records, StatFetch fetchMode) {
            return ThreadExecuter.Execute(
                () => repo.GetStats(records, fetchMode)
            );
        }

        public Dictionary<long, List<DBSTatRow>> GetStats(List<long> records, StatFetch fetchMode) {
            return ThreadExecuter.Execute(
                () => repo.GetStats(records, fetchMode)
            );
        }

        public Dictionary<string, List<DBSTatRow>> GetStats(ISession session, StatFetch fetchMode) {
            return ThreadExecuter.Execute(
                () => repo.GetStats(session, fetchMode)
            );
        }


        public Dictionary<string, string> MapItemBitmaps(List<string> records) {
            return ThreadExecuter.Execute(
                () => repo.MapItemBitmaps(records)
            );
        }

        public Dictionary<string, ISet<DBSTatRow>> GetExpacSkillModifierSkills() {
            return ThreadExecuter.Execute(
                () => repo.GetExpacSkillModifierSkills()
            );
        }

        public string GetSkillName(string skillRecord) {
            return ThreadExecuter.Execute(
                () => repo.GetSkillName(skillRecord)
            );
        }
    }
}
