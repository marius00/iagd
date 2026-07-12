using NHibernate;

namespace IAGrim.Database.Migrations {
    /// <summary>
    /// Adds an index on PlayerItemRecord(record).
    ///
    /// The record-based search filters (stat/damage, class, slot) join PlayerItemRecord on its
    /// 'record' column. The table's only pre-existing index is the composite primary key
    /// (PlayerItemId, Record), whose leftmost column is PlayerItemId, so a join/lookup on 'record'
    /// alone could not use it and fell back to a full table scan. On a fully populated collection
    /// (hundreds of thousands of rows) that made filtered searches take over a minute.
    /// </summary>
    class AddPlayerItemRecordIndex : IDatabaseMigration {
        public override void Migrate(SessionFactory sessionCreator) {
            if (IndexExists(sessionCreator, "idx_playeritemrecord_record")) {
                return;
            }

            using (ISession session = sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery(
                        "CREATE INDEX idx_playeritemrecord_record on PlayerItemRecord (record)"
                    ).ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }
    }
}
