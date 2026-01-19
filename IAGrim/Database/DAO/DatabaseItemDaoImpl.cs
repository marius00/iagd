using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Data;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Parsers.GameDataParsing.Model;
using Microsoft.Data.Sqlite;

namespace IAGrim.Database {
    /// <summary>
    /// Database class for handling internal Grim Dawn items
    /// These are not user owned items
    /// </summary>
    public class DatabaseItemDaoImpl : BaseDao<DatabaseItem>, IDatabaseItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DatabaseItemDaoImpl));

        public DatabaseItemDaoImpl(SessionFactory sessionCreator) : base(sessionCreator) {
        }


        /// <summary>
        /// Get the existing tag dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTagDictionary() {
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (var session = SessionCreator.OpenStatelessSession()) {
                foreach (ItemTag entry in session.CreateCriteria<ItemTag>().List()) {
                    result[entry.Tag] = entry.Name;
                }
            }

            return result;
        }


        public override void Save(DatabaseItem item) {
            using (var session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    session.CreateQuery("DELETE FROM DatabaseItemStat as S WHERE S.Parent.Id = :id")
                        .SetParameter("id", item.Id)
                        .ExecuteUpdate();

                    session.CreateQuery("DELETE FROM DatabaseItem WHERE Record = :record")
                        .SetParameter("record", item.Record)
                        .ExecuteUpdate();

                    transaction.Commit();
                }

                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (DatabaseItemStat stat in item.Stats) {
                        stat.Parent = item;
                        //session.Insert(stat);
                    }

                    session.Save(item);

                    transaction.Commit();
                }
            }
        }

        private void ExecuteTransactionSql(string[] commands, ProgressTracker progressTracker) {
            ExecuteTransactionSqlSqlite(commands, progressTracker);
        }

        private void ExecuteTransactionSqlSqlite(string[] commands, ProgressTracker progressTracker) {
            using SqliteConnection dbConnection = new SqliteConnection(SessionFactoryLoader.SessionFactory.ConnectionString);
            dbConnection.Open();

            using var transaction = dbConnection.BeginTransaction();


            foreach (var sql in commands) {
                using SqliteCommand command = new SqliteCommand(sql, dbConnection);
                command.Transaction = transaction;
                command.ExecuteNonQuery();
                progressTracker?.Increment();
            }

            transaction.Commit();
        }

        public void Save(List<DatabaseItem> items, ProgressTracker progressTracker) {
            int idx = 0;
            const int range = 950;

            List<DatabaseItem> updated;
            using (var session = SessionCreator.OpenSession()) {
                var hashes = new HashSet<string>(
                    session.CreateSQLQuery($"SELECT distinct({DatabaseItemTable.Record} || {DatabaseItemTable.Hash}) from {DatabaseItemTable.Table}")
                        .List<string>()
                );

                // This is absurdly slow
                updated = items.Where(m => !hashes.Contains(m.Record + m.Hash)).ToList();
            }

            DropItemIndexes();

            progressTracker.MaxValue = updated.Count;
            Logger.Info($"There are {updated.Count} updated items to store");
            while (idx < updated.Count) {
                var numItems = Math.Min(updated.Count - idx, range);
                var batch = updated.GetRange(idx, numItems);
                Save(batch, progressTracker, true);

                idx += range;
            }
            Logger.Info($"Batch complete");
        }

        private void DropItemIndexes() {
            var dropCommands = new[] {
                "drop index if exists idx_databaseitemstatv2_parent",
                "drop index if exists idx_databaseitemstatv2_stat",
                "drop index if exists idx_databaseitemstatv2_tv"
            };

            ExecuteTransactionSql(dropCommands, null);
        }

        public void CreateItemIndexes(ProgressTracker progressTracker) {
            var createCommands = new[] {
                "create index idx_databaseitemstatv2_parent on DatabaseItemStat_v2 (id_databaseitem)",
                "create index idx_databaseitemstatv2_stat on DatabaseItemStat_v2 (Stat)",
                "create index idx_databaseitemstatv2_tv on DatabaseItemStat_v2 (TextValue)"
            };

            var sq = new System.Diagnostics.Stopwatch();
            sq.Start();

            progressTracker.MaxValue = createCommands.Length;
            ExecuteTransactionSql(createCommands, progressTracker);

            sq.Stop();
            Logger.Info("Item indexes created");
            Console.WriteLine($"Creating the item indexes took {sq.ElapsedMilliseconds} milliseconds");
        }

        private void Save(List<DatabaseItem> items, ProgressTracker progressTracker, bool reset) {
            // Insert the items (not stats)
            using (var session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var records = items.Select(m => m.Record).ToList();

                    session.CreateSQLQuery($@"
                            DELETE FROM {DatabaseItemStatTable.Table} 
                            WHERE {DatabaseItemStatTable.Item} IN (
                                SELECT {DatabaseItemTable.Id} FROM {DatabaseItemTable.Table}
                                    WHERE {DatabaseItemTable.Record} IN ( :records )
                            )")
                        .SetParameterList("records", records)
                        .ExecuteUpdate();

                    session.CreateSQLQuery($"DELETE FROM {DatabaseItemTable.Table} WHERE {DatabaseItemTable.Record} IN ( :records )")
                        .SetParameterList("records", records)
                        .ExecuteUpdate();

                    transaction.Commit();
                }

                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (DatabaseItem item in items) {
                        session.Save(item);
                        progressTracker.Increment();
                    }

                    transaction.Commit();
                }
            }


            var sq = new System.Diagnostics.Stopwatch();
            sq.Start();
            const string sql = "insert into databaseitemstat_v2 (id_databaseitem, stat, textvalue, val1) values (@id, @stat, @tv, @val)";

            var numStats = InsertStatsSqlite(sql, items);

            sq.Stop();
            Logger.Info("Records stored");
            Console.WriteLine($"Storing the records took {sq.ElapsedMilliseconds} milliseconds");

            Logger.InfoFormat("Stored {0} items and {1} stats to internal db.", items.Count, numStats);
        }

        private int InsertStatsSqlite(string sql, List<DatabaseItem> items) {
            int numStats = 0;
            using (SqliteConnection dbConnection = new SqliteConnection(SessionFactoryLoader.SessionFactory.ConnectionString)) {
                dbConnection.Open();

                using var transaction = dbConnection.BeginTransaction();
                foreach (DatabaseItem item in items) {
                    foreach (DatabaseItemStat stat in item.Stats) {
                        using var command = new SqliteCommand(sql, dbConnection);
                        command.Transaction = transaction;
                        command.Parameters.Add(new SqliteParameter("@id", item.Id));
                        command.Parameters.Add(new SqliteParameter("@stat", stat.Stat));
                        command.Parameters.Add(new SqliteParameter("@tv", stat.TextValue ?? ""));
                        command.Parameters.Add(new SqliteParameter("@val", stat.Value));
                        command.ExecuteNonQuery();
                        numStats++;
                    }
                }

                transaction.Commit();
            }

            return numStats;
        }


        /// <summary>
        /// Returns "special items" which are stackable, such as Dynamite, Scrap, Blood of Chton, Ancient Brain, etc..
        /// </summary>
        public IList<string> GetSpecialStackableRecords() {
            const string sql = @"select i.baserecord
                    from databaseitemstat_v2 s, databaseitem_v2 i 
                    where stat = 'preventEasyDrops' and i.id_databaseitem = s.id_databaseitem";

            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateSQLQuery(sql).List<string>();
            }
        }

        /// <summary>
        /// Returns the records for potions, components, etc.. typical stackables.
        /// </summary>
        public IList<string> GetStackableComponentsPotionsMisc() {
            const string sql = @"select distinct i.baserecord from databaseitemstat_v2 s, databaseitem_v2 i 
                where (Stat = 'Class' AND TextValue IN ('ItemRelic', 'OneShot_PotionHealth', 'OneShot_PotionMana', 'OneShot_Scroll') OR i.baserecord like '%questitems%')
                and i.id_databaseitem = s.id_databaseitem";

            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateSQLQuery(sql).List<string>();
            }
        }

        public IList<ItemSetAssociation> GetItemSetAssociations() {
            const string sql = @"
            SELECT * FROM (
                SELECT BaseRecord, 
                (
	                SELECT T.name FROM DatabaseItemStat_v2 ST, ItemTag T WHERE id_databaseitem IN (SELECT id_databaseitem FROM DatabaseItem_v2 db WHERE db.baserecord = S.TextValue)
	                AND ST.stat = 'setName' AND T.tag = ST.TextValue
                ) as SetName

                FROM DatabaseItemStat_v2 S, DatabaseItem_v2 I
                WHERE Stat = 'itemSetName' 
                AND S.id_databaseitem = I.id_databaseitem
            ) x
            WHERE SetName IS NOT NULL";


            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateSQLQuery(sql)
                    .SetResultTransformer(Transformers.AliasToBean<ItemSetAssociation>())
                    .List<ItemSetAssociation>();
            }
        }



        public IList<string> ListAllRecords() {
            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateCriteria<DatabaseItem>()
                    .SetProjection(Projections.Property("Record"))
                    .AddOrder(Order.Asc("Record"))
                    .List<string>();
            }
        }


        public Int64 GetRowCount() {
            using (ISession session = SessionCreator.OpenSession()) {
                return session.CreateSQLQuery("SELECT COUNT(*) as N FROM DatabaseItem_V2").UniqueResult<Int64>();
            }
        }



        public void Clean() {
            // CREATE TABLE DatabaseItemStat_v2 (id_databaseitemstat  integer primary key autoincrement, id_databaseitem BIGINT, Stat TEXT, TextValue TEXT, val1 DOUBLE, constraint FK9663A5FC6B4AFA92 foreign key (id_databaseitem) references DatabaseItem_v2)
            string[] tables = new[] { "DatabaseItemStat_v2", "DatabaseItem_v2", "ItemTag" };
            string fetchCreateTableQuery = "SELECT sql FROM sqlite_master WHERE type='table' AND name = :table";


            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var table in tables) {
                        string recreateQuery = session.CreateSQLQuery(fetchCreateTableQuery).SetParameter("table", table).UniqueResult<string>();
                        session.CreateSQLQuery("DROP TABLE IF EXISTS " + table).ExecuteUpdate();
                        session.CreateSQLQuery(recreateQuery).ExecuteUpdate();
                    }

                    transaction.Commit();
                }

            }

        }
    }


}