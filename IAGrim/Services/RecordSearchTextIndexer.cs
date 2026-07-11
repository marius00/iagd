using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using IAGrim.Database;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using log4net;
using NHibernate;
using StatTranslator;

namespace IAGrim.Services {
    /// <summary>
    /// Builds the <see cref="RecordStatTextTable"/> lookup table that powers free-text stat search.
    ///
    /// For every item record it renders the record's stats through the same <see cref="StatManager"/>
    /// used for display, but with the roll-dependent numeric placeholders ({0}/{1}/{2}/{4}) blanked
    /// out — so the resulting text is seed-independent and one row per record covers every item that
    /// shares that record. Text placeholders ({3}/{5}/{6}: damage types, skill/race names) are kept,
    /// so lines like "Pierce Damage converted to Physical" remain searchable.
    ///
    /// The index reflects the active language pack, so it is rebuilt whenever the game database is
    /// (re)parsed or the language changes.
    /// </summary>
    public class RecordSearchTextIndexer {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RecordSearchTextIndexer));

        // Reserved record key holding the language the index was last built with. No real item
        // references this record, so it never produces a false search match.
        private const string LanguageStampRecord = "__index_language__";

        private readonly SessionFactory _sessionFactory;
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        private readonly object _lock = new object();

        public RecordSearchTextIndexer(SessionFactory sessionFactory, IDatabaseItemStatDao databaseItemStatDao) {
            _sessionFactory = sessionFactory;
            _databaseItemStatDao = databaseItemStatDao;
        }

        /// <summary>
        /// Builds the index only if it is missing or was built for a different language than the one
        /// given. Cheap no-op on a normal startup where nothing changed. Language changes trigger an
        /// app restart, so this startup check is what keeps the index in sync with the active pack.
        /// </summary>
        public void EnsureBuilt(string languageCode) {
            lock (_lock) {
                if (IsUpToDate(languageCode)) {
                    Logger.Debug("Free-text stat search index is up to date, skipping rebuild");
                    return;
                }
                RebuildInternal(languageCode);
            }
        }

        /// <summary>
        /// Rebuilds the entire search-text index from the current game database and active language.
        /// Call after the game database is (re)parsed.
        /// </summary>
        public void Rebuild(string languageCode) {
            lock (_lock) {
                RebuildInternal(languageCode);
            }
        }

        private void RebuildInternal(string languageCode) {
            try {
                var statManager = RuntimeSettings.StatManager;
                if (statManager == null) {
                    Logger.Warn("Cannot build search-text index: no language/StatManager loaded yet");
                    return;
                }

                Logger.Debug("Building free-text stat search index..");
                var statsByRecord = _databaseItemStatDao.GetAllRecordStatsForIndexing();

                var rows = new List<(string Record, string Text)>(statsByRecord.Count);
                foreach (var kv in statsByRecord) {
                    var text = BuildSearchText(statManager, kv.Value);
                    if (!string.IsNullOrEmpty(text))
                        rows.Add((kv.Key, text));
                }

                Store(rows, languageCode);
                Logger.Debug($"Free-text stat search index built with {rows.Count} records");
            }
            catch (Exception ex) {
                Logger.Warn("Failed to build free-text stat search index", ex);
            }
        }

        private bool IsUpToDate(string languageCode) {
            try {
                using ISession session = _sessionFactory.OpenSession();
                var stamp = session.CreateSQLQuery(
                        $"SELECT {RecordStatTextTable.SearchText} FROM {RecordStatTextTable.Table} WHERE {RecordStatTextTable.Record} = :record")
                    .SetParameter("record", LanguageStampRecord)
                    .UniqueResult<string>();
                return stamp != null && stamp == (languageCode ?? string.Empty);
            }
            catch (Exception ex) {
                Logger.Warn("Could not read search-index language stamp, will rebuild", ex);
                return false;
            }
        }

        private static string BuildSearchText(StatManager statManager, List<DBStatRow> rows) {
            var stats = new HashSet<IItemStat>(rows);
            var lines = statManager.ProcessStats(stats, TranslatedStatType.BODY)
                .Concat(statManager.ProcessStats(stats, TranslatedStatType.HEADER));

            var sb = new StringBuilder();
            foreach (var line in lines) {
                var text = RenderWildcarded(line);
                if (!string.IsNullOrWhiteSpace(text)) {
                    if (sb.Length > 0) sb.Append('\n');
                    sb.Append(text.Trim());
                }
            }

            return sb.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Renders a translated stat line with the roll-dependent numeric placeholders treated as
        /// wildcards (blanked), keeping the text placeholders. TranslatedStat.ToString substitutes a
        /// null numeric param with an empty string, so nulling Param0/1/2/4 yields the number-free
        /// literal text (e.g. "+% Chance to Avoid Melee Attacks").
        /// </summary>
        private static string RenderWildcarded(TranslatedStat stat) {
            var copy = new TranslatedStat {
                Text = stat.Text,
                Param0 = null,
                Param1 = null,
                Param2 = null,
                Param3 = stat.Param3,
                Param4 = null,
                Param5 = stat.Param5,
                Param6 = stat.Param6,
                Type = stat.Type,
            };
            return copy.ToString();
        }

        private void Store(List<(string Record, string Text)> rows, string languageCode) {
            using ISession session = _sessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();

            session.CreateSQLQuery($"DELETE FROM {RecordStatTextTable.Table}").ExecuteUpdate();

            var insert = $"INSERT INTO {RecordStatTextTable.Table} ({RecordStatTextTable.Record}, {RecordStatTextTable.SearchText}) VALUES (:record, :text)";
            foreach (var row in rows) {
                session.CreateSQLQuery(insert)
                    .SetParameter("record", row.Record)
                    .SetParameter("text", row.Text)
                    .ExecuteUpdate();
            }

            // Stamp the language the index was built with (see EnsureBuilt).
            session.CreateSQLQuery(insert)
                .SetParameter("record", LanguageStampRecord)
                .SetParameter("text", languageCode ?? string.Empty)
                .ExecuteUpdate();

            transaction.Commit();
        }
    }
}
