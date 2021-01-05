using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using EvilsoftCommons.Exceptions;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.UI.Controller.dto;
using log4net;
using Newtonsoft.Json;
using NHibernate.Dialect.Schema;
using StatTranslator;

namespace IAGrim.Utilities {

    internal static class ItemHtmlWriter {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemHtmlWriter));


        static ItemHtmlWriter() {
            CopyMissingFiles();
        }


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



        public static JsonItem GetJsonItem(PlayerHeldItem item) {
            // TODO: Modifiers

            bool isCloudSynced = false;
            object[] id = { "", "", "", "" };
            if (item is PlayerItem pi) {
                id = new object[] { pi.BaseRecord, pi.PrefixRecord, pi.SuffixRecord, pi.MateriaRecord };
                isCloudSynced = pi.IsCloudSynchronized;
            }


            ItemTypeDto type;
            string extras = string.Empty;

            if (item.IsRecipe) {
                type = ItemTypeDto.Recipe;
            }
            else if (!string.IsNullOrEmpty(item.Stash)) {
                type = ItemTypeDto.Buddy;
            }
            else if (item is PlayerItem) {
                type = ItemTypeDto.Player;
            }
            else if (item is AugmentationItem) {
                type = ItemTypeDto.Augmentation;
                extras = ItemOperationsUtility.TranslateFaction(
                    RuntimeSettings.Language,
                    ((AugmentationItem) item).Tags.FirstOrDefault(m => m.Stat == "factionSource")?.TextValue ?? string.Empty
                );
            }
            else {
                type = ItemTypeDto.Unknown;
            }



            var json = new JsonItem {
                BaseRecord = item.BaseRecord ?? string.Empty,
                URL = id,
                Icon = item.Bitmap ?? string.Empty,
                Name = PureItemName(item.Name) ?? string.Empty,
                Quality = item.Rarity ?? string.Empty,
                Level = item.MinimumLevel,
                Socket = GetSocketFromItem(item?.Name) ?? string.Empty,
                NumItems = (uint)item.Count,
                InitialNumItems = (uint)item.Count,
                PetStats = item.PetStats.Select(ToJsonStat).ToList(),
                BodyStats = item.BodyStats.Select(ToJsonStat).ToList(),
                HeaderStats = item.HeaderStats.Select(ToJsonStat).ToList(),
                Type = type,
                HasRecipe = item.HasRecipe,
                Buddies = item.Buddies.ToArray(),
                Skill = item.Skill != null ? GetJsonSkill(item.Skill) : null,
                GreenRarity = (int)item.PrefixRarity,
                HasCloudBackup = isCloudSynced,
                Slot = SlotTranslator.Translate(RuntimeSettings.Language, item.Slot ?? ""),
                Extras = extras,
                IsMonsterInfrequent = item.ModifiedSkills.Any(s => s.IsMonsterInfrequent),
            };

            var modifiedSkills = item.ModifiedSkills;
            foreach (var modifiedSkill in modifiedSkills) {
                var translated = modifiedSkill.Translated;
                foreach (var stat in translated.Select(ToJsonStat)) {
                    json.BodyStats.Add(stat);
                }

                if (translated.Count == 0 && !(modifiedSkill.Class == null || modifiedSkill.Tier == null)) {
                    string[] uri = json.URL.Select(o => o.ToString()).ToArray();
                    
                    var error = $@"Could not translate skill-modifier on: '{item.Name}', {json.BaseRecord} - {string.Join(";", uri)}";
                    ExceptionReporter.ReportIssue(error);
                    Logger.Debug($"Could not translate skill-modifier stats for \"{item.Name}\"");
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

        public static List<JsonItem> ToJsonSerializable(List<PlayerHeldItem> items) {
            var jsonItems = new List<JsonItem>(items.Count);
            foreach (var item in items) {
                if (item is PlayerItem pi && !string.IsNullOrEmpty(pi.CachedStats)) {
                    try {
                        var cached = JsonConvert.DeserializeObject<JsonItem>(pi.CachedStats);
                        cached.NumItems = (uint)item.Count; // Can't use a cached item count, no idea how many items are left.
                        cached.InitialNumItems = (uint)item.Count;
                        cached.HasCloudBackup = pi.IsCloudSynchronized;
                        cached.Buddies = pi.Buddies.ToArray();
                        jsonItems.Add(cached);
                    }
                    catch (Exception ex) {
                        Logger.Warn("Error deserializing items, please update player item stats on the Grim Dawn tab", ex);
                        // TODO: Is it safe to do a regular conversion here?
                    }
                }
                else {
                    jsonItems.Add(GetJsonItem(item));
                }
            }
            
            return jsonItems;
        }


        private static string GetSocketFromItem(string name) {
            if (!string.IsNullOrEmpty(name) && name.Contains("[")) {
                string[] tmp = name.Split('[');
                return $"({tmp[1].Replace("]", "")})";
            } else {
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
            } else {
                return name;
            }
        }
    }
}