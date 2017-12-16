
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Table;
using log4net;
using NHibernate;

namespace IAGrim.Database.Migrations {
    class AddSkillTables : IDatabaseMigration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AddSkillTables));

        private readonly string[] Sql = {
            @"
        CREATE TABLE IF NOT EXISTS `itemskill_mapping` (
	        `id_skill`	INTEGER NOT NULL,
	        `id_databaseitem`	INTEGER NOT NULL,
	        PRIMARY KEY(`id_skill`, `id_databaseitem`),
	        UNIQUE (`id_skill`, `id_databaseitem`)
        );",
            @"DROP TABLE IF EXISTS `itemskill`;",
            @"DROP TABLE IF EXISTS `DatabaseItemStat`;",
            @"DROP TABLE IF EXISTS `DatabaseItem`;",
            @"
        CREATE TABLE IF NOT EXISTS `itemskill_v2` (
	        `id_skill`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	        `Description`	TEXT,
	        `Level`	INTEGER,
	        `Name`	TEXT,
	        `Record`	TEXT NOT NULL,
	        `id_databaseitem`	INTEGER NOT NULL,
	        `Trigger`	TEXT
        );
        "
        };

        public void Migrate(ISessionCreator sessionCreator) {
            Logger.Debug("Executing migration");
            using (ISession session = sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var line in Sql) {
                        session.CreateSQLQuery(line).ExecuteUpdate();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
