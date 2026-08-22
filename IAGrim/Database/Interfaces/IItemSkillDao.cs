using System.Collections.Generic;
using IAGrim.Database.Model;
using IAGrim.Parsers.Arz.dto;

namespace IAGrim.Database.Interfaces {
    public interface IItemSkillDao {
        void Save(ISet<ItemGrantedSkill> skills, bool additive);

        void Save(Dictionary<string, List<string>> skillItemMapping, bool additive);

        IList<PlayerItemSkill> ListForRecords(IEnumerable<string> baseRecords);
        void EnsureCorrectSkillRecords();
    }
}