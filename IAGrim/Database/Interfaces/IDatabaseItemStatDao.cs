using System.Collections.Generic;
using IAGrim.Database.DAO.Dto;
using IAGrim.Parsers.GameDataParsing.Model;
using IAGrim.Services.Dto;
using NHibernate;

namespace IAGrim.Database.Interfaces {
    public interface IDatabaseItemStatDao : IBaseDao<DatabaseItemStat> {
        Dictionary<string, List<DBStatRow>> GetStats(ISession session, StatFetch fetchMode);
        Dictionary<string, List<DBStatRow>> GetStats(IEnumerable<string> records, StatFetch fetchMode);
        Dictionary<long, List<DBStatRow>> GetStats(List<long> records, StatFetch fetchMode);

        /// <summary>
        /// Streams every record's translatable stats, keyed by record, for building the free-text
        /// search index. Unlike <see cref="GetStats(ISession, StatFetch)"/> this has no upper-bound
        /// safety guard, as it is expected to cover the entire item database.
        /// </summary>
        Dictionary<string, List<DBStatRow>> GetAllRecordStatsForIndexing();
        Dictionary<string, string> MapItemBitmaps(List<string> records);
        void Save(IEnumerable<DatabaseItemStat> objs, ProgressTracker progressTracker);
    }
}