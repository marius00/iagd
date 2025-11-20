using NHibernate;

namespace IAGrim.Database.Migrations {
    class AddBaseTables : IDatabaseMigration {


        private readonly Dictionary<string, string> _tables = new Dictionary<string, string> {
            {"deletedplayeritem_v3", "CREATE TABLE deletedplayeritem_v3 (id TEXT not null, primary key (id))"},
            {"PlayerItemRecord", "CREATE TABLE PlayerItemRecord (PlayerItemId INTEGER not null, Record TEXT not null, primary key (PlayerItemId, Record))"},
            {"itemskill_v2", "CREATE TABLE itemskill_v2 (id_skill  integer primary key autoincrement, Description TEXT, Name TEXT, Record TEXT, Trigger TEXT, Level INTEGER, id_databaseitem INTEGER)"},
            {"itemskill_mapping", "CREATE TABLE itemskill_mapping (id_skill INTEGER not null, id_databaseitem INTEGER not null, primary key (id_skill, id_databaseitem))"},
            {"buddyitems_v6", "CREATE TABLE buddyitems_v6 (id_item_remote TEXT not null, id_buddy INTEGER not null, baserecord TEXT, prefixrecord TEXT, suffixrecord TEXT, modifierrecord TEXT, transmuterecord TEXT, materiarecord TEXT, stackcount INTEGER, ishardcore INTEGER, mod TEXT, name TEXT, namelowercase TEXT, levelrequirement REAL, created_at INTEGER, rarity TEXT, prefixrarity INTEGER, seed INTEGER, relicseed INTEGER, enchantmentseed INTEGER, AscendantAffixNameRecord TEXT, AscendantAffix2hNameRecord TEXT, RerollsUsed INTEGER, primary key (id_item_remote, id_buddy))"},
            {"BuddyItemRecord_v2", "CREATE TABLE BuddyItemRecord_v2 (id_item TEXT not null, record TEXT not null, primary key (id_item, record))"},
            {"ReplicaItem2", "CREATE TABLE ReplicaItem2 (Id INTEGER not null, playeritemid INTEGER unique, buddyitemid TEXT unique, primary key (Id))"},
            {"ReplicaItemRow", "CREATE TABLE ReplicaItemRow (Id INTEGER not null, replicaitemid INTEGER, Type INTEGER, Text TEXT, TextLowercase TEXT, primary key (Id))"},
            {"settings", "CREATE TABLE settings (setting TEXT not null, val1 INTEGER, V2 TEXT, primary key (setting))"},
            {"BuddySubscription", "CREATE TABLE BuddySubscription (Id INTEGER not null, Nickname TEXT, LastSyncTimestamp INTEGER, IsHidden INTEGER, primary key (Id))"},
            {"DatabaseItemStat_v2", "CREATE TABLE DatabaseItemStat_v2 (id_databaseitemstat  integer primary key autoincrement, id_databaseitem INTEGER, Stat TEXT, TextValue TEXT, val1 REAL, constraint FK_95F02CAE foreign key (id_databaseitem) references DatabaseItem_v2)"},
            {"DatabaseItem_v2", "CREATE TABLE DatabaseItem_v2 (id_databaseitem INTEGER not null, baserecord TEXT unique, name TEXT, hash INTEGER, namelowercase TEXT, primary key (id_databaseitem))"},
            {"ItemTag", "CREATE TABLE ItemTag (Tag TEXT not null, Name TEXT, primary key (Tag))"},
            {"PlayerItem", """
                CREATE TABLE "PlayerItem" (
                	"Id"	INTEGER NOT NULL,
                	"baserecord"	TEXT,
                	"PrefixRecord"	TEXT,
                	"SuffixRecord"	TEXT,
                	"ModifierRecord"	TEXT,
                	"TransmuteRecord"	TEXT,
                	"Seed"	INTEGER,
                	"MateriaRecord"	TEXT,
                	"RelicCompletionBonusRecord"	NUMERIC,
                	"RelicSeed"	INTEGER,
                	"EnchantmentRecord"	TEXT,
                	"PrefixRarity"	INTEGER,
                	"UNKNOWN"	INTEGER,
                	"EnchantmentSeed"	INTEGER,
                	"MateriaCombines"	INTEGER,
                	"StackCount"	INTEGER,
                	"Name"	TEXT,
                	"namelowercase"	TEXT,
                	"Rarity"	TEXT,
                	"LevelRequirement"	REAL,
                	"Mod"	TEXT,
                	"IsHardcore"	INTEGER,
                	"cloudid"	TEXT,
                	"cloud_hassync"	INTEGER,
                	"created_at"	INTEGER, AscendantAffixNameRecord TEXT, AscendantAffix2hNameRecord TEXT, RerollsUsed INT,
                	PRIMARY KEY("Id")
                )
                """},
        };

        private readonly List<string> _indices = new List<string>() {
            "CREATE INDEX idx_databaseitemstatv2_parent_stat on DatabaseItemStat_v2 (id_databaseitem)",
            "CREATE INDEX idx_databaseitemstatv2_stat on DatabaseItemStat_v2 (Stat)",
            "CREATE INDEX idx_databaseitemv2_record on DatabaseItem_v2 (baserecord)",
            "CREATE INDEX idx_playeritem_baserecord on PlayerItem (baserecord)",
            "CREATE INDEX idx_playeritem_levelreq on PlayerItem (LevelRequirement)",
            "CREATE INDEX idx_playeritem_lowercasename on PlayerItem (namelowercase)",
            "CREATE INDEX idx_playeritem_prefix on PlayerItem (PrefixRecord)",
            "CREATE INDEX idx_playeritem_rarity on PlayerItem (Rarity)",
            "CREATE INDEX idx_playeritem_suffix on PlayerItem (SuffixRecord)",
            "CREATE INDEX idx_replicaitem_buddyitemid on ReplicaItem2 (buddyitemid)",
            "CREATE INDEX idx_replicaitem_playeritemid on ReplicaItem2 (playeritemid)",
            "CREATE INDEX idx_replicaitemstat_replicaitemid on ReplicaItemRow (replicaitemid)",
            "CREATE INDEX idx_databaseitemv2_baserecord on DatabaseItem_v2 (baserecord)",
        };

        private readonly List<string> _oldTables = new List<string>() {
            "AugmentationItem",
            "BuddyItemRecord",
            "azurepartition_v2",
            "buddyitems_v5",
            "deletedplayeritem_v2",
            "ReplicaItem",
            "BuddyReplicaItem",
            "BuddyStash",
            "RecipeItem_v2",
        };

        public override void Migrate(SessionFactory sessionCreator) {
            foreach (var table in _tables) {

                if (TableExists(sessionCreator, table.Key)) {
                    continue;
                }

                using ISession session = sessionCreator.OpenSession();
                using ITransaction transaction = session.BeginTransaction();
                session.CreateSQLQuery(table.Value).ExecuteUpdate();
                transaction.Commit();
            }

            foreach (var index in _indices) {
                var name = index.Split(" ")[2];

                if (IndexExists(sessionCreator, name)) {
                    continue;
                }

                using ISession session = sessionCreator.OpenSession();
                using ITransaction transaction = session.BeginTransaction();
                session.CreateSQLQuery(index).ExecuteUpdate();
                transaction.Commit();
            }

            foreach (var table in _oldTables) {
                if (!TableExists(sessionCreator, table)) {
                    continue;
                }

                using ISession session = sessionCreator.OpenSession();
                using ITransaction transaction = session.BeginTransaction();
                session.CreateSQLQuery($"DROP TABLE {table}").ExecuteUpdate();
                transaction.Commit();
            }
        }
    }
}