﻿using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Util;

namespace IAGrim.Database.Synchronizer {
    public class DatabaseSettingRepo : BasicSynchronizer<DatabaseSetting>, IDatabaseSettingDao {
        private readonly IDatabaseSettingDao repo;
        public DatabaseSettingRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            this.repo = new DatabaseSettingDaoImpl(sessionCreator, dialect);
            this.BaseRepo = repo;
        }

        public string GetCurrentDatabasePath() {
            return ThreadExecuter.Execute(
                () => repo.GetCurrentDatabasePath()
            );
        }

        public void UpdateCurrentDatabase(string path) {
            ThreadExecuter.Execute(
                () => repo.UpdateCurrentDatabase(path)
            );
        }

        public void Clean() {
            ThreadExecuter.Execute(
                () => repo.Clean()
            );
        }

        public string GetUuid() {
            return ThreadExecuter.Execute(
                () => repo.GetUuid()
            );
        }
    }
}
