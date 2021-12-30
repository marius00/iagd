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
        Dictionary<string, string> MapItemBitmaps(List<string> records);
        void Save(IEnumerable<DatabaseItemStat> objs, ProgressTracker progressTracker);

        string GetSkillName(string skillRecord);
        Dictionary<string, float> GetSkillTiers();
    }
}