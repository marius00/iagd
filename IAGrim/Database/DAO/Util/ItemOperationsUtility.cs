using System;
using System.Collections.Generic;
using System.Linq;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Model;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using NHibernate;
using NHibernate.Criterion;
using StatTranslator;

namespace IAGrim.Database.DAO.Util {
    static class ItemOperationsUtility {

        public static List<List<PlayerHeldItem>> MergeStackSize(IEnumerable<PlayerHeldItem> items) {
            Dictionary<string, List<PlayerHeldItem>> map = new Dictionary<string, List<PlayerHeldItem>>();
            foreach (var item in items) {
                var mergeIdentifier = item.BaseRecord ?? string.Empty;
                if (item is PlayerItem pi) {
                    mergeIdentifier += (pi.PrefixRecord ?? string.Empty) + (pi.SuffixRecord ?? string.Empty);
                }
                else if (item is BuddyItem bi) {
                    mergeIdentifier += (bi.PrefixRecord ?? string.Empty) + (bi.SuffixRecord ?? string.Empty);
                }

                var key = mergeIdentifier;
                if (map.ContainsKey(key)) {
                    map[key].Add(item);
                }
                else {
                    map[key] = new List<PlayerHeldItem>() { item };
                }
            }

            return map.Values.ToList();
        }


        /// <summary>
        /// Calculate the item rarity for a given set of records (suffix, prefix, etc)
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static string GetRarityForRecords(Dictionary<string, List<DBStatRow>> stats, List<string> records) {
            var classifications = records
                .Where(stats.ContainsKey)
                .SelectMany(record => stats[record].Where(v => v.Stat == "itemClassification"))
                .Select(m => m.TextValue);

            return TranslateClassification(classifications);
        }

        public static string GetRarityForRecord(Dictionary<string, List<DBStatRow>> stats, string record) {
            var classifications = (stats.TryGetValue(record, out var rows) ? rows : Enumerable.Empty<DBStatRow>())
                .Where(v => v.Stat == "itemClassification")
                .Select(m => m.TextValue);

            return TranslateClassification(classifications);
        }

        public static string TranslateClassification(IEnumerable<string> classifications) {
            var enumerable = classifications as string[] ?? classifications.ToArray();
            if (enumerable.Contains("Legendary"))
                return "Epic";
            else if (enumerable.Contains("Epic"))
                return "Blue";
            else if (enumerable.Contains("Rare"))
                return "Green";
            else if (enumerable.Contains("Magical"))
                return "Yellow";
            else if (enumerable.Contains("Common"))
                return "White";
            else
                return "Unknown";
        }

        public static string TranslateFaction(ILocalizedLanguage language, string faction) {
            
            switch (faction) {
                case "Survivors":
                    return language.GetTag("tagFactionSurvivors");
                case "User0":
                    return language.GetTag("tagFactionUser0");
                case "User2":
                    return language.GetTag("tagFactionUser2");
                case "User4":
                    return language.GetTag("tagFactionUser4");
                case "User5":
                    return language.GetTag("tagFactionUser5");
                case "User7":
                    return language.GetTag("tagFactionUser7");
                case "User8":
                    return language.GetTag("tagFactionUser8");
                case "User9":
                    return language.GetTag("tagFactionUser9_ia");
                case "User10":
                    return language.GetTag("tagFactionUser10_ia");
                case "User11":
                    return language.GetTag("tagFactionUser11_ia");
                default:
                    return faction;
            }
        }

        public static float GetMinimumLevelForRecord(Dictionary<string, List<DBStatRow>> stats, string record) {
            return GetMinimumLevelForRecords(stats, new List<string> {
                record
            });
        }

        public static float GetMinimumLevelForRecords(Dictionary<string, List<DBStatRow>> stats, List<string> records) {
            var levels = records
                .Where(stats.ContainsKey)
                .SelectMany(record => stats[record].Where(v => v.Stat == "levelRequirement"))
                .Select(m => m.Value)
                .ToList();


            if (levels.Count == 0)
                return 0;
            else
                return (float)levels.Max<double>();
        }

        /// <summary>
        /// Update the item name to include suffix/affix/etc
        /// </summary>
        public static string GetItemName(Dictionary<string, string> itemTags, Dictionary<string, List<DBStatRow>> stats, RecordCollection item) {
            // Grab tags

            var records = new string[] { item.PrefixRecord, item.BaseRecord, item.SuffixRecord, item.MateriaRecord };
            var desiredTagsNames = new string[] { "lootRandomizerName", "itemNameTag", "itemQualityTag", "itemStyleTag", "description" };
            var tagEntries = records
                .Where(r => r != null && stats.ContainsKey(r))
                .SelectMany(record => stats[record].Where(v => desiredTagsNames.Contains(v.Stat)))
                .ToList();

            // Resolve a tag to its localized name, mirroring the old FirstOrDefault(..)?.Name lookup
            string TagName(string tag) => tag != null && itemTags.TryGetValue(tag, out var n) ? n : null;


            string prefix = string.Empty;
            {
                var prefixEntry = tagEntries.FirstOrDefault(m => m.Record == item.PrefixRecord && m.Stat == "lootRandomizerName");
                if (prefixEntry != null) {
                    prefix = TagName(prefixEntry.TextValue) ?? prefixEntry.TextValue;
                }
            }

            string suffix = string.Empty;
            {
                var suffixEntry = tagEntries.FirstOrDefault(m => m.Record == item.SuffixRecord && m.Stat == "lootRandomizerName");
                if (suffixEntry != null) {
                    suffix = TagName(suffixEntry.TextValue) ?? suffixEntry.TextValue;
                }
            }

            string core = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemNameTag");
                if (coreEntry == null) // Potions
                    coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "description");

                if (coreEntry != null) {
                    core = TagName(coreEntry.TextValue) ?? coreEntry.TextValue;
                }
            }

            string quality = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemQualityTag");
                if (coreEntry != null) {
                    quality = TagName(coreEntry.TextValue) ?? coreEntry.TextValue;
                }
            }

            string style = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemStyleTag");
                if (coreEntry != null) {
                    style = TagName(coreEntry.TextValue) ?? coreEntry.TextValue;
                }
            }

            string materia = string.Empty;
            {
                var entry = tagEntries.FirstOrDefault(m => m.Record == item.MateriaRecord && m.Stat == "description");
                if (entry != null) {
                    materia = TagName(entry.TextValue) ?? entry.TextValue;

                    materia = $" [{materia}]";
                }
            }


            string localizedName = RuntimeSettings.Language.TranslateName(prefix, quality, style, core, suffix);
            return localizedName + materia;
        }
    }
}
