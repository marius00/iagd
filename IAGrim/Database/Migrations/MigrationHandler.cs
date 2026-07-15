using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IAGrim.Database.Migrations {
    class MigrationHandler {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MigrationHandler));
        private readonly SessionFactory _sessionCreator;
        public MigrationHandler(SessionFactory sessionCreator) {
            this._sessionCreator = sessionCreator;
        }
        public void Migrate() {
            Run(new EnableWalJournalMode());
            Run(new AddBaseTables());
            Run(new AddAsterkarnFieldsToPlayerItem());
            Run(new AddAsterkarnFieldsToBuddyItems());

            Run(new DatabaseItemHashFixMigration());
            Run(new FixPlayerItemIdTypeMigration());
            Run(new FixDatabaseItemIdTypeMigration());
            Run(new AddPlayerItemRecordIndex());
            Run(new AddPlayerItemNameIndex());
            Run(new HbmSchemaMigration());
        }

        private void Run(IDatabaseMigration migration) {
            var sw = Stopwatch.StartNew();
            migration.Migrate(_sessionCreator);
            sw.Stop();
            Logger.Info($"[timing] Migration {migration.GetType().Name} took {sw.ElapsedMilliseconds} ms");
        }
    }
}
