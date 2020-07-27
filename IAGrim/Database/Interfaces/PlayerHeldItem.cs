using IAGrim.Services.Dto;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IAGrim.Database.Model;

namespace IAGrim.Database.Interfaces {
    public interface PlayerHeldItem : IComparable {
        Int64 Id { get; }
        string Stash { get; }
        bool IsRecipe { get; }
        bool HasRecipe { get; set;}

        // The names of buddies who has this item
        List<string> Buddies { get; set; }
        ulong Count { get; set; }
        string Name { get; }
        string BaseRecord { get; }

        long PrefixRarity { get; }


        string Rarity { get; }

        float MinimumLevel { get; }
        string Slot { get; }
        bool IsKnown { get; }

        IList<TranslatedStat> HeaderStats { get; }
        IList<TranslatedStat> BodyStats { get; }
        IList<TranslatedStat> PetStats { get; }

        List<SkillModifierStat> ModifiedSkills { get; }

        string Bitmap { get; }

        ISet<DBStatRow> Tags {
            get;
            set;
        }

        PlayerItemSkill Skill { get; }
    }
}
