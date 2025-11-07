using IAGrim.Database.DAO.Table;
using NHibernate;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace IAGrim.Database.Migrations {
    class AddAsterkarnFieldsToBuddyItems : IDatabaseMigration {

        public override void Migrate(SessionFactory sessionCreator) {
            if (!ColumnExists(sessionCreator, BuddyItemsTable.Table, "AscendantAffixNameRecord")) {
                using ISession session = sessionCreator.OpenSession();
                using ITransaction transaction = session.BeginTransaction();

                session.CreateSQLQuery($"ALTER TABLE {BuddyItemsTable.Table} ADD COLUMN AscendantAffixNameRecord TEXT").ExecuteUpdate();
                session.CreateSQLQuery($"ALTER TABLE {BuddyItemsTable.Table} ADD COLUMN AscendantAffix2hNameRecord TEXT").ExecuteUpdate();
                session.CreateSQLQuery($"ALTER TABLE {BuddyItemsTable.Table} ADD COLUMN RerollsUsed INT").ExecuteUpdate();

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