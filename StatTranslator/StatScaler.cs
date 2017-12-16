using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTranslator {
    public static class StatScaler {
        public static void Scale(IItemStat stat, int seed) {
            var NoScale = new List<string>() {
                "characterStrength",
                "characterDexterity",
                "characterIntelligence",
                "defensiveLife",
                "characterMana",

                "characterStrengthModifier",
                "characterDexterityModifier",
                "characterIntelligenceModifier",
                "defensiveLifeModifier",
                "characterManaModifier",

                "characterOffensiveAbility",
                "characterDefensiveAbility",
                "characterOffensiveAbilityModifier",
                "characterDefensiveAbilityModifier",

                "characterLifeRegen",
                "characterManaRegen",
                "characterLifeRegenModifier",
                "characterManaRegenModifier",
            };



            if (NoScale.Contains(stat.Stat)) {
                stat.Value = new Randomizer(seed).IGenerate((int)stat.Value - 20, (int)stat.Value + 20);
            }

        }
    }
}
