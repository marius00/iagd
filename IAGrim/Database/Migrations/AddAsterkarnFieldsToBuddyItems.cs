using IAGrim.Database.DAO.Table;
using NHibernate;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace IAGrim.Database.Migrations {
    class AddAsterkarnFieldsToBuddyItems : IDatabaseMigration {

        public override void Migrate(SessionFactory sessionCreator) {
            var buddyItemColumns = new Dictionary<string, string> {
                {"AscendantAffixNameRecord", "TEXT"},
                {"AscendantAffix2hNameRecord", "TEXT"},
                {"RerollsUsed", "INT"},
                {"seed", "INTEGER"},
                {"relicseed", "INTEGER"},
                {"enchantmentseed", "INTEGER"},
            };
            foreach (var column in buddyItemColumns) {
                using ISession session = sessionCreator.OpenSession();
                using ITransaction transaction = session.BeginTransaction();
                if (!ColumnExists(sessionCreator, BuddyItemsTable.Table, column.Key)) {
                    session.CreateSQLQuery($"ALTER TABLE {BuddyItemsTable.Table} ADD COLUMN {column.Key} {column.Value}").ExecuteUpdate();
                }
                transaction.Commit();
            }


            if (!ColumnExists(sessionCreator, DatabaseItemTable.Table, DatabaseItemTable.NameLowercase)) {
                using ISession session = sessionCreator.OpenSession();
                using ITransaction transaction = session.BeginTransaction();

                session.CreateSQLQuery($"ALTER TABLE {DatabaseItemTable.Table} ADD COLUMN {DatabaseItemTable.NameLowercase} TEXT").ExecuteUpdate();
                transaction.Commit();
            }

            if (!ColumnExists(sessionCreator, "ReplicaItemRow", "TextLowercase")) {
                using ISession session = sessionCreator.OpenSession();
                using ITransaction transaction = session.BeginTransaction();

                session.CreateSQLQuery($"ALTER TABLE ReplicaItemRow ADD COLUMN TextLowercase TEXT").ExecuteUpdate();
                transaction.Commit();
            }
        }
    }
}