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
        [Obsolete] // Absolutely do not use
        public virtual long Id { get; set; }
        private DatabaseItem Internal { get; set; }
        public virtual bool IsKnown => Internal != null;
        public virtual float MinimumLevel { get; set; }
        public virtual PlayerItemSkill Skill { get; } = null;
        public virtual string Bitmap => DatabaseItem.GetBitmap(Tags);
        public virtual string Slot => DatabaseItem.GetSlot(Tags);
        public virtual string Rarity { get; set; }
        public virtual List<SkillModifierStat> ModifiedSkills { get; } = new List<SkillModifierStat>();
        
        public virtual long PrefixRarity { get; set; }
        public virtual string Stash { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameLowercase { get; set; } // To help with case insensitive search on non-ascii characters
        public virtual long StackCount { get; set; }
        public virtual long CreationDate { get; set; }


        public virtual ulong Count {
            get { return (ulong)StackCount; }
            set { StackCount = (long)value; }
        }

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

        // Online UUID
        public virtual string RemoteItemId { get; set; }

        /// <summary>
        /// SubscriptionId / ID of the buddy
        /// </summary>
        public virtual long BuddyId { get; set; }
        
        public virtual string BaseRecord { get; set; }
        public virtual string PrefixRecord { get; set; }
        public virtual string SuffixRecord { get; set; }
        public virtual string ModifierRecord { get; set; }
        public virtual string TransmuteRecord { get; set; }
        public virtual string MateriaRecord { get; set; }
        public virtual bool IsHardcore { get; set; }
        public virtual string Mod { get; set; }



        public override bool Equals(object obj) {
            if (obj is BuddyItem that) {
                return this.RemoteItemId == that.RemoteItemId && this.BuddyId == that.BuddyId;
            }
            else if (obj is PlayerItem pi) {
                return this.BaseRecord == pi.BaseRecord && this.SuffixRecord == pi.SuffixRecord && this.PrefixRecord == pi.PrefixRecord && this.ModifierRecord == pi.ModifierRecord;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode() {
            return (RemoteItemId + BuddyId).GetHashCode();
        }
    }
}

