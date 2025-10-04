using IAGrim.Database.Interfaces;
using IAGrim.Database.DAO;
using IAGrim.Database.Synchronizer.Core;
using IAGrim.Parsers.GameDataParsing.Model;

namespace IAGrim.Database.Synchronizer {
    class ItemTagRepo : BasicSynchronizer<ItemTag>, IItemTagDao {
        private readonly ItemTagDaoImpl repo;
        public ItemTagRepo(ThreadExecuter threadExecuter, SessionFactory sessionCreator) : base(threadExecuter, sessionCreator) {
            this.repo = new ItemTagDaoImpl(sessionCreator);
            this.BaseRepo = repo;
        }


        public void Save(ICollection<ItemTag> items, ProgressTracker tracker) {
            ThreadExecuter.Execute(
                () => repo.Save(items, tracker)
            );
        }

        public IList<ItemTag> GetClassItemTags() {
            return ThreadExecuter.Execute(
                () => repo.GetClassItemTags()
            );

        }
        public ISet<ItemTag> GetValidClassItemTags() {
            return ThreadExecuter.Execute(
                () => repo.GetValidClassItemTags()
            );
        }
    }
}
