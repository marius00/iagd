﻿using DataAccess;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatTranslator {
    public class StatManager {
        private static readonly string[] BodyDamageTypes = {
            "SlowPoison",
            "SlowPhysical",
            "SlowBleeding",
            "Bleeding",
            "SlowLife",
            "SlowFire",
            "SlowCold",
            "SlowLightning",
            "Poison", 
            "Chaos",
            "Fire",
            "Aether",
            "Bleeding",
            "Cold",
            "Lightning",
            "Elemental",
            "Pierce",
            "Physical",
            "Life",
            "TotalDamage",
            "PercentCurrentLife"
        };

        private static readonly ILog Logger = LogManager.GetLogger(typeof(StatManager));
        private readonly ILocalizedLanguage _language;

        public StatManager(ILocalizedLanguage language) {
            _language = language;
        }

        private void ProcessConversionDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat conversionPercentage = stats.FirstOrDefault(m => m.Stat == "conversionPercentage");
            IItemStat conversionOutType = stats.FirstOrDefault(m => m.Stat == "conversionOutType");
            IItemStat conversionInType = stats.FirstOrDefault(m => m.Stat == "conversionInType");

            if (conversionPercentage != null && conversionOutType != null && conversionInType != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_damage_conversion"),
                    Param0 = conversionPercentage.Value,
                    Param3 = DamageTypeTranslation(conversionInType.TextValue),
                    Param5 = DamageTypeTranslation(conversionOutType.TextValue),
                    Type = TranslatedStatType.BODY
                });
            }
            
            // A bit of a lame way of doing this.. maybe look into a better way? stats has a "pet" prefix from earlier preprocessing when loading the database
            IItemStat petConversionPercentage = stats.FirstOrDefault(m => m.Stat == "petconversionPercentage");
            IItemStat petConversionOutType = stats.FirstOrDefault(m => m.Stat == "petconversionOutType");
            IItemStat petConversionInType = stats.FirstOrDefault(m => m.Stat == "petconversionInType");

            if (petConversionPercentage != null && petConversionOutType != null && petConversionInType != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_damage_conversion"),
                    Param0 = petConversionPercentage.Value,
                    Param3 = DamageTypeTranslation(petConversionInType.TextValue),
                    Param5 = DamageTypeTranslation(petConversionOutType.TextValue),
                    Type = TranslatedStatType.BODY
                });
            }
        }

        /// <summary>
        /// Process skill stats
        /// These are non-standard skills, pre-processed by the parser.
        /// 
        /// augmentSkill contains both the skill name and the increment amount
        /// augmentSkillExtras contains any additional info like which class it belongs to, and tier.
        /// This is done to avoid cross-record/item lookups at runtime
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="result"></param>
        private void ProcessAddSkill(ISet<IItemStat> stats, List<TranslatedStat> result) {
            List<IItemStat> skillCandidates = stats.Where(m => m.Stat.StartsWith("augmentSkill") && m.Stat.Length == "augmentSkill".Length + 1)
                .ToList();

            // "augmentSkill1", "augmentSkill2",
            foreach (IItemStat stat in skillCandidates) {
                string statName = stat.Stat;
                // Record is requires as we may have "augmentSkill1" multiple times, from different records.
                IItemStat statExtras = stats.FirstOrDefault(m => m.Stat == statName + "Extras" && m.Record == stat.Record);

                TranslatedStat extraStat = null;

                if (statExtras != null) {
                    extraStat = new TranslatedStat {
                        Text = _language.GetTag(statName + "Extras"),
                        Param0 = statExtras.Value, // Tier
                        Param3 = _language.GetTag(statExtras.TextValue) // Class
                    };
                }

                result.Add(new TranslatedStat {
                    Text = _language.GetTag(statName),
                    Param0 = stat.Value,
                    Param3 = stat.TextValue,
                    Extra = extraStat
                });
            }
        }
        private void ProcessAddMastery(ISet<IItemStat> stats, List<TranslatedStat> result) {
            // "augmentSkill1", "augmentSkill2",
            for (int i = 1; i <= 4; i++) {
                string statName = "augmentMastery" + i;
                IItemStat stat = stats.FirstOrDefault(m => m.Stat == statName);

                if (stat != null) {
                    result.Add(new TranslatedStat {
                        Text = _language.GetTag(statName),
                        Param0 = stat.Value,
                        Param3 = _language.GetTag(stat.TextValue)
                    });
                }
            }
        }

        private void ProcessStun(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat min = stats.FirstOrDefault(m => m.Stat == "offensiveStunMin");
            IItemStat max = stats.FirstOrDefault(m => m.Stat == "offensiveStunMax");
            IItemStat chance = stats.FirstOrDefault(m => m.Stat == "offensiveStunChance");

            if (min == null) {
                return;
            }

            string tag = _language.GetTag(max != null ? "offensiveStunMax" : "offensiveStunMin");

            if (chance != null) {
                tag = _language.GetTag("offensiveStunChance") + tag;
            }

            result.Add(new TranslatedStat {
                Text = tag,
                Param0 = min.Value,
                Param1 = max?.Value,
                Param3 = chance?.Value.ToString()
            });
        }

        private void ProcessAngleDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat angle = stats.FirstOrDefault(m => m.Stat == "skillTargetAngle");
            IItemStat numTargets = stats.FirstOrDefault(m => m.Stat == "skillTargetNumber");

            if (angle != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("skillTargetAngle"),
                    Param0 = numTargets.Value,
                    Param1 = angle.Value
                });
            }
        }

        private void ProcessChainDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat sparkChance = stats.FirstOrDefault(m => m.Stat == "sparkChance");
            IItemStat sparkGap = stats.FirstOrDefault(m => m.Stat == "sparkGap");
            IItemStat sparkMaxNumber = stats.FirstOrDefault(m => m.Stat == "sparkMaxNumber");

            if (sparkChance != null && sparkGap != null && sparkMaxNumber != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("sparkChance"),
                    Param0 = sparkChance.Value,
                    Param1 = sparkMaxNumber.Value,
                    Param2 = sparkGap.Value
                });
            }
        }

        private void ProcessFactionWrits(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat faction = stats.FirstOrDefault(m => m.Stat == "boostedFaction");
            IItemStat multiplier = stats.FirstOrDefault(m => m.Stat == "boostedMultiplier");

            if (faction != null && multiplier != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_faction_boost"),
                    Param0 = (multiplier.Value - 1) * 100,
                    Param3 = _language.GetTag(faction.TextValue),
                    Type = TranslatedStatType.BODY
                });
            }
        }

        private void ProcessReducedResistances(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat chance = stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionPercentChance");
            IItemStat duration = stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionPercentDurationMin");
            IItemStat min = stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionPercentMin");

            if (chance != null && duration != null && min != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_resistance_reduction"),
                    Param0 = chance.Value,
                    Param1 = min.Value,
                    Param2 = duration.Value,
                    Type = TranslatedStatType.BODY
                });
            }
        }

        private void ProcessRacialBonuses(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat racialBonusPercentDamage = stats.FirstOrDefault(m => m.Stat == "racialBonusPercentDamage");
            List<IItemStat> racialBonusRace = stats.Where(m => m.Stat == "racialBonusRace").ToList();

            if (racialBonusPercentDamage != null && racialBonusRace.Count >= 1) {
                string race01 = _language.GetTag(racialBonusRace.FirstOrDefault().TextValue);
                string race02 = racialBonusRace.Count >= 2 ? _language.GetTag(racialBonusRace.FirstOrDefault().TextValue) : null;

                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_damage_racial"),
                    Param0 = racialBonusPercentDamage.Value,
                    Param3 = race01,
                    Param5 = race02,
                    Type = TranslatedStatType.BODY
                });
            }

            IItemStat racialBonusPercentDefense = stats.FirstOrDefault(m => m.Stat == "racialBonusPercentDefense");

            if (racialBonusPercentDefense == null || racialBonusRace.Count < 1) {
                return;
            }

            string raceDef01 = _language.GetTag(racialBonusRace[0].TextValue);
            string raceDef02 = racialBonusRace.Count >= 2 ? _language.GetTag(racialBonusRace.FirstOrDefault().TextValue) : null;

            if (raceDef02 == null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("racialBonusPercentDefense"),
                    Param0 = racialBonusPercentDefense.Value,
                    Param3 = raceDef01,
                    Type = TranslatedStatType.BODY
                });
            }
            else {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("racialBonusPercentDefense02"),
                    Param0 = racialBonusPercentDefense.Value,
                    Param3 = raceDef01,
                    Param5 = raceDef02,
                    Type = TranslatedStatType.BODY
                });
            }
        }

        private void ProcessAttackSpeed(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat characterBaseAttackSpeed = stats.FirstOrDefault(m => m.Stat == "characterBaseAttackSpeed");

            if (characterBaseAttackSpeed == null) {
                return;
            }

            string tag = stats.FirstOrDefault(m => m.Stat == "characterBaseAttackSpeedTag")?.TextValue;
            tag = tag != null ? _language.GetTag(tag) : "Unknown";

            result.Add(new TranslatedStat {
                Text = _language.GetTag("customtag_speed"),
                Param0 = characterBaseAttackSpeed.Value,
                Param3 = tag
            });
        }

        // TODO: Surely there can be some generic way to do this? Input 2 stats and a translation string?
        private void ProcessSlowRetaliation(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat duration = stats.FirstOrDefault(m => m.Stat == "retaliationSlowAttackSpeedDurationMin");
            IItemStat amount = stats.FirstOrDefault(m => m.Stat == "retaliationSlowAttackSpeedMin");

            if (duration != null && amount != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_slow_retaliation"),
                    Param0 = amount.Value,
                    Param1 = duration.Value
                });
            }
        }

        private void ProcessShieldBlock(ISet<IItemStat> stats, List<TranslatedStat> result) {
            IItemStat defensiveBlockChance = stats.FirstOrDefault(m => m.Stat == "defensiveBlockChance");

            if (defensiveBlockChance == null) {
                return;
            }

            IItemStat defensiveBlock = stats.FirstOrDefault(m => m.Stat == "defensiveBlock");
            IItemStat blockAbsorption = stats.FirstOrDefault(m => m.Stat == "blockAbsorption");

            if (defensiveBlock != null && blockAbsorption != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_block_012"),
                    Param0 = defensiveBlockChance.Value,
                    Param1 = defensiveBlock.Value,
                    Param2 = blockAbsorption.Value,
                    Type = TranslatedStatType.HEADER
                });
            }
            else if (defensiveBlock != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_block_01"),
                    Param0 = defensiveBlockChance.Value,
                    Param1 = defensiveBlock.Value,
                    Type = TranslatedStatType.HEADER
                });
            }
        }

        private string DamageTypeTranslation(string d) {
            d = d.Replace("Modifier", "");

            string localized = _language.GetTag(d);

            if (!string.IsNullOrEmpty(localized)) {
                return localized;
            }

            Logger.Warn($"Missing translations for tag \"{d}\"");

            return d.Replace("Base", "");
        }

        private void ProcessHeaderDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            string[] _damageTypes = {
                "BasePoison",
                "BaseChaos",
                "BaseFire",
                "BaseAether",
                "BaseCold",
                "BaseLightning",
                "BasePierce",
                "BasePhysical",
                "BaseLife"
            };
            List<string> damageTypes = _damageTypes.Select(m => $"offensive{m}Min").ToList();
            _ProcessDamage(stats, result, damageTypes, TranslatedStatType.HEADER);
        }

        private void ProcessBodyDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            List<string> damageTypes = BodyDamageTypes.Select(m => $"offensive{m}Min").ToList();
            _ProcessDamage(stats, result, damageTypes, TranslatedStatType.BODY);

            damageTypes = BodyDamageTypes.Select(m => $"offensive{m}Modifier").ToList();
            _ProcessDamage(stats, result, damageTypes, TranslatedStatType.BODY);

            //translationTable[string.Format("offensive{0}Modifier", damageTypes[i])] = "+{0}% " + DamageTypeTranslation(damageTypes[i]) + " Damage";
        }

        private void ProccessBodyRetaliation(ISet<IItemStat> stats, List<TranslatedStat> result) {
            List<string> damageTypes = BodyDamageTypes.Select(m => $"retaliation{m}Min").ToList();

            IEnumerable<IItemStat> candidates = stats.Where(m => damageTypes.Contains(m.Stat));

            foreach (IItemStat minDmg in candidates) {
                string type = minDmg.Stat.Replace("retaliation", "").Replace("Min", "");
                IItemStat maxDmg = stats.FirstOrDefault(m => m.Stat.Equals($"retaliation{type}Max"));
                IItemStat duration = stats.FirstOrDefault(m => m.Stat.Equals($"retaliation{type}DurationMin"));

                float minDmgVal = minDmg.Value;

                StringBuilder sb = new StringBuilder();

                if (maxDmg != null) {
                    sb.Append(_language.GetTag("customtag_013_retaliation"));
                }
                else {
                    sb.Append(_language.GetTag("customtag_03_retaliation"));
                }

                if (duration != null) {
                    sb.Append(_language.GetTag("customtag_retaliation_delay"));
                    minDmgVal *= duration.Value;
                }

                result.Add(new TranslatedStat {
                    Text = sb.ToString(),
                    Param0 = minDmgVal,
                    Param1 = maxDmg?.Value,
                    Param3 = DamageTypeTranslation(type),
                    Param4 = duration?.Value,
                    Type = TranslatedStatType.BODY
                });
            }
        }

        private void _ProcessDamage(
            ISet<IItemStat> stats,
            List<TranslatedStat> result,
            List<string> damageTypes,
            TranslatedStatType location) {
            IEnumerable<IItemStat> candidates = stats.Where(m => damageTypes.Contains(m.Stat));

            foreach (IItemStat minDmg in candidates) {
                string type = minDmg.Stat.Replace("offensive", "").Replace("Min", "");
                IItemStat maxDmg = stats.FirstOrDefault(m => m.Stat.Equals($"offensive{type}Max"));
                IItemStat chance = stats.FirstOrDefault(m => m.Stat.Equals($"offensive{type}Chance"));
                IItemStat duration = stats.FirstOrDefault(m => m.Stat.Equals($"offensive{type}DurationMin"));

                float minDmgVal = minDmg.Value;

                StringBuilder sb = new StringBuilder();

                if (chance != null) {
                    sb.Append(_language.GetTag("customtag_damage_chanceof"));
                }

                if (maxDmg != null) {
                    sb.Append(_language.GetTag("customtag_damage_123"));
                }
                else {
                    sb.Append(type.Contains("Modifier")
                        ? _language.GetTag("customtag_damage_13%")
                        : _language.GetTag("customtag_damage_13"));
                }

                if (duration != null) {
                    sb.Append(_language.GetTag("customtag_damage_delay"));
                    minDmgVal *= duration.Value;
                }

                TranslatedStat sm = new TranslatedStat {
                    Text = sb.ToString(),
                    Param0 = chance?.Value,
                    Param1 = minDmgVal,
                    Param2 = maxDmg?.Value,
                    Param3 = DamageTypeTranslation(type),
                    Param4 = duration?.Value,
                    Type = location
                };

                result.Add(sm);
            }
        }

        private void MapSimpleHeaderEntries(ISet<IItemStat> stats, List<TranslatedStat> result) {
            string[] tags = {
                "offensivePierceRatioMin",
                "defensiveProtection",
                "skillChanceWeight",
                "skillProjectileNumber",
                "skillCooldownTime",
                "skillManaCost",
                "skillTargetRadius",
                "skillActiveDuration"
            };

            Dictionary<string, string> headerTranslationTable = new Dictionary<string, string>();

            foreach (string tag in tags) {
                headerTranslationTable[tag] = _language.GetTag(tag);
            }

            foreach (IItemStat elem in stats.Where(m => headerTranslationTable.Keys.Contains(m.Stat))) {
                result.Add(new TranslatedStat {
                    Text = headerTranslationTable[elem.Stat],
                    Param0 = elem.Value
                });
            }
        }

        public TranslatedStat TranslateSkillAutoController(string record) {
            try {
                // The tag names are 'cast_@allyonattack' etc, without the preceding record path, and without the parameterized chance%

                string tagName = record.Replace("records/controllers/itemskills/", "");
                tagName = tagName.Substring(0, tagName.IndexOf('_', 5));

                string localized = _language.GetTag(tagName);

                if (!string.IsNullOrEmpty(localized)) {
                    string result = System.Text.RegularExpressions.Regex.Match(record, @"\d+").Value;

                    return new TranslatedStat {
                        Param0 = float.Parse(result),
                        Text = localized
                    };
                }

                if (record.StartsWith("records/controllers/itemskills/cast_@selfat")) {
                    string result = System.Text.RegularExpressions.Regex.Match(record, @"\d+").Value;

                    return new TranslatedStat {
                        Param0 = 100, // this is bound to be wrong some day
                        Param1 = float.Parse(result),
                        Text = _language.GetTag("cast_@selfat")
                    };
                }
            }
            catch (FormatException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }
            catch (ArgumentOutOfRangeException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }

            return new TranslatedStat();
        }

        private void MapSimpleBodyEntries(ISet<IItemStat> stats, List<TranslatedStat> result) {
            string[] tags = {
                "defensiveAllMaxResist",
                "weaponDamagePct",
                "offensivePercentCurrentLifeMin",
                "characterLife",
                "characterLifeModifier",
                "augmentAllLevel",
                "characterDefensiveAbility",
                "characterOffensiveAbility",
                "characterDefensiveAbilityModifier",
                "characterOffensiveAbilityModifier",
                "defensiveBlockModifier",
                "defensivePetrify",
                "offensiveCritDamageModifier",
                "characterRunSpeedModifier",
                "characterIncreasedExperience",
                "characterIntelligenceModifier",
                "skillCooldownReduction",
                "retaliationTotalDamageModifier",
                "characterAttackSpeedModifier",
                "defensiveFreeze",
                "characterAttackSpeed",
                "offensiveLifeLeechMin",
                "characterIntelligence",
                "characterManaRegen",
                "characterManaRegenModifier",
                "characterLightRadius",
                "characterDodgePercent",
                "piercingProjectile",
                "characterMana",
                "characterManaModifier",
                "characterEnergyAbsorptionPercent",
                "characterSpellCastSpeedModifier",
                "defensiveReflect",
                "blockRecoveryTime", "characterLifeRegen",
                "characterDexterity",
                "characterDexterityModifier",
                "defensiveTrap",
                "characterLifeRegenModifier",
                "characterDeflectProjectile",
                "characterConstitutionModifier",
                "characterHuntingDexterityReqReduction",
                "characterStrength",
                "characterStrengthModifier",
                "characterTotalSpeedModifier",
                "skillProjectileSpeedModifier",
                "defensiveAbsorptionModifier",
                "skillLifeBonus",
                "skillLifePercent",
                "defensiveTotalSpeedResistance",
                "offensiveTauntMin",
                "defensiveBlockAmountModifier",
                "characterDefensiveBlockRecoveryReduction",
                "defensiveProtectionModifier",
                "projectilePiercingChance",
                "skillProjectileNumber",
                "projectileExplosionRadius",
                "defensiveStun",
                "offensiveTotalDamageModifier",
            };

            Dictionary<string, string> translationTable = new Dictionary<string, string>();

            foreach (string tag in tags) {
                translationTable[tag] = _language.GetTag(tag);
            }

            string[] damageTypes = BodyDamageTypes;
            string resistance = _language.GetTag("Resistance");
            string toMaxResistance = _language.GetTag("ResistanceMaxResist");

            foreach (string damageType in damageTypes) {
                string r = DamageTypeTranslation(damageType);
                translationTable[$"defensive{damageType}"] = $"{{0}}% {r} {resistance}";
                translationTable[$"defensive{damageType}Resistance"] = $"{{0}}% {r} {resistance}";
                translationTable[$"defensive{damageType}MaxResist"] = $"{{0}}% {toMaxResistance}{r} {resistance}";
            }

            foreach (IItemStat elem in stats.Where(m => translationTable.Keys.Contains(m.Stat))) {
                result.Add(new TranslatedStat {
                    Text = translationTable[elem.Stat],
                    Param0 = (float)Math.Round(elem.Value, 1, MidpointRounding.AwayFromZero),
                    Param3 = elem.TextValue
                });
            }
        }

        public List<TranslatedStat> ProcessSkillModifierStats(ISet<IItemStat> stats, string skill, string classtag, float? tier) {
            List<TranslatedStat> result = new List<TranslatedStat>();
            IItemStat weaponDamageEffect = stats.FirstOrDefault(m => m.Stat == "weaponDamagePct");
            IItemStat offensivePhysicalResistanceReduction =
                stats.FirstOrDefault(m => m.Stat == "offensivePhysicalResistanceReductionAbsoluteMin");
            IItemStat petLimit = stats.FirstOrDefault(m => m.Stat == "petLimit");

            TranslatedStat tooltip = null;

            if (classtag != null && tier != null) {
                tooltip = new TranslatedStat {
                    Text = _language.GetTag("augmentSkill1Extras"),
                    Param0 = tier,
                    Param3 = _language.GetTag(classtag)
                };
            }

            if (weaponDamageEffect != null) {
                result.Add(new TranslatedStat {
                    Param0 = weaponDamageEffect.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_weaponDamagePct"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            if (offensivePhysicalResistanceReduction != null) {
                IItemStat offensivePhysicalResistanceReductionAbsoluteDurationMin =
                    stats.FirstOrDefault(m => m.Stat == "offensivePhysicalResistanceReductionAbsoluteDurationMin");

                if (offensivePhysicalResistanceReductionAbsoluteDurationMin != null) {
                    result.Add(new TranslatedStat {
                        Param0 = offensivePhysicalResistanceReduction.Value,
                        Param1 = offensivePhysicalResistanceReductionAbsoluteDurationMin.Value,
                        Param3 = skill,
                        Extra = tooltip,
                        Text = _language.GetTag("customtag_xpac_modif_physicalResistDuration"),
                        Type = TranslatedStatType.FOOTER
                    });
                }
                else {
                    result.Add(new TranslatedStat {
                        Param0 = offensivePhysicalResistanceReduction.Value,
                        Param3 = skill,
                        Extra = tooltip,
                        Text = _language.GetTag("customtag_xpac_modif_physicalResist"),
                        Type = TranslatedStatType.FOOTER
                    });
                }
            }

            if (petLimit != null) {
                result.Add(new TranslatedStat {
                    Param0 = petLimit.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_petLimit"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            IItemStat conversionIn1 = stats.FirstOrDefault(m => m.Stat == "conversionInType");
            IItemStat conversionOutType1 = stats.FirstOrDefault(m => m.Stat == "conversionOutType");
            IItemStat conversionPercentage1 = stats.FirstOrDefault(m => m.Stat == "conversionPercentage");
            AddConversionStat(result, skill, conversionIn1, conversionOutType1, conversionPercentage1);

            IItemStat conversionIn2 = stats.FirstOrDefault(m => m.Stat == "conversionInType2");
            IItemStat conversionOutType2 = stats.FirstOrDefault(m => m.Stat == "conversionOutType2");
            IItemStat conversionPercentage2 = stats.FirstOrDefault(m => m.Stat == "conversionPercentage2");
            AddConversionStat(result, skill, conversionIn2, conversionOutType2, conversionPercentage2);

            // 12-85 Lightning Damage to Callidor's Tempest
            // "offensiveLightningMax"
            // "offensiveLightningMin"
            foreach (string damageType in BodyDamageTypes) {
                IItemStat min = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}Min");
                IItemStat max = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}Max");

                if (min != null) {
                    if (max != null) {
                        result.Add(new TranslatedStat {
                            Param0 = min.Value,
                            Param1 = max.Value,
                            Param3 = skill,
                            Param5 = DamageTypeTranslation(damageType),
                            Text = _language.GetTag("customtag_xpac_modif_offensiveDamageMinMax"),
                            Type = TranslatedStatType.FOOTER
                        });
                    }
                    else {
                        result.Add(new TranslatedStat {
                            Param0 = min.Value,
                            Param3 = skill,
                            Param5 = DamageTypeTranslation(damageType),
                            Text = _language.GetTag("customtag_xpac_modif_offensiveDamageMin"),
                            Type = TranslatedStatType.FOOTER
                        });
                    }

                    IItemStat offensiveXDurationModifier = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}DurationModifier");
                    IItemStat offensiveXModifier = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}Modifier");

                    if (offensiveXDurationModifier != null && offensiveXModifier != null) {
                        result.Add(new TranslatedStat {
                            Param0 = offensiveXDurationModifier.Value,
                            Param1 = offensiveXModifier.Value,
                            Param3 = skill,
                            Param5 = DamageTypeTranslation(damageType),
                            Text = _language.GetTag("offensiveXDurationModifier"),
                            Type = TranslatedStatType.FOOTER
                        });
                    }
                }

                IItemStat defensive = stats.FirstOrDefault(m => m.Stat == $"defensive{damageType}");

                if (defensive != null) {
                    result.Add(new TranslatedStat {
                        Param0 = defensive.Value,
                        Param3 = skill,
                        Param5 = DamageTypeTranslation(damageType),
                        Text = _language.GetTag("customtag_xpac_modif_defense"),
                        Type = TranslatedStatType.FOOTER
                    });
                }
            }

            AddSimpleStat("characterTotalSpeedModifier", "customtag_xpac_modif_speedModifier", stats, skill, result);
            AddSimpleStat("characterDefensiveAbility", "customtag_xpac_modif_defensiveAbilityDebuff", stats, skill, result);
            AddSimpleStat("characterDefensiveAbilityModifier", "customtag_xpac_modif_defensiveAbilityBuff", stats, skill, result);
            AddSimpleStat("offensiveTauntMin", "customtag_xpac_modif_offensiveTaunt", stats, skill, result);
            AddSimpleStat("characterOffensiveAbilityModifier", "customtag_xpac_modif_offensiveAbilityBuff", stats, skill, result);
            AddSimpleStat("retaliationTotalDamageModifier", "customtag_xpac_modif_retaliationTotalDamageModifier", stats, skill, result);
            AddSimpleStat("characterAttackSpeedModifier", "customtag_xpac_modif_characterAttackSpeedModifier", stats, skill, result);

            IItemStat projectileLaunchNumber = stats.FirstOrDefault(m => m.Stat == "projectileLaunchNumber");

            if (projectileLaunchNumber != null) {
                if (projectileLaunchNumber.Value > 1) {
                    result.Add(new TranslatedStat {
                        Param3 = skill,
                        Text = _language.GetTag("customtag_xpac_modif_addProjectile1"),
                        Type = TranslatedStatType.FOOTER
                    });
                }
                else {
                    result.Add(new TranslatedStat {
                        Param0 = projectileLaunchNumber.Value,
                        Param3 = skill,
                        Text = _language.GetTag("customtag_xpac_modif_addProjectileX"),
                        Type = TranslatedStatType.FOOTER
                    });
                }
            }

            AddSimpleStat("skillManaCostReduction", "customtag_xpac_modif_skillManaCostReduction", stats, skill, result);
            AddSimpleStat("skillTargetRadius", "customtag_xpac_modif_skillTargetRadius", stats, skill, result);

            IItemStat sparkChance = stats.FirstOrDefault(m => m.Stat == "sparkChance");
            IItemStat sparkMaxNumber = stats.FirstOrDefault(m => m.Stat == "sparkMaxNumber");

            if (sparkChance != null && sparkMaxNumber != null) {
                result.Add(new TranslatedStat {
                    Param0 = sparkChance.Value,
                    Param1 = sparkMaxNumber.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_sparkChance"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            AddSimpleStat("skillCooldownTime", "customtag_xpac_modif_skillCooldownTime", stats, skill, result);
            AddSimpleStat("offensiveCritDamageModifier", "customtag_xpac_modif_offensiveCritDamageModifier", stats, skill, result);
            AddSimpleStat("skillActiveDuration", "customtag_xpac_modif_skillActiveDuration", stats, skill, result);

            IItemStat offensiveSlowDefensiveAbilityDurationMin = stats.FirstOrDefault(m => m.Stat == "offensiveSlowDefensiveAbilityDurationMin");
            IItemStat offensiveSlowDefensiveAbilityMin = stats.FirstOrDefault(m => m.Stat == "offensiveSlowDefensiveAbilityMin");

            if (offensiveSlowDefensiveAbilityDurationMin != null && offensiveSlowDefensiveAbilityMin != null) {
                result.Add(new TranslatedStat {
                    Param0 = offensiveSlowDefensiveAbilityDurationMin.Value,
                    Param1 = offensiveSlowDefensiveAbilityMin.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_defensiveAbilityDebuffForDuration"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            AddSimpleStat("skillLifePercent", "customtag_xpac_modif_skillLifePercent", stats, skill, result);
            AddSimpleStat("skillTargetAngle", "customtag_xpac_modif_skillTargetAngle", stats, skill, result);
            AddSimpleStat("skillTargetNumber", "customtag_xpac_modif_skillTargetNumber", stats, skill, result);

            IItemStat skillCooldownReductionChance = stats.FirstOrDefault(m => m.Stat == "skillCooldownReductionChance");
            IItemStat skillCooldownReduction = stats.FirstOrDefault(m => m.Stat == "skillCooldownReduction");

            if (skillCooldownReduction != null && skillCooldownReductionChance != null) {
                result.Add(new TranslatedStat {
                    Param0 = skillCooldownReductionChance.Value,
                    Param1 = skillCooldownReduction.Value,
                    Param3 = skill, // TODO: Possible to get skill stuff on this? Tooltip + color
                    Text = _language.GetTag("customtag_xpac_modif_skillCooldownReductionChance"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            IItemStat offensiveTotalDamageReductionPercentMin = stats.FirstOrDefault(m => m.Stat == "offensiveTotalDamageReductionPercentMin");
            IItemStat offensiveTotalDamageReductionPercentDurationMin =
                stats.FirstOrDefault(m => m.Stat == "offensiveTotalDamageReductionPercentDurationMin");

            if (offensiveTotalDamageReductionPercentMin != null && offensiveTotalDamageReductionPercentDurationMin != null) {
                result.Add(new TranslatedStat {
                    Param0 = offensiveTotalDamageReductionPercentMin.Value,
                    Param1 = offensiveTotalDamageReductionPercentDurationMin.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_offensiveTotalResistanceReductionAbsoluteMin"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            IItemStat offensiveTotalResistanceReductionAbsoluteMin =
                stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionAbsoluteMin");
            IItemStat offensiveTotalResistanceReductionAbsoluteDurationMin =
                stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionAbsoluteDurationMin");

            if (offensiveTotalResistanceReductionAbsoluteMin != null && offensiveTotalResistanceReductionAbsoluteDurationMin != null) {
                result.Add(new TranslatedStat {
                    Param0 = offensiveTotalResistanceReductionAbsoluteMin.Value,
                    Param1 = offensiveTotalResistanceReductionAbsoluteDurationMin.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_offensiveTotalDamageReductionPercentDurationMin"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            // +x to summon <skill>
            IItemStat petBurstSpawn = stats.FirstOrDefault(m => m.Stat == "petBurstSpawn");

            if (petBurstSpawn != null) {
                result.Add(new TranslatedStat {
                    Param0 = petBurstSpawn.Value,
                    Param3 = skill,
                    Text = _language.GetTag("petBurstSpawn"),
                    Type = TranslatedStatType.BODY
                });
            }
            
            AddSimpleStat("offensiveDamageMultModifier", "customtag_xpac_modif_offensiveDamageMultModifier", stats, skill, result);

            /*
            if (stats.Count > 0 && result.Count == 0)
            {
                Logger.Warn("No stats parsed for modifiers");
                foreach (var stat in stats)
                {
                    Logger.Debug($"{stat.Stat}: {stat.Value}, {stat.TextValue}");
                }
            }*/

            // Apply the same tooltip to all
            foreach (TranslatedStat entry in result.Where(entry => entry.Extra == null)) {
                entry.Extra = tooltip;
            }

            return result;
        }

        private void AddSimpleStat(
            string statName,
            string translationTag,
            ISet<IItemStat> stats,
            string skill,
            List<TranslatedStat> result) {
            IItemStat stat = stats.FirstOrDefault(m => m.Stat == statName);

            if (stat != null) {
                result.Add(new TranslatedStat {
                    Param0 = stat.Value,
                    Param3 = skill,
                    Text = _language.GetTag(translationTag),
                    Type = TranslatedStatType.FOOTER
                });
            }
        }

        private void AddConversionStat(List<TranslatedStat> result, string skill, IItemStat input, IItemStat output, IItemStat percentage) {
            if (input == null || output == null) {
                return;
            }

            if (percentage != null) {
                result.Add(new TranslatedStat {
                    Param5 = DamageTypeTranslation(input.TextValue),
                    Param6 = DamageTypeTranslation(output.TextValue),
                    Param2 = percentage.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_dmgConversionPerc"),
                    Type = TranslatedStatType.FOOTER
                });
            }
            else {
                result.Add(new TranslatedStat {
                    Param5 = input.TextValue,
                    Param6 = output.TextValue,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_dmgConversion"),
                    Type = TranslatedStatType.FOOTER
                });
            }
        }

        public List<TranslatedStat> ProcessStats(ISet<IItemStat> stats, TranslatedStatType type) {
            List<TranslatedStat> result = new List<TranslatedStat>();

            if (stats == null) {
                return result;
            }

            switch (type) {
                case TranslatedStatType.BODY:
                    ProcessBodyDamage(stats, result);
                    ProccessBodyRetaliation(stats, result);
                    MapSimpleBodyEntries(stats, result);
                    ProcessRacialBonuses(stats, result);
                    ProcessConversionDamage(stats, result);
                    ProcessReducedResistances(stats, result);
                    ProcessFactionWrits(stats, result);
                    ProcessAddMastery(stats, result);

                    ProcessStun(stats, result);
                    ProcessAngleDamage(stats, result);
                    ProcessChainDamage(stats, result);
                    ProcessSlowRetaliation(stats, result);
                    ProcessAddSkill(stats, result);

                    break;

                case TranslatedStatType.HEADER:
                    ProcessShieldBlock(stats, result);
                    ProcessHeaderDamage(stats, result);
                    MapSimpleHeaderEntries(stats, result);
                    ProcessAttackSpeed(stats, result);

                    break;

                case TranslatedStatType.PET: {
                    // In earlier preprocessing the pet stats were prefixed with "pet"
                    List<TranslatedStat> petResults = new List<TranslatedStat>();

                    HashSet<IItemStat> petStats = new HashSet<IItemStat>(stats.Where(m => m.Stat.StartsWith("pet") && m.Stat != "petBonusName")
                        .Select(m => new ItemStat {
                            Stat = m.Stat.Remove(0, 3),
                            TextValue = m.TextValue,
                            Value = m.Value,
                            Record = m.Record
                        }));

                    ProcessShieldBlock(petStats, petResults);
                    ProcessHeaderDamage(petStats, petResults);
                    ProcessBodyDamage(petStats, petResults);
                    ProccessBodyRetaliation(petStats, petResults);
                    MapSimpleHeaderEntries(petStats, petResults);
                    MapSimpleBodyEntries(petStats, petResults);
                    ProcessRacialBonuses(petStats, petResults);
                    ProcessConversionDamage(petStats, petResults);
                    ProcessReducedResistances(petStats, petResults);

                    ProcessStun(petStats, result);
                    ProcessAngleDamage(petStats, result);
                    ProcessChainDamage(petStats, result);

                    result.AddRange(petResults.Select(m => {
                        m.Type = TranslatedStatType.PET;

                        return m;
                    }));
                }

                    break;

                case TranslatedStatType.SKILL:
                    throw new NotImplementedException();

                case TranslatedStatType.FOOTER:
                    throw new NotImplementedException();
            }

            return result;
        }
    }
}