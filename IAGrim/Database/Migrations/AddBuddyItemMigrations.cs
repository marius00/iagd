
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Table;
using log4net;
using NHibernate;

namespace IAGrim.Database.Migrations {
    class AddBuddyItemMigrations : IDatabaseMigration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AddSkillTables));

        private readonly string[] Sql = {
            $@"CREATE TABLE IF NOT EXISTS `buddyitems_v3` (
	            `id_buddyitem`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	            `id_buddy`	INTEGER NOT NULL,
                `id_buddy_remote`	INTEGER NOT NULL,
	            `baserecord`	TEXT NOT NULL,
	            `prefixrecord`	TEXT,
	            `suffixrecord`	TEXT,
	            `modifierrecord`	TEXT,
	            `transmuterecord`	TEXT,
	            `materiarecord`	TEXT,
	            `stackcount`	INTEGER NOT NULL,
	            `mod`	TEXT,
	            `name`	TEXT,
	            `levelrequirement`	INTEGER,
	            `rarity`	TEXT
            );",

            @"DROP TABLE IF EXISTS buddyitems_v2;",
            @"DROP TABLE IF EXISTS BuddyItem;",
            @"DROP TABLE IF EXISTS RecipeItem;",
            @"UPDATE PlayerItem SET stackcount = 1 where stackcount <= 0;",

            @"CREATE TABLE IF NOT EXISTS `BuddyItemRecord` 
                (`id_buddy` INTEGER NOT NULL, `id_buddyitem` INTEGER NOT NULL, `record` TEXT NOT NULL, 
                primary key (id_buddyitem, record));
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
