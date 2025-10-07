using IAGrim.Database.DAO.Table;
using NHibernate;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace IAGrim.Database.Migrations {
    class AddAsterkarnFieldsToPlayerItem : IDatabaseMigration {

        public override void Migrate(SessionFactory sessionCreator) {
            if (ColumnExists(sessionCreator, "playeritem", "AscendantAffixNameRecord")) {
                return;
            }

            using (ISession session = sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery($"ALTER TABLE {PlayerItemTable.Table} ADD COLUMN AscendantAffixNameRecord TEXT").ExecuteUpdate();
                    session.CreateSQLQuery($"ALTER TABLE {PlayerItemTable.Table} ADD COLUMN AscendantAffix2hNameRecord TEXT").ExecuteUpdate();
                    session.CreateSQLQuery($"ALTER TABLE {PlayerItemTable.Table} ADD COLUMN RerollsUsed INT").ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }
    }
}