using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using StatTranslator;

namespace IAGrim.Database.Model {
    public class AugmentationItem : BaseItem, PlayerHeldItem, RecordCollection {
        public virtual int CompareTo(object obj) {
            if (obj is PlayerHeldItem item) {
                if (Name != null && item.Name != null) {
                    return Name.CompareTo(item.Name);
                }
                return Id.CompareTo(item.Id);
            }

            return 0;
        }

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Rarity { get; set; }
        public virtual float MinimumLevel { get; set; }


        public virtual string Stash => string.Empty;
        public virtual bool IsRecipe => false;
        public virtual bool HasRecipe { get; set; } = false;
        public virtual List<string> Buddies { get; set; } = new List<string>();
        public virtual uint Count { get; set; } = 0;
        public virtual int PrefixRarity => 0;

        public virtual string Slot => Tags.FirstOrDefault(m => "Class".Equals(m.Stat))?.TextValue ?? string.Empty;

        public virtual bool IsKnown => true;
        public virtual List<SkillModifierStat> ModifiedSkills => new List<SkillModifierStat>();
        public virtual string Bitmap => Parser.Helperclasses.ParserHelpers<DBSTatRow>.GetBitmap(Tags);
        public virtual PlayerItemSkill Skill => null;

        private DatabaseItem Internal { get; set; } // Its used - Check HBM
    }
}
