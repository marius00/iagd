using IAGrim.Database.Interfaces;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Database.Synchronizer.Core;
using System.Security.Policy;
using System.Collections.Generic;

namespace IAGrim.Database.Synchronizer {
    class ItemReplicaRepo : BasicSynchronizer<ReplicaItem>, IReplicaItemDao {
        private readonly ReplicaItemDaoImpl repo;
        public ItemReplicaRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            repo = new ReplicaItemDaoImpl(sessionCreator, dialect);
            this.BaseRepo = repo;
        }

        public void Save(ReplicaItem obj, List<ReplicaItemRow> rows) {
            ThreadExecuter.Execute(
                () => repo.Save(obj, rows)
            );
        }

        public IList<long> GetPlayerItemIds() {
            return ThreadExecuter.Execute(
                () => repo.GetPlayerItemIds()
            );
        }

        public IList<string> GetBuddyItemIds() {
            return ThreadExecuter.Execute(
                () => repo.GetBuddyItemIds()
            );
        }

        public void DeleteAll() {
            ThreadExecuter.Execute(
                () => repo.DeleteAll()
            );
        }
    }
}
