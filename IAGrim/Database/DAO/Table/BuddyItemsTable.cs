using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database.DAO.Table {
    static class BuddyItemsTable {
        public const string Table = "buddyitems_v6";
        public const string SubscriptionId = "id_buddy"; // Subscriber
        public const string BaseRecord = "baserecord";
        public const string PrefixRecord = "prefixrecord";
        public const string SuffixRecord = "suffixrecord";
        public const string ModifierRecord = "modifierrecord";
        public const string TransmuteRecord = "transmuterecord";
        public const string MateriaRecord = "materiarecord";
        public const string StackCount = "stackcount";
        public const string IsHardcore = "ishardcore";
        public const string Mod = "mod";
        public const string Name = "name";
        public const string NameLowercase = "namelowercase";
        public const string LevelRequirement = "levelrequirement";
        public const string Rarity = "rarity";
        public const string RemoteItemId = "id_item_remote"; // Remote ID
        public const string CreatedAt = "created_at";
    }
}
