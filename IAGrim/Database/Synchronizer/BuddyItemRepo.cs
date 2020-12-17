using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.BuddyShare.dto;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Dto;
using IAGrim.Database.Synchronizer.Core;

namespace IAGrim.Database.Synchronizer {
    class BuddyItemRepo : BasicSynchronizer<BuddyItem>, IBuddyItemDao {
        private readonly IBuddyItemDao _repo;
        public BuddyItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            _repo = new BuddyItemDaoImpl(sessionCreator, dialect);
            this.BaseRepo = _repo;
        }

        public IList<BuddyItem> ListItemsWithMissingRarity() {
            return ThreadExecuter.Execute(
                () => _repo.ListItemsWithMissingRarity()
            );
        }

        public IList<BuddyItem> ListItemsWithMissingLevelRequirement() {
            return ThreadExecuter.Execute(
                () => _repo.ListItemsWithMissingLevelRequirement()
            );
        }

        public IList<BuddyItem> ListItemsWithMissingName() {
            return ThreadExecuter.Execute(
                () => _repo.ListItemsWithMissingName()
            );
        }

        public void UpdateLevelRequirements(IList<BuddyItem> items) {
            ThreadExecuter.Execute(
                () => _repo.UpdateLevelRequirements(items)
            );

        }

        public void UpdateRarity(IList<BuddyItem> items) {
            ThreadExecuter.Execute(
                () => _repo.UpdateRarity(items)
            );
        }

        public void UpdateNames(IList<BuddyItem> items) {
            ThreadExecuter.Execute(
                () => _repo.UpdateNames(items)
            );
        }

        public IList<BuddyItem> FindBy(ItemSearchRequest query) {
            return ThreadExecuter.Execute(
                () => _repo.FindBy(query)
            );
        }

        public long GetNumItems(long subscriptionId) {
            return ThreadExecuter.Execute(
                () => _repo.GetNumItems(subscriptionId)
            );
        }

        public BuddyStash GetBySubscriptionId(long subscriptionId) {
            return ThreadExecuter.Execute(
                () => _repo.GetBySubscriptionId(subscriptionId)
            );
        }

        public void RemoveBuddy(long buddyId) {
            ThreadExecuter.Execute(
                () => _repo.RemoveBuddy(buddyId)
            );
        }

        public void SetItems(long userid, string description, List<JsonBuddyItem> items) {
            ThreadExecuter.Execute(
                () => _repo.SetItems(userid, description, items)
            );
        }
    }
}
