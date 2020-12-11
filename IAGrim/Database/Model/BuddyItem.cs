using DataAccess;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using IAGrim.Database.Model;

namespace IAGrim.Database {
    public class BuddyItem : BaseItem, PlayerHeldItem {
        public virtual long Id { get; set; }
        private DatabaseItem Internal { get; set; }
        public virtual bool IsKnown => Internal != null;
        public virtual float MinimumLevel { get; set; }
        public virtual PlayerItemSkill Skill { get; } = null;
        public virtual string Bitmap => DatabaseItem.GetBitmap(Tags);
        public virtual string Slot => DatabaseItem.GetSlot(Tags);
        public virtual string Rarity { get; set; }
        public virtual List<SkillModifierStat> ModifiedSkills { get; } = new List<SkillModifierStat>();
        
        public virtual long PrefixRarity => 0;
        public virtual string Stash { get; set; }
        public virtual string Name { get; set; }
        public virtual ulong Count { get; set; }
        public virtual long CreationDate { get; set; }


        public virtual int CompareTo(object obj) {
            PlayerHeldItem item = obj as PlayerHeldItem;

            if (item != null) {
                if (Name != null && item.Name != null) {
                    return Name.CompareTo(item.Name);
                }
                return Id.CompareTo(item.Id);
            }

            return 0;
        }

        public virtual bool IsRecipe => false;
        public virtual bool HasRecipe { get; set; }
        public virtual List<string> Buddies { get; set; } = new List<string>();

        public virtual long RemoteItemId { get; set; }
        public virtual long BuddyId { get; set; }
        public virtual string BaseRecord { get; set; }
        public virtual string PrefixRecord { get; set; }
        public virtual string SuffixRecord { get; set; }
        public virtual string ModifierRecord { get; set; }
        public virtual string TransmuteRecord { get; set; }
        public virtual string MateriaRecord { get; set; }
        public virtual bool IsHardcore { get; set; }
        public virtual string Mod { get; set; }
    }
}

