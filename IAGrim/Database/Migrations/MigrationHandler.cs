using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.Migrations {
    class MigrationHandler {
        private readonly ISessionCreator _sessionCreator;
        public MigrationHandler(ISessionCreator sessionCreator) {
            this._sessionCreator = sessionCreator;
        }
        public void Migrate() {
            new AddSkillTables().Migrate(_sessionCreator);
            new AddBuddyItemMigrations().Migrate(_sessionCreator);
            
        }
    }
}
