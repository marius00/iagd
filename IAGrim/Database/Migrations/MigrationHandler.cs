﻿using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Util;

namespace IAGrim.Database.Migrations {
    class MigrationHandler {
        private readonly ISessionCreator _sessionCreator;
        public MigrationHandler(ISessionCreator sessionCreator) {
            this._sessionCreator = sessionCreator;
        }
        public void Migrate() {
            new DatabaseItemHashFixMigration().Migrate(_sessionCreator);
            if (_sessionCreator.GetDialect() == SqlDialect.Postgres) {
                new PostgresMigrations().Migrate(_sessionCreator);
            }
        }
    }
}
