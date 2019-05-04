using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Model;
using IAGrim.Parsers.GameDataParsing.Model;

namespace IAGrim.Database.Synchronizer {
    class DatabaseItemRepo : BasicSynchronizer<DatabaseItem>, IDatabaseItemDao {
        private readonly IDatabaseItemDao _repo;
        public DatabaseItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this._repo = new DatabaseItemDaoImpl(sessionCreator);
            this.BaseRepo = _repo;
        }


        public DatabaseItem FindByRecord(string record) {
            return ThreadExecuter.Execute(
                () => _repo.FindByRecord(record)
            );
        }

        public long GetRowCount() {
            return ThreadExecuter.Execute(
                () => _repo.GetRowCount()
            );
        }

        public IList<ItemSetAssociation> GetItemSetAssociations() {
            return ThreadExecuter.Execute(
                () => _repo.GetItemSetAssociations()
            );
        }

        public IList<string> GetSpecialStackableRecords() {
            return ThreadExecuter.Execute(
                () => _repo.GetSpecialStackableRecords()
            );
        }

        public IList<string> GetStackableComponentsPotionsMisc() {
            return ThreadExecuter.Execute(
                () => _repo.GetStackableComponentsPotionsMisc()
            );
        }


        public DatabaseItemDto FindDtoByRecord(string record) {
            return ThreadExecuter.Execute(
                () => _repo.FindDtoByRecord(record)
            );
        }

        public List<DatabaseItemDto> GetCraftableItems() {
            return ThreadExecuter.Execute(
                () => _repo.GetCraftableItems()
            );
        }

        public List<DatabaseItemDto> GetByClass(string itemClass) {
            return ThreadExecuter.Execute(
                () => _repo.GetByClass(itemClass)
            );
        }

        public Dictionary<string, string> GetTagDictionary() {
            return ThreadExecuter.Execute(
                () => _repo.GetTagDictionary()
            );
        }

        public IList<string> ListAllRecords() {
            return ThreadExecuter.Execute(
                () => _repo.ListAllRecords()
            );
        }

        public void Save(DatabaseItem item) {
            ThreadExecuter.Execute(
                () => _repo.Save(item)
            );
        }

        public void Save(ICollection<DatabaseItem> items) {
            ThreadExecuter.Execute(
                () => _repo.Save(items), 
                ThreadExecuter.ThreadTimeout * 2
            );
        }
        public void Save(List<DatabaseItem> items, ProgressTracker progressTracker) {
            ThreadExecuter.Execute(
                () => _repo.Save(items, progressTracker),
                ThreadExecuter.ThreadTimeout * 2
            );
        }
        
        public void SaveOrUpdate(ICollection<DatabaseItem> items) {
            ThreadExecuter.Execute(
                () => _repo.SaveOrUpdate(items),
                ThreadExecuter.ThreadTimeout * 2
            );
        }

        public IList<RecipeItem> SearchForRecipeItems(ItemSearchRequest query) {
            return ThreadExecuter.Execute(
                () => _repo.SearchForRecipeItems(query)
            );
        }
    }
}
