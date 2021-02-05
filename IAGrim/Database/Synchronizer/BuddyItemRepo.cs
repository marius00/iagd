using IAGrim.Database.Interfaces;
using System.Collections.Generic;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Dto;
using IAGrim.Database.Synchronizer.Core;

namespace IAGrim.Database.Synchronizer {
    class BuddyItemRepo : BasicSynchronizer<BuddyItem>, IBuddyItemDao {
        private readonly IBuddyItemDao _repo;
        public BuddyItemRepo(ThreadExecuter threadExecuter, ISessionCreator sessionCreator, SqlDialect dialect) : base(threadExecuter, sessionCreator) {
            _repo = new BuddyItemDaoImpl(sessionCreator, new DatabaseItemStatDaoImpl(sessionCreator, dialect), dialect);
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

        public IList<string> GetOnlineIds(BuddySubscription subscription) {
            return ThreadExecuter.Execute(
                () => _repo.GetOnlineIds(subscription)
            );
        }

        public void Save(BuddySubscription subscription, List<BuddyItem> items) {
            ThreadExecuter.Execute(
                () => _repo.Save(subscription, items)
            );
        }

        public void Delete(BuddySubscription subscription, List<DeleteItemDto> items) {
            ThreadExecuter.Execute(
                () => _repo.Delete(subscription, items)
            );
        }

        public void RemoveBuddy(long buddyId) {
            ThreadExecuter.Execute(
                () => _repo.RemoveBuddy(buddyId)
            );
        }
    }
}
