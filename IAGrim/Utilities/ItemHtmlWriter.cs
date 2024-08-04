using IAGrim.Database;
using IAGrim.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EvilsoftCommons.Exceptions;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.Services.ItemReplica;
using IAGrim.UI.Controller.dto;
using log4net;
using Newtonsoft.Json;
using StatTranslator;

namespace IAGrim.Utilities {
    internal static class ItemHtmlWriter {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemHtmlWriter));

        private static JsonStat ToJsonStat(TranslatedStat stat) {
            return new JsonStat {
                Text = stat.Text,
                Param0 = stat.Param0,
                Param1 = stat.Param1,
                Param2 = stat.Param2,
                Param3 = stat.Param3,
                Param4 = stat.Param4,
                Param5 = stat.Param5,
                Param6 = stat.Param6,
                Extras = stat.Extra?.ToString()
            };
        }

        private static JsonSkill GetJsonSkill(PlayerItemSkill skill) {
            return new JsonSkill {
                Name = skill.Name,
                Description = skill.Description,
                Level = skill.Level,
                Trigger = skill.Trigger?.ToString(),
                PetStats = skill.PetStats.Select(ToJsonStat).ToList(),
                BodyStats = skill.BodyStats.Select(ToJsonStat).ToList(),
                HeaderStats = skill.HeaderStats.Select(ToJsonStat).ToList(),
            };
        }


        private static string GetUniqueIdentifier(PlayerHeldItem item) {
            switch (item) {
                case PlayerItem pi:
                    return $"PI/{pi.Id}/{pi.CloudId}";
                case BuddyItem bi:
                    // TODO: Remove this, buddy items are never transferable. Gotta provide a better unique id.
                    return $"BI/{bi.BuddyId}/{bi.RemoteItemId}";
                default:
                    return $"UK/{item.BaseRecord}";
            }
        }

        private static JsonItem GetJsonItem(PlayerHeldItem item) {
            // TODO: Modifiers

            bool isHardcore = false;
            bool isCloudSynced = false;
            object[] transferUrl = { "", "", "", "" };
            string uniqueIdentifier = GetUniqueIdentifier(item);
            List<ItemStatInfo> replicaStats = null;
            var mergeIdentifier = item.BaseRecord ?? string.Empty;
            if (item is PlayerItem pi) {
                transferUrl = new object[] { pi.BaseRecord, pi.PrefixRecord, pi.SuffixRecord, pi.MateriaRecord, pi.Mod, pi.IsHardcore };
                isCloudSynced = pi.IsCloudSynchronized;
                isHardcore = pi.IsHardcore;

                if (!string.IsNullOrEmpty(pi.ReplicaInfo)) {
                    replicaStats = JsonConvert.DeserializeObject<List<ItemStatInfo>>(pi.ReplicaInfo);
                }


                mergeIdentifier += (pi.PrefixRecord ?? string.Empty) + (pi.SuffixRecord ?? string.Empty);
            }
            else if (item is BuddyItem bi) {
                mergeIdentifier += (bi.PrefixRecord ?? string.Empty) + (bi.SuffixRecord ?? string.Empty);
                if (!string.IsNullOrEmpty(bi.ReplicaInfo)) {
                    replicaStats = JsonConvert.DeserializeObject<List<ItemStatInfo>>(bi.ReplicaInfo);
                }
            }

            if (replicaStats != null && replicaStats.Count == 0) {
                replicaStats = null; // Too many things check for null instead of empty, so just set it to null.
            }

            ItemTypeDto type;
            string extras = item.Stash;

            if (item.IsRecipe) {
                type = ItemTypeDto.Recipe;
            }
            else if (!string.IsNullOrEmpty(item.Stash)) {
                type = ItemTypeDto.Buddy;
            }
            else if (item is PlayerItem) {
                type = ItemTypeDto.Player;
            }
            else {
                type = ItemTypeDto.Unknown;
            }

            bool skipStats = replicaStats != null;
            var replicaBodyStats = new List<JsonStat>(0);
            if (skipStats) {
                // Add skillz
                replicaBodyStats = item.BodyStats
                    .Where(m => m.Extra != null)
                    .Select(ToJsonStat)
                    .ToHashSet()
                    .ToList();
            }

            var json = new JsonItem {
                UniqueIdentifier = uniqueIdentifier,
                MergeIdentifier = mergeIdentifier,
                BaseRecord = item.BaseRecord ?? string.Empty,
                URL = transferUrl,
                Icon = item.Bitmap ?? string.Empty,
                Name = PureItemName(item.Name) ?? string.Empty,
                Quality = item.Rarity ?? string.Empty,
                Level = item.MinimumLevel,
                Socket = GetSocketFromItem(item?.Name) ?? string.Empty,
                PetStats = skipStats ? new List<JsonStat>() : item.PetStats.Select(ToJsonStat).ToHashSet().ToList(),
                BodyStats = skipStats ? replicaBodyStats : item.BodyStats.Select(ToJsonStat).ToHashSet().ToList(),
                HeaderStats = skipStats ? new List<JsonStat>() : item.HeaderStats.Select(ToJsonStat).ToHashSet().ToList(),
                Type = type,
                HasRecipe = item.HasRecipe,
                Skill = (item.Skill != null && !skipStats) ? GetJsonSkill(item.Skill) : null,
                GreenRarity = (int)item.PrefixRarity,
                HasCloudBackup = isCloudSynced,
                Slot = SlotTranslator.Translate(RuntimeSettings.Language, item.Slot ?? ""),
                Extras = extras,
                IsMonsterInfrequent = item.ModifiedSkills.Any(s => s.IsMonsterInfrequent),
                IsHardcore = isHardcore,
                ReplicaStats = replicaStats,
            };

            if (!skipStats) {
                var modifiedSkills = item.ModifiedSkills;
                foreach (var modifiedSkill in modifiedSkills) {
                    var translated = modifiedSkill.Translated;
                    foreach (var stat in translated.Select(ToJsonStat)) {
                        json.BodyStats.Add(stat);
                    }
                }
            }


            return json;
        }

        /// <summary>
        /// Copy any css/js files from the app\resource folder to the items working directory
        /// </summary>
        public static void CopyMissingFiles() {
            Logger.Debug("Copying missing files / etc to IA storage folder");
            string appResFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");

            foreach (string dirPath in Directory.GetDirectories(appResFolder, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(appResFolder, GlobalPaths.StorageFolder));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(appResFolder, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(appResFolder, GlobalPaths.StorageFolder), true);
            }

            Logger.Debug("Copy complete");
        }

        public static List<List<JsonItem>> ToJsonSerializable(List<PlayerHeldItem> items) {
            var jsonItems = items.Select(GetJsonItem).ToList();

            var merged = ItemOperationsUtility.MergeStackSize(jsonItems);
            foreach (var itemList in merged) {
                itemList.Sort();
            }

            return merged;
        }


        private static string GetSocketFromItem(string name) {
            if (!string.IsNullOrEmpty(name) && name.Contains("[")) {
                string[] tmp = name.Split('[');
                return $"({tmp[1].Replace("]", "")})";
            }
            else {
                return string.Empty;
            }
        }


        /// <summary>
        /// Strip any socket from item name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string PureItemName(string name) {
            if (!string.IsNullOrEmpty(name) && name.Contains("[")) {
                string[] tmp = name.Split('[');
                return tmp[0].Trim();
            }
            else {
                return name;
            }
        }
    }
}