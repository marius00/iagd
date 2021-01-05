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

        /// <summary>
        /// Merges identical items, summing up the stacksize.
        /// </summary>
        public static List<T> MergeStackSize<T>(IEnumerable<T> items) where T : BaseItem, PlayerHeldItem {
            Dictionary<string, T> map = new Dictionary<string, T>();
            foreach (var item in items) {
                
                var key = item.BaseRecord + item.PrefixRecord + item.SuffixRecord + item.ModifierRecord;
                if (map.ContainsKey(key)) {
                    map[key].Count += item.Count;
                }
                else {
                    map[key] = item;
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
            var classifications = stats.Where(m => records.Contains(m.Key))
                .SelectMany(m => m.Value.Where(v => v.Stat == "itemClassification"))
                .Select(m => m.TextValue);

            return TranslateClassification(classifications);
        }

        public static string GetRarityForRecord(Dictionary<string, List<DBStatRow>> stats, string record) {
            var classifications = stats.Where(m => record.Equals(m.Key))
                .SelectMany(m => m.Value.Where(v => v.Stat == "itemClassification"))
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
            var levels = stats.Where(m => records.Contains(m.Key))
                .SelectMany(m => m.Value.Where(v => v.Stat == "levelRequirement"))
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
        public static string GetItemName(ISession session, Dictionary<string, List<DBStatRow>> stats, RecordCollection item) {
            // Grab tags

            var records = new string[] { item.PrefixRecord, item.BaseRecord, item.SuffixRecord, item.MateriaRecord };
            var desiredTagsNames = new string[] { "lootRandomizerName", "itemNameTag", "itemQualityTag", "itemStyleTag", "description" };
            var relevants = stats.Where(m => records.Contains(m.Key));
            var tagEntries = relevants.SelectMany(m => m.Value.Where(v => desiredTagsNames.Contains(v.Stat))).ToList();


            // Grab tag values
            ICriteria tagCrit = session.CreateCriteria<ItemTag>();
            tagCrit.Add(Restrictions.In("Tag", tagEntries.Select(m => m.TextValue).ToArray()));
            var tags = tagCrit.List<ItemTag>();


            string prefix = string.Empty;
            {
                var prefixEntry = tagEntries.FirstOrDefault(m => m.Record == item.PrefixRecord && m.Stat == "lootRandomizerName");
                if (prefixEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == prefixEntry.TextValue);
                    prefix = tag?.Name ?? prefixEntry.TextValue;
                }
            }

            string suffix = string.Empty;
            {
                var suffixEntry = tagEntries.FirstOrDefault(m => m.Record == item.SuffixRecord && m.Stat == "lootRandomizerName");
                if (suffixEntry != null) {
                    suffix = tags.FirstOrDefault(m => m.Tag == suffixEntry.TextValue)?.Name ?? suffixEntry.TextValue;
                }
            }

            string core = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemNameTag");
                if (coreEntry == null) // Potions
                    coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "description");

                if (coreEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == coreEntry.TextValue);
                    core = tag?.Name ?? coreEntry.TextValue;
                }
            }

            string quality = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemQualityTag");
                if (coreEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == coreEntry.TextValue);
                    quality = tag?.Name ?? coreEntry.TextValue;
                }
            }

            string style = string.Empty;
            {
                var coreEntry = tagEntries.FirstOrDefault(m => m.Record == item.BaseRecord && m.Stat == "itemStyleTag");
                if (coreEntry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == coreEntry.TextValue);
                    style = tag?.Name ?? coreEntry.TextValue;
                }
            }

            string materia = string.Empty;
            {
                var entry = tagEntries.FirstOrDefault(m => m.Record == item.MateriaRecord && m.Stat == "description");
                if (entry != null) {
                    var tag = tags.FirstOrDefault(m => m.Tag == entry.TextValue);
                    materia = tag?.Name ?? entry.TextValue;

                    materia = $" [{materia}]";
                }
            }


            string localizedName = RuntimeSettings.Language.TranslateName(prefix, quality, style, core, suffix);
            return localizedName + materia;
        }
    }
}
