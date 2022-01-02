using IAGrim.Database.Interfaces;
using IAGrim.Database;
using IAGrim.Parsers.Arz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DataAccess;
using IAGrim.Database.Model;
using StatTranslator;
using IAGrim.Utilities;
using IAGrim.Services.Dto;

namespace IAGrim.Database {
    public class PlayerItem : BaseItem, PlayerHeldItem, IComparable, ICloneable, RecordCollection {
        public virtual long Id { get; set; }

        [Obsolete]
        public virtual string AzureUuid { get; set; }

        public virtual string CloudId { get; set; }

        public virtual bool IsCloudSynchronized {
            get => IsCloudSynchronizedValue != null && IsCloudSynchronizedValue != 0;
            set => IsCloudSynchronizedValue = value ? 1 : 0;
        }
        public virtual System.Int64 IsCloudSynchronizedValue { get; set; }

        public virtual bool IsKnown => Tags != null && Tags.Count > 0;
        
        public virtual string Bitmap => Parser.Helperclasses.ParserHelpers<DBStatRow>.GetBitmap(Tags);

        public virtual string Rarity { get; set; }
        public string Mod { get; set; }
        public virtual bool IsHardcore { get; set; }

        public bool IsExpansion1 { get; set; }

        public virtual double LevelRequirement { get; set; }

        /// <summary>
        /// Get the minimum level to use this item
        /// </summary>
        public virtual float MinimumLevel {
            get { return (float) LevelRequirement; }
            set { LevelRequirement = value; }
        }

        public virtual string Slot {
            get {
                if (Tags.Any(m => "Class".Equals(m.Stat)))
                    return Tags.FirstOrDefault(m => "Class".Equals(m.Stat)).TextValue;
                else
                    return string.Empty;
            }
        }

        public virtual PlayerItemSkill Skill { get; set; }


        private DatabaseItem Internal { get; set; }


        public virtual List<SkillModifierStat> ModifiedSkills { get; } = new List<SkillModifierStat>();


        public virtual long Seed { get; set; }

        public virtual uint USeed {
            get {
                unchecked {
                    return (uint) (int) Seed;
                }
            }
        }

        public virtual string RelicCompletionBonusRecord { get; set; }
        public virtual long RelicSeed { get; set; }
        public virtual long UNKNOWN { get; set; }
        public virtual long EnchantmentSeed { get; set; }
        public virtual long MateriaCombines { get; set; }
        public virtual long StackCount { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameLowercase { get; set; } // To help with case insensitive search on non-ascii characters
        public virtual long PrefixRarity { get; set; }

        public virtual string Stash => string.Empty;


        public virtual ulong Count {
            get { return (ulong) StackCount; }
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

        public virtual bool IsRecipe {
            get { return false; }
        }


        public virtual bool HasRecipe { get; set; }


        public virtual object Clone() {
            return this.MemberwiseClone();
        }

        public virtual long? CreationDate { get; set; }
        public virtual string SearchableText { get; set; }
        public virtual string ReplicaInfo { get; set; }
    }
}