using System.Collections.Generic;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Interfaces {
    public interface IBuddyItemDao : IBaseDao<BuddyItem> {
        void RemoveBuddy(long buddyId);
        IList<BuddyItem> ListItemsWithMissingDetails();
        void UpdateItemDetails(IList<BuddyItem> items);
        IList<BuddyItem> FindBy(ItemSearchRequest query);

        long GetNumItems(long subscriptionId);
        IList<string> GetOnlineIds(BuddySubscription subscription);
        void Save(BuddySubscription subscription, List<BuddyItem> items);
        void Delete(BuddySubscription subscription, List<DeleteItemDto> items);

        IList<BuddyItem> ListMissingReplica();

        void Delete();
    }
}
