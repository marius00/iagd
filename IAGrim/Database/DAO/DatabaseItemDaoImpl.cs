using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using DataAccess;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Parsers.GameDataParsing.Model;
using IAGrim.Services.Dto;
using log4net.Repository.Hierarchy;
using System.Data.Common;
using System.Data.Odbc;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace IAGrim.Database {
    /// <summary>
    /// Database class for handling internal Grim Dawn items
    /// These are not user owned items
    /// </summary>
    public class DatabaseItemDaoImpl : BaseDao<DatabaseItem>, IDatabaseItemDao {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DatabaseItemDaoImpl));

        public DatabaseItemDaoImpl(ISessionCreator sessionCreator, SqlDialect dialect) : base(sessionCreator, dialect) {
        }


        /// <summary>
        /// Get the existing tag dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTagDictionary() {
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (session.BeginTransaction()) {
                    foreach (ItemTag entry in session.CreateCriteria<ItemTag>().List()) {
                        result[entry.Tag] = entry.Name;
                    }
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
            if (Dialect == SqlDialect.Sqlite) {
                ExecuteTransactionSqlSqlite(commands, progressTracker);
            }
            else {
                ExecuteTransactionSqlPostgres(commands, progressTracker);
            }
        }

        private void ExecuteTransactionSqlPostgres(string[] commands, ProgressTracker progressTracker) {
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    foreach (var command in commands) {
                        session.CreateSQLQuery(command);
                        progressTracker?.Increment();
                    }

                    transaction.Commit();
                }
            }
        }

        private void ExecuteTransactionSqlSqlite(string[] commands, ProgressTracker progressTracker) {
            using (SQLiteConnection dbConnection = new SQLiteConnection(SessionFactoryLoader.SessionFactory.ConnectionString)) {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction()) {
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection)) {
                        foreach (var sql in commands) {
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                            progressTracker?.Increment();
                        }
                    }

                    transaction.Commit();
                }
            }
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
            while (idx < updated.Count) {
                var numItems = Math.Min(updated.Count - idx, range);
                var batch = updated.GetRange(idx, numItems);
                Save(batch, progressTracker, true);

                idx += range;
            }
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

            int numStats;
            if (Dialect == SqlDialect.Sqlite) {
                numStats = InsertStatsSqlite(sql, items);
            }
            else {
                numStats = InsertStatsPostgres(items);
            }

            sq.Stop();
            Logger.Info("Records stored");
            Console.WriteLine($"Storing the records took {sq.ElapsedMilliseconds} milliseconds");

            Logger.InfoFormat("Stored {0} items and {1} stats to internal db.", items.Count, numStats);
        }

        private int InsertStatsPostgres(List<DatabaseItem> items) {
            int numStats = 0;

            string GetQueryString(int numRows) {
                if (numRows > 8190)
                    throw new ArgumentException("Cannot create query, too many rows/args");

                string query = $"INSERT INTO {DatabaseItemStatTable.Table} ({DatabaseItemStatTable.Item}, {DatabaseItemStatTable.Stat}, {DatabaseItemStatTable.Value}, {DatabaseItemStatTable.TextValue}) VALUES ";

                // Batching these into 7500 inserts (~30,000 params, the postgres limit is 32767)
                List<string> entries = new List<string>(numRows);
                for (int i = 0; i < numRows; i++) {
                    entries.Add($"(:i{i}, :s{i}, :v{i}, :tv{i})");
                }

                
                return query + string.Join(", ", entries);
            }

            // Insert stats
            using (var session = SessionCreator.OpenStatelessSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {

                    // Batching these into 7500 inserts (~30,000 params, the postgres limit is 32767)
                    int batchSize = 7500;
                    List<string> entries = new List<string>(batchSize);


                    foreach (var item in items) {
                        numStats += item.Stats.Count;

                        // Not ideal to not use argumnets, but sending in 30,000 named arguments takes minutes, not seconds.
                        foreach (DatabaseItemStat stat in item.Stats) {
                            var v = stat.Value.ToString().Replace(",", ".");
                            entries.Add($"({item.Id}, '{stat.Stat}', {v}, '{stat.TextValue?.Replace("'", "''")}')");

                            // 1875 items, max args
                            if (entries.Count >= batchSize) {
                                var query = session.CreateSQLQuery($"INSERT INTO {DatabaseItemStatTable.Table} ({DatabaseItemStatTable.Item}, {DatabaseItemStatTable.Stat}, {DatabaseItemStatTable.Value}, {DatabaseItemStatTable.TextValue}) VALUES "
                                                       + string.Join(",", entries));
                                int affectedRows = query.ExecuteUpdate();
                                Logger.Info($"Inserted stats with {affectedRows} affected rows");

                                entries.Clear();
                            }
                        }

                    }

                    // We got a batch that never hit 7500
                    if (entries.Count > 0) {
                        var query = session.CreateSQLQuery($"INSERT INTO {DatabaseItemStatTable.Table} ({DatabaseItemStatTable.Item}, {DatabaseItemStatTable.Stat}, {DatabaseItemStatTable.Value}, {DatabaseItemStatTable.TextValue}) VALUES "
                                                           + string.Join(",", entries));
                        int affectedRows = query.ExecuteUpdate();
                        Logger.Info($"Inserted stats with {affectedRows} affected rows (trailing)");
                    }

                    transaction.Commit();
                }
            }

            return numStats;
        }

        private int InsertStatsSqlite(string sql, List<DatabaseItem> items) {
            int numStats = 0;
            using (SQLiteConnection dbConnection = new SQLiteConnection(SessionFactoryLoader.SessionFactory.ConnectionString)) {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction()) {
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection)) {
                        command.CommandText = sql;
                        foreach (DatabaseItem item in items) {
                            foreach (DatabaseItemStat stat in item.Stats) {
                                command.Parameters.Add(new SQLiteParameter("@id", item.Id));
                                command.Parameters.Add(new SQLiteParameter("@stat", stat.Stat));
                                command.Parameters.Add(new SQLiteParameter("@tv", stat.TextValue));
                                command.Parameters.Add(new SQLiteParameter("@val", stat.Value));
                                command.ExecuteNonQuery();
                                numStats++;
                            }
                        }
                    }

                    transaction.Commit();
                }
            }

            return numStats;
        }


        public DatabaseItem FindByRecord(string record) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var item = session.CreateCriteria<DatabaseItem>().Add(Restrictions.Eq("Record", record)).UniqueResult();
                    if (item != null)
                        return (DatabaseItem) item;
                }
            }

            return null;
        }


        internal class InteralRowStat {
            public string Record { get; set; }
            public string Stat { get; set; }
            public double Value { get; set; }
            public string TextValue { get; set; }

            public string Name { get; set; }
        }

        // 

        /// <summary>
        /// Returns "special items" which are stackable, such as Dynamite, Scrap, Blood of Chton, Ancient Brain, etc..
        /// </summary>
        public IList<string> GetSpecialStackableRecords() {
            const string sql = @"select i.baserecord
                    from databaseitemstat_v2 s, databaseitem_v2 i 
                    where stat = 'preventEasyDrops' and i.id_databaseitem = s.id_databaseitem";

            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery(sql).List<string>();
                }
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
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery(sql).List<string>();
                }
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
                using (session.BeginTransaction()) {
                    return session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<ItemSetAssociation>())
                        .List<ItemSetAssociation>();
                }
            }
        }

        public List<DatabaseItemDto> GetByClass(string itemClass) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var subquery = $"SELECT {DatabaseItemStatTable.Item} FROM {DatabaseItemStatTable.Table} " +
                                   $" WHERE {DatabaseItemStatTable.Stat} = 'Class' AND {DatabaseItemStatTable.TextValue} = :itemClass";
                    var sql = string.Join(" ",
                        $" SELECT {DatabaseItemTable.Record} as Record, ",
                        $" {DatabaseItemTable.Name} as Name, ",
                        $" {DatabaseItemStatTable.Stat} as Stat, {DatabaseItemStatTable.Value} as Value, {DatabaseItemStatTable.TextValue} AS TextValue",
                        $" FROM {DatabaseItemTable.Table} i, {DatabaseItemStatTable.Table} s ",
                        $" WHERE i.{DatabaseItemTable.Id} = s.{DatabaseItemStatTable.Item} ",
                        $" AND i.{DatabaseItemTable.Id} IN ({subquery})"
                    );

                    IQuery query = session.CreateSQLQuery(sql)
                        .SetParameter("itemClass", itemClass)
                        .SetResultTransformer(Transformers.AliasToBean<InteralRowStat>());

                    return ToDto(query.List<InteralRowStat>());
                }
            }
        }

        public List<DatabaseItemDto> GetCraftableItems() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var subquery1 = string.Join(" ",
                        $"SELECT {DatabaseItemStatTable.TextValue} FROM {DatabaseItemStatTable.Table} ",
                        $"WHERE {DatabaseItemStatTable.Stat} = 'artifactName'"
                    );

                    var subquery2 = string.Join(" ",
                        $"SELECT {DatabaseItemStatTable.Item} FROM {DatabaseItemStatTable.Table} ",
                        $" WHERE {DatabaseItemStatTable.Stat} = 'Class' AND {DatabaseItemStatTable.TextValue} = 'ItemRelic'"
                    );

                    var sql = string.Join(" ",
                        $" SELECT {DatabaseItemTable.Record} as Record,",
                        $"{DatabaseItemTable.Name} as Name, {DatabaseItemStatTable.Stat} as Stat, ",
                        $"{DatabaseItemStatTable.Value} as Value, {DatabaseItemStatTable.TextValue} as TextValue",
                        $" FROM {DatabaseItemTable.Table} i, {DatabaseItemStatTable.Table} s ",
                        $" WHERE i.{DatabaseItemTable.Id} = s.{DatabaseItemStatTable.Item} ",
                        $" AND ({DatabaseItemTable.Record} IN ({subquery1}) OR i.{DatabaseItemTable.Id} IN ({subquery2}))"
                    );

                    Logger.Debug(sql);

                    IQuery query = session.CreateSQLQuery(sql)
                        .SetResultTransformer(Transformers.AliasToBean<InteralRowStat>());

                    return ToDto(query.List<InteralRowStat>());
                }
            }
        }

        private static List<DatabaseItemDto> ToDto(IList<InteralRowStat> rows) {
            List<DatabaseItemDto> items = new List<DatabaseItemDto>();

            DatabaseItemDto item = null;
            foreach (InteralRowStat row in rows) {
                // New item
                if (item == null || !item.Record.Equals(row.Record)) {
                    item = new DatabaseItemDto {
                        Record = row.Record,
                        Name = row.Name
                    };
                    items.Add(item);
                }

                item.Stats.Add(new RecipeDbStatRow {
                    Stat = row.Stat,
                    Value = row.Value,
                    TextValue = row.TextValue,
                    Parent = item
                });
            }

            return items;
        }


        public DatabaseItemDto FindDtoByRecord(string record) {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    var sql = string.Join(" ",
                        $" SELECT {DatabaseItemTable.Record} as Record, {DatabaseItemTable.Name} as Name, ",
                        $"{DatabaseItemStatTable.Stat} as Stat, {DatabaseItemStatTable.Value} as Value, ",
                        $"{DatabaseItemStatTable.TextValue} as TextValue",
                        $" FROM {DatabaseItemTable.Table} i, {DatabaseItemStatTable.Table} s ",
                        $" WHERE i.{DatabaseItemTable.Id} = s.{DatabaseItemStatTable.Item} ",
                        $" AND {DatabaseItemTable.Record} = :record "
                    );

                    IQuery query = session.CreateSQLQuery(sql)
                        .SetParameter("record", record)
                        .SetResultTransformer(Transformers.AliasToBean<InteralRowStat>());

                    foreach (DatabaseItemDto elem in ToDto(query.List<InteralRowStat>())) {
                        return elem;
                    }
                }
            }

            return null;
        }


        public IList<string> ListAllRecords() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (session.BeginTransaction()) {
                    return session.CreateCriteria<DatabaseItem>()
                        .SetProjection(Projections.Property("Record"))
                        .AddOrder(Order.Asc("Record"))
                        .List<string>();
                }
            }
        }


        public Int64 GetRowCount() {
            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    return session.CreateCriteria<DatabaseItem>()
                        .SetProjection(Projections.RowCountInt64())
                        .UniqueResult<Int64>();
                }
            }
        }


        public IList<RecipeItem> SearchForRecipeItems(ItemSearchRequest query) {
            // No idea when recipes were added, the user probably wants owned items only.
            if (query.RecentOnly) {
                return new List<RecipeItem>();
            }

            using (ISession session = SessionCreator.OpenSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    ICriteria criterias = session.CreateCriteria<RecipeItem>();


                    if (!string.IsNullOrEmpty(query.Wildcard)) {
                        criterias.Add(Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItem>()
                            .Add(Restrictions.InsensitiveLike("Name", string.Format("%{0}%", query.Wildcard.Replace(' ', '%'))))
                            .SetProjection(Projections.Property("Record"))));
                    }

                    AddItemSearchCriterias(criterias, query);
                    criterias.Add(Restrictions.Eq("IsHardcore", query.IsHardcore));

                    IList<RecipeItem> items = criterias.List<RecipeItem>();
                    return items;
                }
            }
        }

        public static void AddItemSearchCriterias(ICriteria criterias, ItemSearchRequest query) {
            if (!string.IsNullOrEmpty(query.Wildcard)) {
                criterias.Add(Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItem>()
                    .Add(Restrictions.InsensitiveLike("Name", string.Format("%{0}%", query.Wildcard.Replace(' ', '%'))))
                    .SetProjection(Projections.Property("Record"))));
            }

            if (query.IsRetaliation) {
                var subquery = Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItemStat>()
                    .Add(Restrictions.Like("Stat", "retaliation%"))
                    .CreateAlias("Parent", "P")
                    .SetProjection(Projections.Property("P.Record")));
                criterias.Add(subquery);
            }


            // Add the damage/resist filter
            foreach (string[] filter in query.Filters) {
                var subquery = Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItemStat>()
                    .Add(Restrictions.In("Stat", filter))
                    .CreateAlias("Parent", "P")
                    .SetProjection(Projections.Property("P.Record")));

                criterias.Add(subquery);
            }

            // Add the MINIMUM level requirement (if any)
            if (query.MinimumLevel > 0) {
                var subquery = Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItemStat>()
                    .Add(Restrictions.Eq("Stat", "levelRequirement"))
                    .Add(Restrictions.Ge("Value", query.MinimumLevel))
                    .CreateAlias("Parent", "P")
                    .SetProjection(Projections.Property("P.Record")));

                criterias.Add(subquery);
            }

            // Add the MAXIMUM level requirement (if any)
            if (query.MaximumLevel < 120 && query.MaximumLevel > 0) {
                var subquery = Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItemStat>()
                    .Add(Restrictions.Eq("Stat", "levelRequirement"))
                    .Add(Restrictions.Le("Value", query.MaximumLevel))
                    .CreateAlias("Parent", "P")
                    .SetProjection(Projections.Property("P.Record")));

                criterias.Add(subquery);
            }


            // Equipment Slot
            string[] slot = query.Slot;
            if (slot?.Length > 0) {
                criterias.Add(Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItemStat>()
                    .Add(Restrictions.Eq("Stat", "Class"))
                    .Add(Restrictions.In("TextValue", slot))
                    .CreateAlias("Parent", "P")
                    .SetProjection(Projections.Property("P.Record"))));
            }

            // Player Class
            string[] classes = query.Classes.ToArray();
            if (classes.Length > 0) {
                criterias.Add(Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItemStat>()
                    .Add(Restrictions.In("Stat", new string[] {
                        "augmentSkill1Extras", "augmentSkill2Extras", "augmentMastery1", "augmentMastery2",
                        "augmentSkill4Extras", "augmentSkill3Extras", "augmentMastery4", "augmentMastery3"
                    }))
                    .Add(Restrictions.In("TextValue", classes))
                    .CreateAlias("Parent", "P")
                    .SetProjection(Projections.Property("P.Record"))));
            }


            // Add the quality filter (if not set to 'any'/unknown)

            if (!string.IsNullOrEmpty(query.Rarity)) {
                string val = null;
                switch (query.Rarity) {
                    case "Yellow":
                        val = "Magical";
                        break;
                    case "Blue":
                        val = "Epic";
                        break;
                    case "Epic":
                        val = "Legendary";
                        break;
                    case "Green":
                        val = "Rare";
                        break;
                }

                if (!string.IsNullOrEmpty(val)) {
                    var subquery = Subqueries.PropertyIn("BaseRecord", DetachedCriteria.For<DatabaseItemStat>()
                        .Add(Restrictions.Eq("Stat", "itemClassification"))
                        .Add(Restrictions.Eq("TextValue", val))
                        .CreateAlias("Parent", "P")
                        .SetProjection(Projections.Property("P.Record")));
                    criterias.Add(subquery);
                }
            }


#if DEBUG
            //criterias.SetMaxResults(50);
#else
//criterias.SetMaxResults(500);
#endif
            criterias.CreateAlias("Internal", "db");
        }

        public void Clean() {
            if (Dialect == SqlDialect.Sqlite) {
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
            else {

                using (ISession session = SessionCreator.OpenSession()) {
                    using (ITransaction transaction = session.BeginTransaction()) {
                        session.CreateSQLQuery("DELETE FROM DatabaseItemStat_v2 cascade").ExecuteUpdate();
                        session.CreateSQLQuery("DELETE FROM DatabaseItem_v2 cascade").ExecuteUpdate();
                        session.CreateSQLQuery("DELETE FROM ItemTag cascade").ExecuteUpdate();
                        transaction.Commit();
                    }

                }

            }

        }
    }


}