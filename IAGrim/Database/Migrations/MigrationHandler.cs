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
            // Order matters: tables must exist before columns can be added to them,
            // and all columns must exist before anything indexes or queries them.
            // Databases going as far back as 2016 are still in the wild, missing columns added since.
            Run(new EnableWalJournalMode());
            Run(new AddBaseTables());
            Run(new AddAsterkarnFieldsToPlayerItem());
            Run(new AddAsterkarnFieldsToBuddyItems());
            Run(new HbmSchemaMigration());

            Run(new DatabaseItemHashFixMigration());
            Run(new FixPlayerItemIdTypeMigration());
            Run(new FixDatabaseItemIdTypeMigration());

            Run(new AddIndices());
        }

        private void Run(IDatabaseMigration migration) {
            var name = migration.GetType().Name;
            var sw = Stopwatch.StartNew();

            try {
                migration.Migrate(_sessionCreator);
            } catch (Exception ex) {
                // Later migrations are how a damaged or outdated schema gets repaired,
                // so one failure must not take down the entire chain (and with it, startup).
                Logger.Fatal($"Migration {name} failed: {ex.Message}", ex);
            }

            sw.Stop();
            Logger.Info($"[timing] Migration {name} took {sw.ElapsedMilliseconds} ms");
        }
    }
}
