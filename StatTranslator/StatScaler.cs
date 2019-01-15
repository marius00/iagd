using DataAccess;
using System.Collections.Generic;

namespace StatTranslator
{
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
                stat.Value = new Randomizer(seed).GenerateInt((int)stat.Value - 20, (int)stat.Value + 20);
            }
        }
    }
}
