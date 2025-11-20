using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Migrations {
    class MigrationHandler {
        private readonly SessionFactory _sessionCreator;
        public MigrationHandler(SessionFactory sessionCreator) {
            this._sessionCreator = sessionCreator;
        }
        public void Migrate() {
            new EnableWalJournalMode().Migrate(_sessionCreator);
            new AddBaseTables().Migrate(_sessionCreator);
            new AddAsterkarnFieldsToPlayerItem().Migrate(_sessionCreator);
            new AddAsterkarnFieldsToBuddyItems().Migrate(_sessionCreator);
            
            new DatabaseItemHashFixMigration().Migrate(_sessionCreator);
        }
    }
}
