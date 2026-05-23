using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using log4net;
using NHibernate;

namespace IAGrim.Database.Migrations {
    /// <summary>
    /// Reads all embedded .hbm.xml resources and ensures every mapped property
    /// has a corresponding column in the SQLite database. Adds missing columns via ALTER TABLE.
    /// This is idempotent and safe to run on every startup.
    /// </summary>
    class HbmSchemaMigration : IDatabaseMigration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HbmSchemaMigration));
        private static readonly XNamespace Ns = "urn:nhibernate-mapping-2.2";

        public override void Migrate(SessionFactory sessionCreator) {
            var assembly = Assembly.GetExecutingAssembly();
            var hbmResources = assembly.GetManifestResourceNames()
                .Where(r => r.EndsWith(".hbm.xml", StringComparison.OrdinalIgnoreCase));

            foreach (var resourceName in hbmResources) {
                try {
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    if (stream == null) continue;

                    var doc = XDocument.Load(stream);
                    var mappingEl = doc.Root;
                    if (mappingEl == null) continue;

                    var assemblyName = mappingEl.Attribute("assembly")?.Value;
                    var ns = mappingEl.Attribute("namespace")?.Value;

                    foreach (var classEl in mappingEl.Elements(Ns + "class")) {
                        var tableName = classEl.Attribute("table")?.Value;
                        var className = classEl.Attribute("name")?.Value;
                        if (tableName == null || className == null) continue;

                        // Skip if table doesn't exist yet (AddBaseTables will create it with all columns)
                        if (!TableExists(sessionCreator, tableName)) continue;

                        var existingColumns = GetExistingColumns(sessionCreator, tableName);
                        var clrType = ResolveType(assemblyName, ns, className);

                        foreach (var propEl in classEl.Elements(Ns + "property")) {
                            var propName = propEl.Attribute("name")?.Value;
                            if (propName == null) continue;

                            var columnName = propEl.Attribute("column")?.Value ?? propName;

                            if (existingColumns.Contains(columnName.ToLowerInvariant())) continue;

                            var sqliteType = GetSqliteType(clrType, propName);
                            var sql = $"ALTER TABLE \"{tableName}\" ADD COLUMN \"{columnName}\" {sqliteType}";

                            Logger.Info($"Adding missing column: {tableName}.{columnName} ({sqliteType})");

                            using ISession session = sessionCreator.OpenSession();
                            using ITransaction transaction = session.BeginTransaction();
                            session.CreateSQLQuery(sql).ExecuteUpdate();
                            transaction.Commit();
                        }
                    }
                } catch (Exception ex) {
                    Logger.Warn($"Failed to process HBM resource {resourceName}", ex);
                }
            }
        }

        private static HashSet<string> GetExistingColumns(SessionFactory sessionCreator, string table) {
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using ISession session = sessionCreator.OpenSession();
            foreach (var row in session.CreateSQLQuery($"PRAGMA table_info('{table}')").List()) {
                var arr = (object[])row;
                var name = (string)arr[1];
                columns.Add(name);
            }
            return columns;
        }

        private static Type? ResolveType(string? assemblyName, string? ns, string className) {
            if (assemblyName == null || ns == null) return null;
            var fullName = $"{ns}.{className}";
            try {
                var asm = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == assemblyName);
                return asm?.GetType(fullName);
            } catch {
                return null;
            }
        }

        private static string GetSqliteType(Type? clrType, string propertyName) {
            if (clrType == null) return "TEXT";

            var prop = clrType.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop == null) return "TEXT";

            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (type == typeof(string)) return "TEXT";
            if (type == typeof(long) || type == typeof(int) || type == typeof(short) ||
                type == typeof(byte) || type == typeof(bool) || type == typeof(Int64) ||
                type == typeof(ulong) || type == typeof(uint)) return "INTEGER";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "REAL";

            return "TEXT";
        }
    }
}

