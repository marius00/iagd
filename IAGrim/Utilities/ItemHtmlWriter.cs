using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Model;
using IAGrim.UI.Controller.dto;
using log4net;
using NHibernate.Dialect.Schema;
using StatTranslator;

namespace IAGrim.Utilities {

    internal static class ItemHtmlWriter {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemHtmlWriter));

        static ItemHtmlWriter() {
            CopyMissingFiles();
        }

        private static JsonSkill GetJsonSkill(PlayerItemSkill skill) {
            return new JsonSkill {
                Name = skill.Name,
                Description = skill.Description,
                Level = skill.Level,
                Trigger = skill.Trigger?.ToString(),
                PetStats = skill.PetStats.Select(m => new JsonStat { Label = m.ToString(), Extras = m.Extra?.ToString() }).ToList(),
                BodyStats = skill.BodyStats.Select(m => new JsonStat { Label = m.ToString(), Extras = m.Extra?.ToString() }).ToList(),
                HeaderStats = skill.HeaderStats.Select(m => new JsonStat { Label = m.ToString(), Extras = m.Extra?.ToString() }).ToList(),
            };
        }



        private static JsonItem GetJsonItem(PlayerHeldItem item) {
            // TODO: Modifiers

            bool isCloudSynced = false;
            object[] id = { item.Id, "", "", "", "" };
            if (item is PlayerItem pi) {
                id = new object[] { pi.Id, pi.BaseRecord, pi.PrefixRecord, pi.SuffixRecord, pi.MateriaRecord };
                isCloudSynced = !string.IsNullOrWhiteSpace(pi.AzureUuid);
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
                extras = ItemOperationsUtility.TranslateFaction(((AugmentationItem) item).Tags.FirstOrDefault(m => m.Stat == "factionSource")?.TextValue ?? string.Empty);
            }
            else {
                type = ItemTypeDto.Unknown;
            }



            var json = new JsonItem {
                BaseRecord = item.BaseRecord,
                URL = id,
                Icon = item.Bitmap,
                Name = PureItemName(item.Name),
                Quality = item.Rarity,
                Level = item.MinimumLevel,
                Socket = GetSocketFromItem(item?.Name),
                NumItems = item.Count,
                PetStats = item.PetStats.Select(m => new JsonStat { Label = m.ToString(), Extras = m.Extra?.ToString() }).ToList(),
                BodyStats = item.BodyStats.Select(m => new JsonStat { Label = m.ToString(), Extras = m.Extra?.ToString() }).ToList(),
                HeaderStats = item.HeaderStats.Select(m => new JsonStat { Label = m.ToString(), Extras = m.Extra?.ToString() }).ToList(),
                Type = type,
                HasRecipe = item.HasRecipe,
                Buddies = item.Buddies.ToArray(),
                Skill = item.Skill != null ? GetJsonSkill(item.Skill) : null,
                GreenRarity = item.PrefixRarity,
                HasCloudBackup = isCloudSynced,
                Slot = SlotTranslator.Translate(item.Slot ?? ""),
                Extras = extras
            };

            var modifiedSkills = item.ModifiedSkills;
            foreach (var modifiedSkill in modifiedSkills) {
                var translated = modifiedSkill.Translated;
                foreach (var translatedStat in translated) {
                    json.BodyStats.Add(new JsonStat {
                        Label = translatedStat.ToString(),
                        Extras = translatedStat.Extra?.ToString()
                    });
                }

                if (translated.Count == 0) {
                    Logger.Debug($"Could not translate skill-modifier stats for \"{item.Name}\"");
                }
            }

            return json;
        }

        /// <summary>
        /// Copy any css/js files from the app\resource folder to the items working directory
        /// </summary>
        public static void CopyMissingFiles() {
            string appResFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");

            foreach (string dirPath in Directory.GetDirectories(appResFolder, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(appResFolder, GlobalPaths.StorageFolder));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(appResFolder, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(appResFolder, GlobalPaths.StorageFolder), true);
            }
        }

        public static List<JsonItem> ToJsonSerializeable(List<PlayerHeldItem> items) {
            var jsonItems = new List<JsonItem>(items.Select(GetJsonItem));
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