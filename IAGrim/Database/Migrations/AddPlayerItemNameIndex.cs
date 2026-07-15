using NHibernate;

namespace IAGrim.Database.Migrations {
    /// <summary>
    /// Adds an index on PlayerItem(name, Id).
    ///
    /// The paged item search orders by "PI.name, PI.Id" (see SearchForItems) so LIMIT/OFFSET slices
    /// are stable across pages. PlayerItem had no index on 'name', so that ORDER BY forced SQLite to
    /// materialize and sort the entire match set before applying LIMIT - on a fully populated
    /// collection (100k+ rows) a broad search sorted every matching row just to return the first page.
    /// With this index the ordering is satisfied by an index walk, letting LIMIT terminate early
    /// instead of sorting everything. The composite (name, Id) matches the ORDER BY key exactly.
    /// </summary>
    class AddPlayerItemNameIndex : IDatabaseMigration {
        public override void Migrate(SessionFactory sessionCreator) {
            if (IndexExists(sessionCreator, "idx_playeritem_name")) {
                return;
            }

            using (ISession session = sessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateSQLQuery(
                        "CREATE INDEX idx_playeritem_name on PlayerItem (name, Id)"
                    ).ExecuteUpdate();

                    transaction.Commit();
                }
            }
        }
    }
}
