using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Database.Synchronizer.Core;
using IAGrim.Parsers.GameDataParsing.Model;

namespace IAGrim.Database.Synchronizer {
    class DatabaseItemRepo : BasicSynchronizer<DatabaseItem>, IDatabaseItemDao {
        private readonly IDatabaseItemDao _repo;
        public DatabaseItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            this._repo = new DatabaseItemDaoImpl(sessionCreator, dialect);
            this.BaseRepo = _repo;
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

        public void Save(ICollection<DatabaseItem> items) {
            ThreadExecuter.Execute(
                () => _repo.Save(items), 
                ThreadExecuter.ThreadTimeout * 2,
                true
            );
        }

        public void Save(List<DatabaseItem> items, ProgressTracker progressTracker) {
            ThreadExecuter.Execute(
                () => _repo.Save(items, progressTracker),
                ThreadExecuter.ThreadTimeout * 2,
                true
            );
        }
        
        public void SaveOrUpdate(ICollection<DatabaseItem> items) {
            ThreadExecuter.Execute(
                () => _repo.SaveOrUpdate(items),
                ThreadExecuter.ThreadTimeout * 2,
                true
            );
        }


        public void CreateItemIndexes(ProgressTracker progressTracker) {
            ThreadExecuter.Execute(
                () => _repo.CreateItemIndexes(progressTracker),
                ThreadExecuter.ThreadTimeout * 2,
                true
            );
        }


        public void Clean() {
            ThreadExecuter.Execute(
                () => _repo.Clean()
            );
        }
    }
}
