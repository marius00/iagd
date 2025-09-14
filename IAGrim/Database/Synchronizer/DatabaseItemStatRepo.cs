using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using NHibernate;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Synchronizer.Core;
using IAGrim.Parsers.GameDataParsing.Model;

namespace IAGrim.Database.Synchronizer {
    class DatabaseItemStatRepo : BasicSynchronizer<DatabaseItemStat>, IDatabaseItemStatDao {
        private readonly IDatabaseItemStatDao repo;
        public DatabaseItemStatRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this.repo = new DatabaseItemStatDaoImpl(sessionCreator);
            this.BaseRepo = repo;
        }

        public Dictionary<string, List<DBStatRow>> GetStats(IEnumerable<string> records, StatFetch fetchMode) {
            return ThreadExecuter.Execute(
                () => repo.GetStats(records, fetchMode)
            );
        }

        public Dictionary<long, List<DBStatRow>> GetStats(List<long> records, StatFetch fetchMode) {
            return ThreadExecuter.Execute(
                () => repo.GetStats(records, fetchMode)
            );
        }

        public Dictionary<string, List<DBStatRow>> GetStats(ISession session, StatFetch fetchMode) {
            return ThreadExecuter.Execute(
                () => repo.GetStats(session, fetchMode)
            );
        }


        public Dictionary<string, string> MapItemBitmaps(List<string> records) {
            return ThreadExecuter.Execute(
                () => repo.MapItemBitmaps(records)
            );
        }

        public void Save(IEnumerable<DatabaseItemStat> objs, ProgressTracker progressTracker) {
            ThreadExecuter.Execute(
                () => repo.Save(objs, progressTracker)
            );
        }

    }
}
