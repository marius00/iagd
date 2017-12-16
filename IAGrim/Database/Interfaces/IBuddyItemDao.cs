using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.BuddyShare.dto;
using IAGrim.Database.Dto;

namespace IAGrim.Database.Interfaces {
    public interface IBuddyItemDao : IBaseDao<BuddyItem> {
        void SetItems(long userid, string description, List<JsonBuddyItem> items);
        void RemoveBuddy(long buddyId);
        IList<BuddyItem> ListItemsWithMissingRarity();
        IList<BuddyItem> ListItemsWithMissingLevelRequirement();
        IList<BuddyItem> ListItemsWithMissingName();

        void UpdateLevelRequirements(IList<BuddyItem> items);
        void UpdateRarity(IList<BuddyItem> items);
        void UpdateNames(IList<BuddyItem> items);
        IList<BuddyItem> FindBy(Search query);
    }
}
