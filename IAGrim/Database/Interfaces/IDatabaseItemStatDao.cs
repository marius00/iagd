using System.Collections.Generic;
using IAGrim.Database.DAO.Dto;
using IAGrim.Services.Dto;
using NHibernate;

namespace IAGrim.Database.Interfaces {
    public interface IDatabaseItemStatDao : IBaseDao<DatabaseItemStat> {
        Dictionary<string, List<DBSTatRow>> GetStats(ISession session, StatFetch fetchMode);
        Dictionary<string, List<DBSTatRow>> GetStats(IEnumerable<string> records, StatFetch fetchMode);
        Dictionary<long, List<DBSTatRow>> GetStats(List<long> records, StatFetch fetchMode);
        Dictionary<string, string> MapItemBitmaps(List<string> records);

        Dictionary<string, ISet<DBSTatRow>> GetExpacSkillModifierSkills();

        string GetSkillName(string skillRecord);
    }
}