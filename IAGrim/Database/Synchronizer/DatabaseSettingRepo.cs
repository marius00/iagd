using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Synchronizer {
    class DatabaseSettingRepo : BasicSynchronizer<DatabaseSetting>, IDatabaseSettingDao {
        private readonly IDatabaseSettingDao repo;
        public DatabaseSettingRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this.repo = new DatabaseSettingDaoImpl(sessionCreator);
            this.BaseRepo = repo;
        }

        public string GetCurrentDatabasePath() {
            return ThreadExecuter.Execute(
                () => repo.GetCurrentDatabasePath()
            );
        }

        public long GetLastDatabaseUpdate() {
            return ThreadExecuter.Execute(
                () => repo.GetLastDatabaseUpdate()
            );
        }

        public void UpdateCurrentDatabase(string path) {
            ThreadExecuter.Execute(
                () => repo.UpdateCurrentDatabase(path)
            );
        }

        public void UpdateDatabaseTimestamp(long ts) {
            ThreadExecuter.Execute(
                () => repo.UpdateDatabaseTimestamp(ts)
            );
        }

        public string GetUuid() {
            return ThreadExecuter.Execute(
                () => repo.GetUuid()
            );
        }

        public void SetUuid(string uuid) {
            ThreadExecuter.Execute(
                () => repo.SetUuid(uuid)
            );
        }

        public void UpdateRecipeTimestamp(long ts) {
            ThreadExecuter.Execute(
                () => repo.UpdateRecipeTimestamp(ts)
            );
        }
    }
}
