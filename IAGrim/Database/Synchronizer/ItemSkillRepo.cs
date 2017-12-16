using System.Collections.Generic;
using IAGrim.Database.DAO;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Model;
using IAGrim.Parsers.Arz.dto;

namespace IAGrim.Database.Synchronizer {
    internal class ItemSkillRepo : IItemSkillDao {
        private readonly IItemSkillDao _repo;
        private readonly ThreadExecuter _threadExecuter;

        public ItemSkillRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) {
            _repo = new ItemSkillDaoImpl(sessionCreator);
            _threadExecuter = threadExecuter;
        }

        public void Save(ISet<ItemGrantedSkill> skills, bool additive) {
            _threadExecuter.Execute(
                () => _repo.Save(skills, additive)
            );
        }

        public void Save(Dictionary<string, List<string>> skillItemMapping, bool additive) {
            _threadExecuter.Execute(
                () => _repo.Save(skillItemMapping, additive)
            );
        }

        public IList<PlayerItemSkill> List() {
            return _threadExecuter.Execute(
                () => _repo.List()
            );
        }

        public void EnsureCorrectSkillRecords() {
            _threadExecuter.Execute(
                () => _repo.EnsureCorrectSkillRecords()
            );
        }
    }
}