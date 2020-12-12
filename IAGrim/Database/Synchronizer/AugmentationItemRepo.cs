using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.BuddyShare.dto;
using IAGrim.Database.DAO;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Dto;
using IAGrim.Database.Model;

namespace IAGrim.Database.Synchronizer {
    public class AugmentationItemRepo : BasicSynchronizer<AugmentationItem>, IAugmentationItemDao {
        private readonly IAugmentationItemDao _repo;
        public AugmentationItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, IDatabaseItemStatDao databaseItemStatDao, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            _repo = new AugmentationItemDaoImpl(sessionCreator, databaseItemStatDao, dialect);
            this.BaseRepo = _repo;
        }

        public void UpdateState() {
            ThreadExecuter.Execute(
                () => _repo.UpdateState()
            );
        }

        public IList<AugmentationItem> Search(ItemSearchRequest query) {
            return ThreadExecuter.Execute(
                () => _repo.Search(query)
            );
        }
    }
}
