using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Model;

namespace IAGrim.Database.Synchronizer {
    class DatabaseItemRepo : BasicSynchronizer<DatabaseItem>, IDatabaseItemDao {
        private readonly IDatabaseItemDao repo;
        public DatabaseItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator) : base(threadExecuter, sessionCreator) {
            this.repo = new DatabaseItemDaoImpl(sessionCreator);
            this.BaseRepo = repo;
        }

        public DatabaseItem FindByRecord(string record) {
            return ThreadExecuter.Execute(
                () => repo.FindByRecord(record)
            );
        }

        public long GetRowCount() {
            return ThreadExecuter.Execute(
                () => repo.GetRowCount()
            );
        }


        public IList<ItemTag> GetClassItemTags() {
            return ThreadExecuter.Execute(
                () => repo.GetClassItemTags()
            );

        }
        public IList<ItemTag> GetValidClassItemTags() {
            return ThreadExecuter.Execute(
                () => repo.GetValidClassItemTags()
            );
        }

        public DatabaseItemDto FindDtoByRecord(string record) {
            return ThreadExecuter.Execute(
                () => repo.FindDtoByRecord(record)
            );
        }

        public List<DatabaseItemDto> GetCraftableItems() {
            return ThreadExecuter.Execute(
                () => repo.GetCraftableItems()
            );
        }

        public List<DatabaseItemDto> GetByClass(string itemClass) {
            return ThreadExecuter.Execute(
                () => repo.GetByClass(itemClass)
            );
        }

        public Dictionary<string, string> GetTagDictionary() {
            return ThreadExecuter.Execute(
                () => repo.GetTagDictionary()
            );
        }

        public IList<string> ListAllRecords() {
            return ThreadExecuter.Execute(
                () => repo.ListAllRecords()
            );
        }

        public void Save(DatabaseItem item) {
            ThreadExecuter.Execute(
                () => repo.Save(item)
            );
        }

        public void Save(ICollection<DatabaseItem> items) {
            ThreadExecuter.Execute(
                () => repo.Save(items), 
                ThreadExecuter.ThreadTimeout * 2
            );
        }

        public void Save(ICollection<ItemTag> items) {
            ThreadExecuter.Execute(
                () => repo.Save(items)
            );
        }

        public void SaveOrUpdate(ICollection<DatabaseItem> items) {
            ThreadExecuter.Execute(
                () => repo.SaveOrUpdate(items),
                ThreadExecuter.ThreadTimeout * 2
            );
        }

        public void SaveOrUpdate(ICollection<ItemTag> items) {
            ThreadExecuter.Execute(
                () => repo.SaveOrUpdate(items)
            );
        }

        public IList<RecipeItem> SearchForRecipeItems(Search query) {
            return ThreadExecuter.Execute(
                () => repo.SearchForRecipeItems(query)
            );
        }
    }
}
