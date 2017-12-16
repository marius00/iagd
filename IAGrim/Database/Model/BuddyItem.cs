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
        public long Id { get; set; }
        private DatabaseItem Internal { get; set; }

        public bool IsKnown => Internal != null;

        public float MinimumLevel { get; set; }



        public PlayerItemSkill Skill { get; } = null;

        public string Bitmap => DatabaseItem.GetBitmap(Tags);

        public string Slot => DatabaseItem.GetSlot(Tags);

        public string Rarity { get; set; }

        public List<SkillModifierStat> ModifiedSkills { get; } = new List<SkillModifierStat>();
        
        public virtual int PrefixRarity => 0;

        public virtual string Stash { get; set; }

        public string Name { get; set; }

        public uint Count { get; set; }



        public int CompareTo(object obj) {
            PlayerHeldItem item = obj as PlayerHeldItem;

            if (item != null) {
                if (Name != null && item.Name != null) {
                    return Name.CompareTo(item.Name);
                }
                return Id.CompareTo(item.Id);
            }

            return 0;
        }

        public bool IsRecipe => false;

        public bool HasRecipe { get; set; }

        public List<string> Buddies { get; set; } = new List<string>();
    }
}

