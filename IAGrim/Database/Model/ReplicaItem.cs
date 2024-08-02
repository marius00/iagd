using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Database {
    /// <summary>
    /// A replica item is basically just the minimum parameters needed to create an item,
    /// along with its real item stats.
    ///
    /// The stats are stored in "Text" as a JSON array.
    /// </summary>
    public class ReplicaItem {
        public virtual long Id { get; set; }
        public virtual long? PlayerItemId { get; set; }
        public virtual string BuddyItemId { get; set; }
        public virtual string BaseRecord { get; set; } = "";
        public virtual string PrefixRecord { get; set; } = "";
        public virtual string SuffixRecord { get; set; } = "";
        public virtual string ModifierRecord { get; set; } = "";
        public virtual string TransmuteRecord { get; set; } = "";
        public virtual uint Seed { get; set; } = 0u;
        public virtual string MateriaRecord { get; set; } = "";
        public virtual string RelicCompletionBonusRecord { get; set; } = "";
        public virtual uint RelicSeed { get; set; } = 0u;
        public virtual string EnchantmentRecord { get; set; } = "";
        public virtual uint EnchantmentSeed { get; set; } = 0u;
    }
}
