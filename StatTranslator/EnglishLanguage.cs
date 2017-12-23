using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTranslator {

    public class EnglishLanguage : ILocalizedLanguage {
        public bool WarnIfMissing { get; } = false;

        public void SetTagIfMissing(string tag, string value) {
            if (!stats.ContainsKey(tag))
                stats[tag] = value;
        }

        private Dictionary<string, string> stats = new Dictionary<string, string> {
            {"tagItemNameOrder", "{%_s0}{%_s1}{%_s2}{%_s3}{%_s4}" },


            // Simply Header stats
            { "offensivePierceRatioMin", "{0}% Armor Piercing"},
            { "defensiveProtection", "{0} Armor"},
            { "skillChanceWeight", "{0}% Chance to be Used"},
            { "skillProjectileNumber", "{0} Projectile"},
            { "skillCooldownTime", "{0} Seconds Skill Recharge" },
            { "skillManaCost", "{0} Energy Cost" },
            { "skillTargetRadius", "{0} Meter Radius" },
            { "skillActiveDuration", "{0} Second Duration" }, // Skills only

            // Simply Body Stats
            { "weaponDamagePct", "+{0}% Weapon Damage" },

            { "offensivePercentCurrentLifeMin", "{0}% Reduction to Enemy\"s Health" },
            { "characterLife", "+{0} Health" },
            { "characterLifeModifier", "+{0}% Health" },
            { "augmentAllLevel", "+{0} to All Skills" },
            { "characterDefensiveAbility", "+{0} Defensive Ability" },
            { "characterOffensiveAbility", "+{0} Offensive Ability" },
            { "characterDefensiveAbilityModifier", "+{0}% Defensive Ability" },
            { "characterOffensiveAbilityModifier", "+{0}% Offensive Ability" },
            { "augmentMastery1", "+{0} to All Skills in {3}" },
            { "augmentMastery2", "+{0} to All Skills in {3}" },
            { "augmentMastery3", "+{0} to All Skills in {3}" },
            { "augmentMastery4", "+{0} to All Skills in {3}" },
            { "augmentSkill1", "+{0} to {3}" },
            { "augmentSkill2", "+{0} to {3}" },
            { "augmentSkill3", "+{0} to {3}" },
            { "augmentSkill4", "+{0} to {3}" },
            { "augmentSkill1Extras", "Tier {0} {3} skill" },
            { "augmentSkill2Extras", "Tier {0} {3} skill" },
            { "augmentSkill3Extras", "Tier {0} {3} skill" },
            { "augmentSkill4Extras", "Tier {0} {3} skill" },
            { "defensivePetrify", "{0}% Reduced Petrify Duration" },
            { "offensiveCritDamageModifier", "+{0}% Crit Damage" },
            { "characterRunSpeedModifier", "+{0}% Movement Speed" },
            { "characterIntelligenceModifier", "+{0}% Spirit" },
            { "skillCooldownReduction", "-{0}% Skill Cooldown Reduction" },
            { "retaliationTotalDamageModifier", "+{0}% Total Retaliation Damage" }, // not caught by the .replace("Modifier") for some reason..
            { "characterAttackSpeedModifier", "+{0}% Attack Speed" },
            { "characterAttackSpeed", "+{0}% Attack Speed" },
            { "offensiveLifeLeechMin", "{0}% of Attack Damage converted to Health" },
            { "characterIntelligence", "+{0} Spirit" },
            { "characterManaRegen", "+{0} Energy Regenerated per second" },
            { "characterManaRegenModifier", "+{0}% Energy Regenerated per second" },
            { "characterLightRadius", "+{0}% Light Radius" },
            { "characterDodgePercent", "+{0}% Chance to Avoid Melee Attacks" },
            { "piercingProjectile", "{0}% Chance to pass through Enemies" },
            { "characterMana", "+{0} Energy" },
            { "characterManaModifier", "+{0}% Energy" },
            { "characterEnergyAbsorptionPercent", "{0}% Energy Absorption From Enemy Spells" },
            { "characterSpellCastSpeedModifier", "+{0}% Casting Speed" },
            { "defensiveReflect", "{0}% Damage Reflected"},
            { "blockRecoveryTime", "{0} second Block Recovery" },
            { "characterLifeRegen", "+{0} Health/s" },
            { "characterDexterity", "+{0} Cunning" },
            { "defensiveTrap", "{0}% Reduced Entrapment Duration" },
            { "characterLifeRegenModifier", "+{0}% Health/s" },
            { "characterDeflectProjectile", "+{0}% Chance to Avoid Projectiles" },
            { "characterConstitutionModifier", "+{0}% Constitution" },
            { "characterHuntingDexterityReqReduction", "-{0}% Cunning Req. for Ranged Weapons" },
            { "characterStrength", "+{0} Physique" },
            { "characterStrengthModifier", "+{0}% Physique" },
            { "characterTotalSpeedModifier", "+{0}% Total Speed" }, // Deprecated?
            { "skillProjectileSpeedModifier", "+{0}% Increase in Projectile Speed" }, // Deprecated?
            { "defensiveAbsorptionModifier", " Increases Armor Absorption by {0}%" },
            { "skillLifeBonus", "{0} Health Restored" },
            { "skillLifePercent", "{0}% Health Restored" },
            { "defensiveTotalSpeedResistance", "{0}% Slow Resistance" },
            { "defensiveBlockModifier", "Increases Shield Block Chance by {0}%" },
            { "offensiveTauntMin", "Taunt target for {0} Seconds" },
            { "defensiveBlockAmountModifier", "+{0}% Shield Damage Blocked" },
            { "characterDefensiveBlockRecoveryReduction", "+{0}% Shield Recovery Time" },
            { "skillTargetAngle", "{0} Targets in a {1}° Angle" },
            { "offensiveStunMin", "Stun target for {0} Seconds" },
            { "offensiveStunMax", "Stun target for {0}-{1} Seconds" },
            { "offensiveStunChance", "{3}% chance to " },
            { "defensiveProtectionModifier", "Increases Armor by {0}%" },
            { "projectilePiercingChance", "{0}% Chance to pass through Enemies" },
            { "projectileExplosionRadius", "{0} Meter Radius" },
            { "sparkChance", "{0}% Chance of affecting up to {1} targets within {2} Meters" },

            // Skill triggers
            
            {"cast_@allyonattack", " " },
            {"cast_@allyonlowhealth", " " },
            {"cast_@enemylocationonkill", " " },
            {"cast_@enemyonanyhit", " " },
            {"cast_@enemyonattack", "{0}% Chance on attack" },
            {"cast_@enemyonattackcrit", "{0}% Chance on a critical attack (target enemy)" },
            {"cast_@enemyonblock", "{0}% Chance when blocked" },
            {"cast_@enemyonhitcritical", "{0}% Chance when hit by a critical" },
            {"cast_@enemyonkill", "{0}% Chance on Enemy Death" },  //C
            {"cast_@enemyonmeleehit", "{0}% Chance when hit by a melee attack" },
            {"cast_@enemyonprojectilehit", "{0}% NPSkill Proc" },
            {"cast_@selfonanyhit", "{0}% Chance when hit" }, //C
            {"cast_@selfonattack", "{0}% Chance on attacking" },
            {"cast_@selfonattackcrit", "{0}% Chance on a critical attack" },
            {"cast_@selfonblock", "{0}% Chance when blocking" },
            {"cast_@selfonhitcritica", "{0}% Chance when Hit by a Critical" },//C
            {"cast_@selfonkill", "{0}% Chance on Enemy Death" }, //C
            {"cast_@selfonlowhealth", "{0}% Chance at 25% health" },
            {"cast_@selfonmeleehit", "{0}% Chance when Hit by Melee Attacks" },
            {"cast_@selfonprojectilehit", "{0}% Chance when Hit by Ranged Attacks" },
            {"cast_@selfat", "{0}% Chance at {1}% Health" },

            // Retaliation
            { "customtag_013_retaliation", "{0}-{1} {3} Retaliation"},
            { "customtag_03_retaliation", "{0} {3} Retaliation"},
            { "customtag_retaliation_delay", " over {4} Seconds" },

            // Damage
            { "customtag_damage_chanceof", "{0}% Chance of "},
            { "customtag_damage_123", "{1}-{2} {3} Damage"},
            { "customtag_damage_13", "{1} {3} Damage"},
            { "customtag_damage_13%", "+{1}% {3} Damage"},
            { "customtag_damage_delay", " over {4} Seconds" },
            { "customtag_damage_racial", "+{0}% Damage to {3}" },
            { "customtag_damage_racial02", "+{0}% Damage to {3} & {5}" },
            { "customtag_damage_conversion", "{0}% {3} Damage converted to {5}" },

            // Xpac
            { "customtag_xpac_modif_weaponDamagePct", "{0}% Weapon Damage to {3}" },
            { "customtag_xpac_modif_petLimit", "+{0} to Pet Limit to {3}" },
            { "customtag_xpac_modif_physicalResist", "{0} Reduced target's Physical Resistance to {3}" },
            { "customtag_xpac_modif_physicalResistDuration", "{0} Reduced target's Physical Resistance for {1} Seconds to {3}" },
            { "customtag_xpac_modif_dmgConversionPerc", "100% {5} Damage converted to {6} Damage to {3}" },
            { "customtag_xpac_modif_dmgConversion", "{5} Damage converted to {6} Damage to {3}" },
            { "customtag_xpac_modif_speedModifier", "-{0}% Total Speed to {3}" },
            { "customtag_xpac_modif_defensiveAbilityDebuff", "{0} Defensive Ability to {3}" },
            { "customtag_xpac_modif_defensiveAbilityBuff", "+{0}% Defensive Ability to {3}" },
            { "customtag_xpac_modif_offensiveAbilityBuff", "+{0}% Offensive Ability to {3}" },
            { "customtag_xpac_modif_offensiveTaunt", "Generate Additional Threat to {3}" },
            { "customtag_xpac_modif_addProjectileX", "{0} Projectiles to {3}" },
            { "customtag_xpac_modif_addProjectile1", "1 Projectile to {3}" },
            { "customtag_xpac_modif_offensiveDamageMinMax", "{0}-{1} {5} Damage to {3}" },
            { "customtag_xpac_modif_offensiveDamageMin", "{0} {5} Damage to {3}" },
            { "customtag_xpac_modif_skillManaCostReduction", "-{0}% Skill Energy Cost to {3}" },
            { "customtag_xpac_modif_skillTargetRadius", "{0} Meter Target Area to {3}" },
            { "customtag_xpac_modif_sparkChance", "{0}% Chance of affecting up to {1} targets to {3}" },
            { "customtag_xpac_modif_skillCooldownTime", "{0} Second Skill Recharge to {3}" },
            { "customtag_xpac_modif_offensiveCritDamageModifier", "+{0}% Crit Damage to {3}" },
            { "customtag_xpac_modif_skillActiveDuration", "{0} Second Duration to {3}" },
            { "customtag_xpac_modif_defense", "{0}% {5} Resistance to {3}" },
            { "customtag_xpac_modif_defensiveAbilityDebuffForDuration", "{1} Reduced target's Defensive Ability for {0} Seconds to {3}" },
            { "customtag_xpac_modif_skillLifePercent", "{0}% Health Restored to {3}" },
            { "customtag_xpac_modif_skillTargetAngle", "{0} Degree Attack Arc to {3}" },
            { "customtag_xpac_modif_skillTargetNumber", "{0} Target Maximum to {3}" },
            { "customtag_xpac_modif_skillCooldownReductionChance", "{0}% Chance of +{1}% Skill Cooldown Reduction to {3}" },
            { "customtag_xpac_modif_offensiveTotalDamageReductionPercentDurationMin", "{0}% Reduced target's Damage for {1} Seconds to {3}" },
            { "customtag_xpac_modif_offensiveDamageMultModifier", "Total Damage Modified by {0}% to {3}" },
            { "customtag_xpac_modif_retaliationTotalDamageModifier", "+{0}% to All Retaliation Damage to {3}" },
            { "offensiveXDurationModifier", "+{1}% {5} Damage with +{0}% Increased Duration to {3}" },


            { "racialBonusPercentDefense", "+{0}% Less Damage From {3}" },
            { "racialBonusPercentDefense02", "+{0}% Less Damage From {3} & {5}" },

            { "customtag_faction_boost", "+{0}% faction gain with {3}" },
            { "User7", "Black Legion" },
            { "User2", "Homestead" },
            { "User4", "Outcast" },
            { "User8", "Kymon's Chosen" },
            { "Survivors", "Devil's Crossing" },
            { "User0", "The Rovers" },
            { "User5", "Order of Death's Vigil" },
            { "Aetherials", "Aetherials" },
            { "Cthonians", "Cthonians" },
            { "Outlaws", "Outlaws" },
            { "User6", "Undead" },

            { "customtag_block_012", "{0}% Chance to Block {1} Damage ({2}% Absorption)" },
            { "customtag_block_01", "{0}% Chance to Block {1} Damage" },
            { "customtag_speed", "Speed: {3} ({0})" },

            { "customtag_resistance_reduction", "{0}% Chance of {1}% Reduced target's Resistance For {2} Seconds" },

            // Races
            { "Race001", "Undead" },
            { "Race002", "Beastkin" },
            { "Race003", "Aetherials" },
            { "Race004", "Chthonic" },
            { "Race005", "Aether Corruption" },
            { "Race009", "Human" },
            { "Race012", "Beastkin" },

            {"class00", "abcccc" },
            {"class01", "Soldier" },
            {"class02", "Demolitionist" },
            {"class03", "Occultist" },
            {"class04", "Nightblade" },
            {"class05", "Arcanist" },
            {"class06", "Shaman" },
            {"class07", "Inquisitor" },
            {"class08", "Necromancer" },

            // Attack speeds
            {"tagAttackSpeedVeryFast", "Very Fast"},
            {"tagAttackSpeedFast", "Fast"},
            {"tagAttackSpeedAverage", "Average"},
            {"tagAttackSpeedSlow", "Slow"},
            {"tagAttackSpeedVerySlow", "Very Slow"},

            // Damage types            
            { "SlowPhysical", "Internal Trauma"},
            { "SlowFire", "Burn"},
            { "SlowCold", "Frost"},
            { "SlowLightning", "Electrocute"},
            { "SlowVitality", "Vitality Decay"},
            { "SlowPoison", "Poison"},
            { "SlowLife", "Vitality Decay" },
            { "SlowBleeding", "Bleeding" },
            { "Poison", "Acid" },
            { "BasePoison", "Acid" },
            { "BonusPhysical", "Bonus" },
            { "Life", "Vitality" },
            { "TotalDamage", "to All" },
            { "iatag_ui_physical", "Physical" },
            { "PercentCurrentLife", "Life Reduction" },

            { "Physical", "Physical"},
            { "Fire", "Fire"},
            { "Cold", "Cold"},
            { "Vitality", "Vitality"},
            { "Lightning", "Lightning"},
            { "Chaos", "Chaos"},
            { "Bleeding", "Bleeding"},
            { "Elemental", "Elemental"},
            { "Pierce", "Pierce"},
            { "Aether", "Aether"},

            { "BasePhysical", "Physical"},
            { "BaseFire", "Fire"},
            { "BaseCold", "Cold"},
            { "BaseVitality", "Vitality"},
            { "BaseLightning", "Lightning"},
            { "BaseChaos", "Chaos"},
            { "BaseBleeding", "Bleeding"},
            { "BaseElemental", "Elemental"},
            { "BasePierce", "Pierce"},
            { "BaseAether", "Aether"},
            { "BaseLife", "Vitality"},
            { "Resistance", "Resistance" },

            // HTML
            { "iatag_html_choose_a_relic", "Choose a Relic" },
            { "iatag_html_choose_a_recipe", "Choose a Recipe" },
            { "iatag_html_choose_a_component", "Choose a Component" },
            { "iatag_html_badstate_close", "Close" },
            { "iatag_html_transferall", "Transfer all" },
            { "iatag_html_transfer", "Transfer to Stash" },
            { "iatag_html_items_youcancraftthisitem", "You can craft this item." },
            { "iatag_html_items_buddy_alsohasthisitem1", "also has this item" },
            { "iatag_html_items_buddy_alsohasthisitem2", "of your Buddies also have this item" },
            { "iatag_html_items_buddy_alsohasthisitem3", "of your Buddies have this item" },
            { "iatag_html_items_buddy_alsohasthisitem4", "has this item" },
            { "iatag_html_levlerequirement", "Level Requirement:" },
            { "iatag_html_any", "Any" },
            { "iatag_html_items_affix2", "This item has two green affixes" },
            { "iatag_html_items_affix3", "This item has three green affixes! (Rare!)" },
            { "iatag_html_items_unknown", "Unknown Item" },
            { "iatag_html_bonustopets", "Bonus to All Pets" },
            { "iatag_html_copytoclipboard", "Copy to Clipboard" },
            { "iatag_html_tab_header_items", "Items" },
            { "iatag_html_tab_header_crafting", "Crafting" },
            { "iatag_html_tab_header_components", "Components" },
            { "iatag_html_tab_header_discord", "Discord" },
            { "iatag_html_crafting_lacking", "You are currently lacking:" },
            { "iatag_html_items_no_items", "No items found!" },
            { "iatag_html_items_grantsskill", "Grants Skill:" },
            { "iatag_html_items_level", "Level" },
            { "iatag_html_badstate_title", "Ooops!" },
            { "iatag_html_badstate_subtitle", "It seems IA has gotten in a bad state!" },
            

            // UI, IA Only
            { "iatag_ui_buddy_userid", "User ID: " },
            { "iatag_ui_buddy_userid_none", "None" },
            { "iatag_ui_buddy_userid_name", "Name: " },
            { "iatag_ui_buddy_userid_numeric_error_message", "Numerics only, ask your buddy for his Id!" },
            { "iatag_ui_online_backups_disabled", "Online backups are disabled" },
            { "iatag_ui_online_backup_email", "You are logged in as {OnlineBackupEmail}" },
            { "iatag_ui_x_items_backed_up", "{numSynchronized}/{numItems} items backed up, {numUnsynchronizedItems} remaining.." },
            { "iatag_ui_all_items_backed_up", "All of your items have been backed up." },
            { "iatag_ui_hc", " (HC)" },
            { "iatag_ui_vanilla", "Vanilla" },
            { "iatag_ui_vanilla_xpac", "Vanilla & Expansion" },
            { "iatag_ui_yes", "Yes" },
            { "iatag_ui_no", "No" },
            { "iatag_ui_survivalmode", "The Crucible (DLC)" },
            { "iatag_no_stash_abort", "No stash chosen, aborting transfer.." },
            { "iatag_stash_status_error", "Could not inject into Grim Dawn, are you sure the bank is CLOSED?" },
            {"iatag_stash3_failure", "Cannot deposit item, please ensure Stash 3 is empty." },
            {"iatag_stash3_success", "Successfully deposited {0} out of {1} items" },
            {"iatag_deposit_stash_open", "Cannot deposit item, please close your stash in-game" },
            {"iatag_deposit_stash_full", "Could not insert item, stash full" },
            {"iatag_deposit_stash_sorted", "Cannot deposit item, please open or close your stash in-game (stash was sorted)" },
            {"iatag_deposit_stash_unknown_tooltip", "Stash status is unknown, open & close stash in-game to resolve." },
            {"iatag_deposit_stash_unknown_feedback", "Cannot deposit item, please open and close your stash in-game" },
            {"iatag_stash_hotfix_1_0_40_0_rejected", "Transfer rejected due to v1.0.40.0 hotfix - Either use instatransfer or close GrimDawn first" },
            {"iatag_deposit_pipe_success", "Item sent to GD" },
            {"iatag_file_does_not_exist", "The specified file does not exist" },
            {"iatag_file_not_zip", "Specified file is not a zip file!?" },
            {"iatag_pg_restore_error", "Error restoring from backup, see log file for more details" },
            {"iatag_stash_open", "Stash: Open" },
            {"iatag_stash_crafting", "Stash: Crafting" },
            {"iatag_stash_closed", "Stash: Closed" },
            {"iatag_stash_error", "Stash: ERROR" },
            {"iatag_stash_unknown", "Stash: Unknown" },
            {"iatag_stash_sorted", "Stash: Open/Restricted" },
            {"iatag_stash_", "Stash: " },
            {"iatag_copied_clipboard", "Items copied to clipboard" },
            {"iatag_stash_not_found", "Could not locate and stash files.." },
            {"iatag_legacy_backup", "Invalid backup file. If this is a really old backup, please see the instructions on restoring from a legacy backup." },
            {"iatag_postgres_backup", "Invalid backup file. This backup format has been deprecated. Contact me on itemassistant@gmail.com and I'll send you a manual restore." },

            {"iatag_not_imlpemented", "Functionality not implemented" },


            {"iatag_rarity_any", "Any" },
            {"iatag_rarity_yellow", "Yellow" },
            {"iatag_rarity_green", "Green" },
            {"iatag_rarity_blue", "Blue" },
            {"iatag_rarity_epic", "Epic" },

            {"iatag_feedback_too_close_to_stash", "Delaying stash loot - Standing too close to stash!" },
            {"iatag_feedback_delaying_stash_loot_status", "Waiting for stash to close.." },
            {"iatag_feedback_no_items_to_loot", "No items to loot in stash"},
            {"iatag_feedback_unable_to_loot_stash4", "Unable to loot stash page.."},
            {"iatag_feedback_item_does_not_exist", "Cannot deposit item, item does not appear to exist.. (ghost item)"},
            {"iatag_feedback_cloud_save_enabled_ingame", "WARNING - Grim Dawn Cloud saving is active." },

            {"iatag_slot_any", "Any" },
            {"iatag_slot_head", "Head" },
            {"iatag_slot_hands", "Hands" },
            {"iatag_slot_feet", "Feet" },
            {"iatag_slot_legs", "Legs" },
            {"iatag_slot_chest", "Chest" },
            {"iatag_slot_belt", "Belt" },
            {"iatag_slot_medal", "Medal" },
            {"iatag_slot_ring", "Ring" },
            {"iatag_slot_shoulder", "Shoulder" },
            {"iatag_slot_neck", "Amulet/Neck" },
            {"iatag_slot_weapon1h", "Weapon (1h)" },
            {"iatag_slot_weapon2h", "Weapon (2h)" },
            {"iatag_slot_weaponranged", "Weapon (Ranged)" },
            {"iatag_slot_offhand", "Offhand" },
            {"iatag_slot_shield", "Shield" },
            {"iatag_slot_component", "Component" },
            {"iatag_slot_relic", "Relic" },
            {"iatag_slot_other", "Other" },
        };

        private const string english = "{%_s0}{%_s1}{%_s2}{%_s3}{%_s4}";
        ItemNameCombinator ItemCombinator = new ItemNameCombinator(english);

        public string TranslateName(string prefix, string quality, string style, string name, string suffix) {
            return ItemCombinator.TranslateName(prefix, quality, style, name, suffix);
        }

        public string[] Serialize() {
            return stats.Keys.ToArray();
        }


        public string GetTag(string tag) {
            if (stats.ContainsKey(tag))
                return stats[tag];
            else
                return string.Empty;

        }
    }
}
