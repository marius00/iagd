using DataAccess;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StatTranslator {
    public class StatManager {
        private static readonly string[] BodyDamageTypes = { "SlowPoison", "SlowPhysical", "SlowBleeding", "Bleeding", "SlowLife", "SlowFire", "SlowCold", "SlowLightning", "Poison", "Chaos", "Fire", "Aether", "Bleeding", "Cold", "Lightning", "Elemental", "Pierce", "Physical", "Life", "TotalDamage", "PercentCurrentLife" };
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StatManager));
        private readonly ILocalizedLanguage _language;

        public StatManager(ILocalizedLanguage language) {
            this._language = language;
        }


        private void ProcessConversionDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            var conversionPercentage = stats.FirstOrDefault(m => m.Stat == "conversionPercentage");
            var conversionOutType = stats.FirstOrDefault(m => m.Stat == "conversionOutType");
            var conversionInType = stats.FirstOrDefault(m => m.Stat == "conversionInType");
            if (conversionPercentage != null && conversionOutType != null && conversionInType != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_damage_conversion"),
                    Param0 = conversionPercentage.Value,
                    Param3 = DamageTypeTranslation(conversionInType.TextValue),
                    Param5 = DamageTypeTranslation(conversionOutType.TextValue),
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
            // "augmentSkill1", "augmentSkill2",
            for (int i = 1; i <= 4; i++) {
                var statName = "augmentSkill" + i;
                var stat = stats.FirstOrDefault(m => m.Stat == statName);
                var statExtras = stats.FirstOrDefault(m => m.Stat == statName + "Extras");
                TranslatedStat extraStat = null;
                if (statExtras != null) {

                    extraStat = new TranslatedStat {
                        Text = _language.GetTag(statName + "Extras"),
                        Param0 = statExtras.Value,
                        Param3 = _language.GetTag(statExtras.TextValue)
                    };
                }
                if (stat != null) {
                    result.Add(new TranslatedStat {
                        Text = _language.GetTag(statName),
                        Param0 = stat.Value,
                        Param3 = stat.TextValue,
                        Extra = extraStat
                    });
                }
            }
        }
        private void ProcessAddMastery(ISet<IItemStat> stats, List<TranslatedStat> result) {
            // "augmentSkill1", "augmentSkill2",
            for (int i = 1; i <= 4; i++) {
                var statName = "augmentMastery" + i;
                var stat = stats.FirstOrDefault(m => m.Stat == statName);

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
            var min = stats.FirstOrDefault(m => m.Stat == "offensiveStunMin");
            var max = stats.FirstOrDefault(m => m.Stat == "offensiveStunMax");
            var chance = stats.FirstOrDefault(m => m.Stat == "offensiveStunChance");

            if (min != null) {
                string tag;
                if (max != null) {
                    tag = _language.GetTag("offensiveStunMax");
                } else {
                    tag = _language.GetTag("offensiveStunMin");
                }

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
        }

        private void ProcessAngleDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            var angle = stats.FirstOrDefault(m => m.Stat == "skillTargetAngle");
            var numTargets = stats.FirstOrDefault(m => m.Stat == "skillTargetNumber");
            if (angle != null) {
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("skillTargetAngle"),
                    Param0 = numTargets.Value,
                    Param1 = angle.Value
                });
            }
        }

        private void ProcessChainDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            var sparkChance = stats.FirstOrDefault(m => m.Stat == "sparkChance");
            var sparkGap = stats.FirstOrDefault(m => m.Stat == "sparkGap");
            var sparkMaxNumber = stats.FirstOrDefault(m => m.Stat == "sparkMaxNumber");
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
            var faction = stats.FirstOrDefault(m => m.Stat == "boostedFaction");
            var multiplier = stats.FirstOrDefault(m => m.Stat == "boostedMultiplier");
            
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
            var chance = stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionPercentChance");
            var duration = stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionPercentDurationMin");
            var min = stats.FirstOrDefault(m => m.Stat == "offensiveTotalResistanceReductionPercentMin");
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
            var racialBonusPercentDamage = stats.FirstOrDefault(m => m.Stat == "racialBonusPercentDamage");
            var racialBonusRace = stats.Where(m => m.Stat == "racialBonusRace").ToList();
            if (racialBonusPercentDamage != null && racialBonusRace.Count >= 1) {
                var race01 = _language.GetTag(racialBonusRace.FirstOrDefault().TextValue);
                var race02 = racialBonusRace.Count >= 2 ? _language.GetTag(racialBonusRace.FirstOrDefault().TextValue) : null;
                var tag = race02 == null ? _language.GetTag("customtag_damage_racial") : _language.GetTag("customtag_damage_racial02");
                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_damage_racial"),
                    Param0 = racialBonusPercentDamage.Value,
                    Param3 = race01,
                    Param5 = race02,
                    Type = TranslatedStatType.BODY
                });
            }

            var racialBonusPercentDefense = stats.FirstOrDefault(m => m.Stat == "racialBonusPercentDefense");
            if (racialBonusPercentDefense != null && racialBonusRace.Count > 1) {
                var race01 = _language.GetTag(racialBonusRace[0].TextValue);
                var race02 = racialBonusRace.Count >= 2 ? _language.GetTag(racialBonusRace.FirstOrDefault().TextValue) : null;

                if (race02 == null) {
                    result.Add(new TranslatedStat {
                        Text = _language.GetTag("racialBonusPercentDefense"),
                        Param0 = racialBonusPercentDefense.Value,
                        Param3 = race01,
                        Type = TranslatedStatType.BODY
                    });
                } else {
                    result.Add(new TranslatedStat {
                        Text = _language.GetTag("racialBonusPercentDefense02"),
                        Param0 = racialBonusPercentDefense.Value,
                        Param3 = race01,
                        Param5 = race02,
                        Type = TranslatedStatType.BODY
                    });
                }
            }

        }


        private void ProcessAttackSpeed(ISet<IItemStat> stats, List<TranslatedStat> result) {
            var characterBaseAttackSpeed = stats.Where(m => m.Stat == "characterBaseAttackSpeed").FirstOrDefault();
            if (characterBaseAttackSpeed != null) {
                var tag = stats.Where(m => m.Stat == "characterBaseAttackSpeedTag").FirstOrDefault()?.TextValue;
                if (tag != null)
                    tag = _language.GetTag(tag);
                else
                    tag = "Unknown";

                result.Add(new TranslatedStat {
                    Text = _language.GetTag("customtag_speed"),
                    Param0 = characterBaseAttackSpeed.Value,
                    Param3 = tag
                });
            }
        }

        private void ProcessShieldBlock(ISet<IItemStat> stats, List<TranslatedStat> result) {
            var defensiveBlockChance = stats.Where(m => m.Stat == "defensiveBlockChance").FirstOrDefault();
            if (defensiveBlockChance != null) {
                var defensiveBlock = stats.Where(m => m.Stat == "defensiveBlock").FirstOrDefault();
                var blockAbsorption = stats.Where(m => m.Stat == "blockAbsorption").FirstOrDefault();

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
        }


        private string DamageTypeTranslation(string d) {
            d = d.Replace("Modifier", "");

            var localized = _language.GetTag(d);
            if (!string.IsNullOrEmpty(localized))
                return localized;
            else {
                Logger.Warn($"Missing translations for tag \"{d}\"");
                return d.Replace("Base", "");
            }
        }
        
        private void ProcessHeaderDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            string[] _damageTypes = { "BasePoison", "BaseChaos", "BaseFire", "BaseAether", "BaseCold", "BaseLightning", "BasePierce", "BasePhysical", "BaseLife" };
            var damageTypes = _damageTypes.Select(m => string.Format("offensive{0}Min", m)).ToList();
            _ProcessDamage(stats, result, damageTypes, TranslatedStatType.HEADER);
        }


        private void ProcessBodyDamage(ISet<IItemStat> stats, List<TranslatedStat> result) {
            var damageTypes = BodyDamageTypes.Select(m => $"offensive{m}Min").ToList();
            _ProcessDamage(stats, result, damageTypes, TranslatedStatType.BODY);


            damageTypes = BodyDamageTypes.Select(m => $"offensive{m}Modifier").ToList();
            _ProcessDamage(stats, result, damageTypes, TranslatedStatType.BODY);

            //translationTable[string.Format("offensive{0}Modifier", damageTypes[i])] = "+{0}% " + DamageTypeTranslation(damageTypes[i]) + " Damage";
        }

        private void ProccessBodyRetaliation(ISet<IItemStat> stats, List<TranslatedStat> result) {
            var damageTypes = BodyDamageTypes.Select(m => string.Format("retaliation{0}Min", m)).ToList();

            var candidates = stats.Where(m => damageTypes.Contains(m.Stat));
            foreach (var minDmg in candidates) {
                var type = minDmg.Stat.Replace("retaliation", "").Replace("Min", "");
                var maxDmg = stats.Where(m => m.Stat.Equals(string.Format("retaliation{0}Max", type))).FirstOrDefault();
                var duration = stats.Where(m => m.Stat.Equals(string.Format("retaliation{0}DurationMin", type))).FirstOrDefault();
                
                
                float minDmgVal = minDmg?.Value ?? 0;

                StringBuilder sb = new StringBuilder();
                if (maxDmg != null) {
                    sb.Append(_language.GetTag("customtag_013_retaliation"));
                } else {
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

        private void _ProcessDamage(ISet<IItemStat> stats, List<TranslatedStat> result, List<string> damageTypes, TranslatedStatType location) {

            var candidates = stats.Where(m => damageTypes.Contains(m.Stat));
            foreach (var minDmg in candidates) {
                
                var type = minDmg.Stat.Replace("offensive", "").Replace("Min", "");
                var maxDmg = stats.FirstOrDefault(m => m.Stat.Equals(string.Format("offensive{0}Max", type)));
                var chance = stats.FirstOrDefault(m => m.Stat.Equals(string.Format("offensive{0}Chance", type)));
                var duration = stats.FirstOrDefault(m => m.Stat.Equals(string.Format("offensive{0}DurationMin", type)));

                var minDmgVal = minDmg.Value;

                StringBuilder sb = new StringBuilder();

                if (chance != null) {
                    sb.Append(_language.GetTag("customtag_damage_chanceof"));
                }
                if (maxDmg != null) {
                    sb.Append(_language.GetTag("customtag_damage_123"));
                }
                else {
                    if (type.Contains("Modifier"))
                        sb.Append(_language.GetTag("customtag_damage_13%"));
                    else
                        sb.Append(_language.GetTag("customtag_damage_13"));
                }

                if (duration != null) {
                    sb.Append(_language.GetTag("customtag_damage_delay"));
                    minDmgVal *= duration.Value;
                }

                //
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
            string[] tags = new[] { "offensivePierceRatioMin", "defensiveProtection", "skillChanceWeight", "skillProjectileNumber",
                "skillCooldownTime", "skillManaCost", "skillTargetRadius", "skillActiveDuration" };

            var headerTranslationTable = new Dictionary<string, string>();
            foreach (var tag in tags)
                headerTranslationTable[tag] = _language.GetTag(tag);

            foreach (var elem in stats.Where(m => headerTranslationTable.Keys.Contains(m.Stat))) {
                result.Add(new TranslatedStat {
                    Text = headerTranslationTable[elem.Stat],
                    Param0 = elem.Value
                });
            }
        }

        public TranslatedStat TranslateSkillAutoController(string record) {
            var mapping = new Dictionary<string, string> {
                {"records/controllers/itemskills/cast_@allyonattack_", "-" },
                {"records/controllers/itemskills/cast_@allyonlowhealth", "-" },
                {"records/controllers/itemskills/cast_@enemylocationonkill", "" },
                {"records/controllers/itemskills/cast_@enemyonanyhit_", "" },
                {"records/controllers/itemskills/cast_@enemyonattack", "{0}% Chance on attack" },
                {"records/controllers/itemskills/cast_@enemyonattackcrit", "{0}% Chance on a critical attack (target enemy)" },
                {"records/controllers/itemskills/cast_@enemyonblock", "{0}% Chance when blocked" },
                {"records/controllers/itemskills/cast_@enemyonhitcritical", "{0}% Chance when hit by a critical" },
                {"records/controllers/itemskills/cast_@enemyonkill", "{0}% Chance on Enemy Death" },  //C
                {"records/controllers/itemskills/cast_@enemyonmeleehit", "{0}% Chance when hit by a melee attack" },
                {"records/controllers/itemskills/cast_@enemyonprojectilehit", "{0}% NPSkill Proc" },
                {"records/controllers/itemskills/cast_@selfonanyhit", "{0}% Chance when hit" }, //C
                {"records/controllers/itemskills/cast_@selfonattack", "{0}% Chance on attacking" },
                {"records/controllers/itemskills/cast_@selfonattackcrit", "{0}% Chance on a critical attack" },
                {"records/controllers/itemskills/cast_@selfonblock", "{0}% Chance when blocking" },
                {"records/controllers/itemskills/cast_@selfonhitcritica", "{0}% Chance when Hit by a Critical" },//C
                {"records/controllers/itemskills/cast_@selfonkill", "{0}% Chance on Enemy Death" }, //C
                {"records/controllers/itemskills/cast_@selfonlowhealth", "{0}% Chance at 25% health" },
                {"records/controllers/itemskills/cast_@selfonmeleehit", "{0}% Chance when Hit by Melee Attacks" },
                {"records/controllers/itemskills/cast_@selfonprojectilehit", "{0}% Chance when Hit by Ranged Attacks" },
            };
            

            try {
                // The tag names are 'cast_@allyonattack' etc, without the preceding record path, and without the parameterized chance%

                var tagName = record.Replace("records/controllers/itemskills/", "");
                tagName = tagName.Substring(0, tagName.IndexOf('_', 5));

                var localized = _language.GetTag(tagName);
                if (!string.IsNullOrEmpty(localized)) {
                    var result = System.Text.RegularExpressions.Regex.Match(record, @"\d+").Value;
                    return new TranslatedStat {
                        Param0 = float.Parse(result),
                        Text = localized
                    };
                }                
                else if (record.StartsWith("records/controllers/itemskills/cast_@selfat")) {
                    var result = System.Text.RegularExpressions.Regex.Match(record, @"\d+").Value;
                    return new TranslatedStat {
                        Param0 = 100, // this is bound to be wrong some day
                        Param1 = float.Parse(result),
                        Text = _language.GetTag("cast_@selfat")
                    };
                }
            } catch (FormatException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            } catch (ArgumentOutOfRangeException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }

            return new TranslatedStat();
        }


        private void MapSimpleBodyEntries(ISet<IItemStat> stats, List<TranslatedStat> result) {
            string[] tags = new[] { "weaponDamagePct", "offensivePercentCurrentLifeMin", "characterLife", "characterLifeModifier",
                "augmentAllLevel", "characterDefensiveAbility", "characterOffensiveAbility", "characterDefensiveAbilityModifier",
                "characterOffensiveAbilityModifier", "defensiveBlockModifier",
                "defensivePetrify", "offensiveCritDamageModifier", "characterRunSpeedModifier", "characterIntelligenceModifier",
                "skillCooldownReduction", "retaliationTotalDamageModifier", "characterAttackSpeedModifier", "characterAttackSpeed",
                "offensiveLifeLeechMin", "characterIntelligence", "characterManaRegen", "characterManaRegenModifier", "characterLightRadius",
                "characterDodgePercent", "piercingProjectile", "characterMana", "characterManaModifier", "characterEnergyAbsorptionPercent",
                "characterSpellCastSpeedModifier", "defensiveReflect", "blockRecoveryTime", "characterLifeRegen", "characterDexterity",
                "defensiveTrap", "characterLifeRegenModifier", "characterDeflectProjectile", "characterConstitutionModifier",
                "characterHuntingDexterityReqReduction", "characterStrength", "characterStrengthModifier", "characterTotalSpeedModifier",
                "skillProjectileSpeedModifier", "defensiveAbsorptionModifier", "skillLifeBonus", "skillLifePercent",
                "defensiveTotalSpeedResistance", "offensiveTauntMin", "defensiveBlockAmountModifier", "characterDefensiveBlockRecoveryReduction",
                "defensiveProtectionModifier", "projectilePiercingChance", "skillProjectileNumber", "projectileExplosionRadius"
            };

            Dictionary<string, string> translationTable = new Dictionary<string, string>();
            foreach (var tag in tags)
                translationTable[tag] = _language.GetTag(tag);
            

            string[] damageTypes = BodyDamageTypes;
            var resistance = _language.GetTag("Resistance");

            for (var i = 0; i < damageTypes.Length; i++) {
                var r = DamageTypeTranslation(damageTypes[i]);
                translationTable[$"defensive{damageTypes[i]}"] = "{0}% " + r + " " + resistance;
                translationTable[$"defensive{damageTypes[i]}Resistance"] = "{0}% " + r + " " + resistance;
                
                translationTable[$"defensive{damageTypes[i]}MaxResist"] = "{0}% to Maximum " + r + " " + resistance;
            }

            foreach (var elem in stats.Where(m => translationTable.Keys.Contains(m.Stat))) {
                result.Add(new TranslatedStat {
                    Text = translationTable[elem.Stat],
                    Param0 = (float)Math.Round(elem.Value, 1, MidpointRounding.AwayFromZero),
                    Param3 = elem.TextValue
                });
            }
        }

        public List<TranslatedStat> ProcessSkillModifierStats(ISet<IItemStat> stats, string skill) {
            List<TranslatedStat> result = new List<TranslatedStat>();
            var weaponDamageEffect = stats.FirstOrDefault(m => m.Stat == "weaponDamagePct");
            var offensivePhysicalResistanceReduction = stats.FirstOrDefault(m => m.Stat == "offensivePhysicalResistanceReductionAbsoluteMin");
            var petLimit = stats.FirstOrDefault(m => m.Stat == "petLimit");

            if (weaponDamageEffect != null) {
                result.Add(new TranslatedStat {
                    Param0 = weaponDamageEffect.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_weaponDamagePct"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            if (offensivePhysicalResistanceReduction != null) {
                var offensivePhysicalResistanceReductionAbsoluteDurationMin = stats.FirstOrDefault(m => m.Stat == "offensivePhysicalResistanceReductionAbsoluteDurationMin");
                if (offensivePhysicalResistanceReductionAbsoluteDurationMin != null) {
                    result.Add(new TranslatedStat {
                        Param0 = offensivePhysicalResistanceReduction.Value,
                        Param1 = offensivePhysicalResistanceReductionAbsoluteDurationMin.Value,
                        Param3 = skill,
                        Text = _language.GetTag("customtag_xpac_modif_physicalResistDuration"),
                        Type = TranslatedStatType.FOOTER
                    });

                }
                else {
                    result.Add(new TranslatedStat {
                        Param0 = offensivePhysicalResistanceReduction.Value,
                        Param3 = skill,
                        Text = _language.GetTag("customtag_xpac_modif_physicalResist"),
                        Type = TranslatedStatType.FOOTER
                    });

                }
                // 
            }



            if (petLimit != null) {
                result.Add(new TranslatedStat {
                    Param0 = petLimit.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_petLimit"),
                    Type = TranslatedStatType.FOOTER
                });
            }



            var conversionIn1 = stats.FirstOrDefault(m => m.Stat == "conversionInType");
            var conversionOutType1 = stats.FirstOrDefault(m => m.Stat == "conversionOutType");
            var conversionPercentage1 = stats.FirstOrDefault(m => m.Stat == "conversionPercentage");
            AddConversionStat(result, skill, conversionIn1, conversionOutType1, conversionPercentage1);

            var conversionIn2 = stats.FirstOrDefault(m => m.Stat == "conversionInType2");
            var conversionOutType2 = stats.FirstOrDefault(m => m.Stat == "conversionOutType2");
            var conversionPercentage2 = stats.FirstOrDefault(m => m.Stat == "conversionPercentage2");
            AddConversionStat(result, skill, conversionIn2, conversionOutType2, conversionPercentage2);

            // 12-85 Lightning Damage to Callidor's Tempest
            // "offensiveLightningMax"
            // "offensiveLightningMin"
            foreach (var damageType in BodyDamageTypes) {
                var min = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}Min");
                var max = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}Max");
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

                    var offensiveXDurationModifier = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}DurationModifier");
                    var offensiveXModifier = stats.FirstOrDefault(m => m.Stat == $"offensive{damageType}Modifier");
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


                var defensive = stats.FirstOrDefault(m => m.Stat == $"defensive{damageType}");
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

            var projectileLaunchNumber = stats.FirstOrDefault(m => m.Stat == "projectileLaunchNumber");
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

            var sparkChance = stats.FirstOrDefault(m => m.Stat == "sparkChance");
            var sparkMaxNumber = stats.FirstOrDefault(m => m.Stat == "sparkMaxNumber");
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

            var offensiveSlowDefensiveAbilityDurationMin = stats.FirstOrDefault(m => m.Stat == "offensiveSlowDefensiveAbilityDurationMin");
            var offensiveSlowDefensiveAbilityMin = stats.FirstOrDefault(m => m.Stat == "offensiveSlowDefensiveAbilityMin");
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


            var skillCooldownReductionChance = stats.FirstOrDefault(m => m.Stat == "skillCooldownReductionChance");
            var skillCooldownReduction = stats.FirstOrDefault(m => m.Stat == "skillCooldownReduction");
            if (skillCooldownReduction != null && skillCooldownReductionChance != null) {
                result.Add(new TranslatedStat {
                    Param0 = skillCooldownReductionChance.Value,
                    Param1 = skillCooldownReduction.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_skillCooldownReductionChance"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            var offensiveTotalDamageReductionPercentMin = stats.FirstOrDefault(m => m.Stat == "offensiveTotalDamageReductionPercentMin");
            var offensiveTotalDamageReductionPercentDurationMin = stats.FirstOrDefault(m => m.Stat == "offensiveTotalDamageReductionPercentDurationMin");
            if (offensiveTotalDamageReductionPercentMin != null && offensiveTotalDamageReductionPercentDurationMin != null) {
                result.Add(new TranslatedStat {
                    Param0 = offensiveTotalDamageReductionPercentMin.Value,
                    Param1 = offensiveTotalDamageReductionPercentDurationMin.Value,
                    Param3 = skill,
                    Text = _language.GetTag("customtag_xpac_modif_offensiveTotalDamageReductionPercentDurationMin"),
                    Type = TranslatedStatType.FOOTER
                });
            }

            AddSimpleStat("offensiveDamageMultModifier", "customtag_xpac_modif_offensiveDamageMultModifier", stats, skill, result);

            if (stats.Count > 0 && result.Count == 0) {
                Logger.Warn("No stats parsed for modifiers");
                foreach (var stat in stats) {
                    Logger.Debug($"{stat.Stat}: {stat.Value}, {stat.TextValue}");
                }
            }
            return result;
        }

        private void AddSimpleStat(string statName, string translationTag, ISet<IItemStat> stats, string skill, List<TranslatedStat> result) {
            var stat = stats.FirstOrDefault(m => m.Stat == statName);
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
            if (input == null || output == null)
                return;
            
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

            // null plays havoc with AutoMapper TODO: Is this still required? automapper is no longer being used
            if (stats == null)
                return result;

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
                    ProcessAddSkill(stats, result);
                    
                    break;

                case TranslatedStatType.HEADER:
                    ProcessShieldBlock(stats, result);
                    ProcessHeaderDamage(stats, result);
                    MapSimpleHeaderEntries(stats, result);
                    ProcessAttackSpeed(stats, result);
                    //ProcessSkillTrigger(stats, result); //
                    break;
                case TranslatedStatType.PET: {
                        // In earlier preprocessing the pet stats were prefixed with "pet"
                        List<TranslatedStat> petResults = new List<TranslatedStat>();

                        var petStats = new HashSet<IItemStat>(stats.Where(m => m.Stat.StartsWith("pet") && m.Stat != "petBonusName")
                            .Select(m => {
                                return new ItemStat {
                                    Stat = m.Stat.Remove(0, 3),
                                    TextValue = m.TextValue,
                                    Value = m.Value
                                };
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

                        result.AddRange(petResults.Select(m => { m.Type = TranslatedStatType.PET; return m; }));
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
