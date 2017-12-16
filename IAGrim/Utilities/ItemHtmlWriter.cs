using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IAGrim.Database.Model;
using log4net;
using StatTranslator;

namespace IAGrim.Utilities {

    internal static class ItemHtmlWriter {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemHtmlWriter));

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

            object[] id = { item.Id, "", "", "", "" };
            PlayerItem pi = item as PlayerItem;
            if (pi != null) {
                id = new object[] { pi.Id, pi.BaseRecord, pi.PrefixRecord, pi.SuffixRecord, pi.MateriaRecord };
            }


            int type;
            if (item.IsRecipe)
                type = 0;
            else if (!string.IsNullOrEmpty(item.Stash))
                type = 1;
            else
                type = 2;





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
                GreenRarity = item.PrefixRarity
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
        private static void CopyMissingFiles() {
            string appResFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");

            foreach (string dirPath in Directory.GetDirectories(appResFolder, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(appResFolder, GlobalPaths.StorageFolder));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(appResFolder, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(appResFolder, GlobalPaths.StorageFolder), true);
            }

            /*
            foreach (var pattern in new string[] { "*.css", "*.js" }) {
                foreach (var file in Directory.GetFiles(appResFolder, pattern)) {
                    FileInfo fileInfo = new FileInfo(file);

                    var target = Path.Combine(GlobalPaths.StorageFolder, fileInfo.Name);
                    //if (!File.Exists(target))
                    File.Copy(file, target, true);
                }
            }

            var recipeFileSrc = Path.Combine(appResFolder, "recipe.png");
            var recipeFileDst = Path.Combine(GlobalPaths.StorageFolder, "recipe.png");
            if (!File.Exists(recipeFileDst))
                File.Copy(recipeFileSrc, recipeFileDst);*/
        }

        public static List<JsonItem> ToJsonSerializeable(List<PlayerHeldItem> items) {
            CopyMissingFiles();

            string src = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "item-kjs.html");
            string dst = GlobalPaths.ItemsHtmlFile;

            File.Copy(src, dst, true); // Redundant really, static file now

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